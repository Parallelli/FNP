using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{
    class PathPattern : IEquatable<PathPattern>
    {
        public PathPattern() { labelSeq = new List<string>(); patternSize = 0; vid = new HashSet<int>(); }

        public void appendLabel(string label)
        {
            labelSeq.Add(label);
            patternSize++;
        }
        public List<string> getPathPattern()
        {
            return labelSeq;
        }

        private List<string> labelSeq;
        private int patternSize;
        public HashSet<int> vid; //pivots set
        public int getPatternSize() { return patternSize; }

        #region IEquatable<PathPattern> patterns;
        public override int GetHashCode()
        {
            return labelSeq.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as PathPattern);
        }
        public bool Equals(PathPattern obj)
        {
            bool ret = true;
            if (obj == null) ret = false;
            else
            {
                int s1 = obj.patternSize;
                int s2 = this.patternSize;
                if (s1 != s2) ret = false;
                else
                {
                    for (int i = 0; i < s1; i++)
                        if (obj.labelSeq[i].CompareTo(this.labelSeq[i]) != 0)
                        {
                            ret = false;
                            break;
                        }
                }
            }
            return ret;
        }
        #endregion

    }
}
