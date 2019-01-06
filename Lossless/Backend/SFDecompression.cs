using Lossless.Backend;
using System;
using System.IO;

namespace Lossless
{
    class SFDecompression
    {
        public delegate void UpdateStep(float counter, float max);
        public delegate void CompleteStep();
        public event UpdateStep UpdateEvent;
        public event CompleteStep CompleteEvent;

        private const int MAX_TREE_NODES = 511;

        private  uint readBit(ref BitStream stream)
        {
            if (stream.Index == stream.BytePointer.Length)
            {
                throw new StopReadingException();
            }
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

        public byte[] Decompress(byte[] input)
        {
            MemoryStream memoryStream = new MemoryStream();
            uint inputSize = (uint)input.Length;
            TreeNode[] nodes = new TreeNode[MAX_TREE_NODES];

            for (int counter = 0; counter < nodes.Length; counter++)
            {
                nodes[counter] = new TreeNode();
            }

            TreeNode root, node;
            BitStream stream = new BitStream();
            uint  nodeCount;

            if (inputSize < 1) return null;

            stream.BytePointer = input;
            stream.BitPosition = 0;

            nodeCount = 0;
            root = recoverTree(nodes, ref stream, ref nodeCount);

            uint indexer = (stream.Index * 8) + stream.BitPosition;
            bool stop = false;
            while(indexer < (stream.BytePointer.Length*8))
            {
                if (stream.Index == stream.BytePointer.Length - 1 && stream.BitPosition != 0)
                {
                    uint bitPosition = stream.BitPosition;
                    bool allOthersBitsTheSame = true;
                    uint template = (uint)(Convert.ToBoolean(stream.BytePointer[stream.Index] & (1 << (int)(7 - bitPosition))) ? 1 : 0);
                    bitPosition++;
                    for (; bitPosition < 8; bitPosition++)
                    {
                        uint x = (uint)(Convert.ToBoolean(stream.BytePointer[stream.Index] & (1 << (int)(7 - bitPosition))) ? 1 : 0);
                        if (template != x)
                        {
                            allOthersBitsTheSame = false;
                            break;
                        }
                    }
                    if (allOthersBitsTheSame) stop = true;
                }

                node = root;
                try
                {
                    while (node.Symbol < 0)
                    {
                        if (Convert.ToBoolean(readBit(ref stream)))
                            node = node.ChildB;
                        else
                            node = node.ChildA;
                    }
                }
                catch(StopReadingException e)
                {
                    stop = true;
                }
           
                if (stop) break;

                if (indexer % ((stream.BytePointer.Length * 8 / 100)+1) == 0)
                {
                    if (UpdateEvent != null)
                    {
                        UpdateEvent.Invoke(indexer, stream.BytePointer.Length * 8);
                    }
                }
                
                memoryStream.Append((byte)node.Symbol);

                indexer = (stream.Index * 8) + stream.BitPosition;
            }
            if (CompleteEvent != null)
            {
                CompleteEvent.Invoke();
            }

            return memoryStream.ToArray();
        }

    }
}
