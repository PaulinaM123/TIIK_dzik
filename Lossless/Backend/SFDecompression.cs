using Lossless.Backend;
using System;

namespace Lossless
{
    class SFDecompression
    {
        public delegate void UpdateStep(uint counter, uint max);
        public delegate void CompleteStep();
        public event UpdateStep UpdateEvent;
        public event CompleteStep CompleteEvent;


        private const int MAX_TREE_NODES = 511;

        public class TreeNode
        {
            public TreeNode ChildA;
            public TreeNode ChildB;
            public int Symbol;
        }

        private  void initBitStream(ref BitStream stream, byte[] buffer)
        {
            stream.BytePointer = buffer;
            stream.BitPosition = 0;
        }

        private  uint readBit(ref BitStream stream)
        {
            byte[] buffer = stream.BytePointer;
            uint bit = stream.BitPosition;
            uint x = (uint)(Convert.ToBoolean(buffer[stream.Index] & (1 << (int)(7 - bit))) ? 1 : 0);
            bit = (bit + 1) & 7;

            if (!Convert.ToBoolean(bit))
            {
                ++stream.Index;
            }

            stream.BitPosition = bit;

            return x;
        }

        private  uint read8Bits(ref BitStream stream)
        {
            byte[] buffer = stream.BytePointer;
            uint bit = stream.BitPosition;
            uint x = (uint)((buffer[stream.Index] << (int)bit) | (buffer[stream.Index + 1] >> (int)(8 - bit)));
            ++stream.Index;

            return x;
        }

        private  TreeNode recoverTree(TreeNode[] nodes, ref BitStream stream, ref uint nodeNumber)
        {
            TreeNode thisNode;

            thisNode = nodes[nodeNumber];
            nodeNumber = nodeNumber + 1;

            thisNode.Symbol = -1;
            thisNode.ChildA = null;
            thisNode.ChildB = null;

            if (Convert.ToBoolean(readBit(ref stream)))
            {
                thisNode.Symbol = (int)read8Bits(ref stream);
                return thisNode;
            }

            if (Convert.ToBoolean(readBit(ref stream)))
            {
                thisNode.ChildA = recoverTree(nodes, ref stream, ref nodeNumber);
            }

            if (Convert.ToBoolean(readBit(ref stream)))
            {
                thisNode.ChildB = recoverTree(nodes, ref stream, ref nodeNumber);
            }

            return thisNode;
        }

        public  void Decompress(byte[] input, byte[] output, uint inputSize, uint outputSize)
        {
            TreeNode[] nodes = new TreeNode[MAX_TREE_NODES];

            for (int counter = 0; counter < nodes.Length; counter++)
            {
                nodes[counter] = new TreeNode();
            }

            TreeNode root, node;
            BitStream stream = new BitStream();
            uint i, nodeCount;
            byte[] buffer;

            if (inputSize < 1) return;

            initBitStream(ref stream, input);

            nodeCount = 0;
            root = recoverTree(nodes, ref stream, ref nodeCount);
            buffer = output;

            for (i = 0; i < outputSize; ++i)
            {
                node = root;

                while (node.Symbol < 0)
                {
                    if (Convert.ToBoolean(readBit(ref stream)))
                        node = node.ChildB;
                    else
                        node = node.ChildA;
                }

                if (i % ((outputSize / 100)+1) == 0)
                {
                    if (UpdateEvent != null)
                    {
                        UpdateEvent.Invoke(i, inputSize);
                    }
                }

                buffer[i] = (byte)node.Symbol;
            }
            if (CompleteEvent != null)
            {
                CompleteEvent.Invoke();
            }
        }
    }
}
