using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lossless
{
    class TreeNode
    {
        public TreeNode Parent { get; set; }
        public IList<TreeNode> Children { get; set; }
        public Dictionary<char,int> Data { get; set; }
        public byte BinaryCode { get; set; }


        public TreeNode()
        {
            Data = new Dictionary<char, int>();
        }

    }
}
