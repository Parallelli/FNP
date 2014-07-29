using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{
    class Path
    {
        private List<int> nodeSeq;
        public Path() { nodeSeq = new List<int>(); }
        public void appendNode(int nodeID)
        {
            nodeSeq.Add(nodeID);
        }
        internal int getFirstNode()
        {
            var ret = 0;
            try
            {
                ret = nodeSeq[0];
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return ret;
        }

        internal int getLastNode()
        {
            var ret = 0;
            try
            {
                ret = nodeSeq[nodeSeq.Count - 1];
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return ret;
        }

        internal bool hasNode(int neighbor)
        {
            foreach (int node in nodeSeq)
            {
                if (node == neighbor) return true;
            }
            return false;
        }
    }
}
