using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{
    class PatternPairOperation
    {
        public Graph Pattern1;
        public Graph Pattern2;
        public List<Graph> new_patterns;
        public List<Graph> old_patterns;
        public int R;

        public PatternPairOperation(Graph Pattern1, Graph Pattern2, List<Graph> old_patterns, int R)
        {
            this.Pattern1 = Pattern1;
            this.Pattern2 = Pattern2;
            new_patterns = new List<Graph>();
            this.old_patterns = old_patterns;
            this.R = R;
        }

        public bool qualified()
        {
            if (Math.Abs(Pattern1.n - Pattern2.n) > 1)
                return false;

            Dictionary<string, int> map1 = new Dictionary<string, int>();
            Dictionary<string, int> map2 = new Dictionary<string, int>();
            for (int u = 0; u < Pattern1.n; ++u)
            {
                string label = Pattern1.getLabel(u);
                if (!map1.ContainsKey(label))
                    map1[label] = 1;
                else
                    map1[label] = map1[label] + 1;
            }

            for (int u = 0; u < Pattern2.n; ++u)
            {
                string label = Pattern2.getLabel(u);
                if (!map2.ContainsKey(label))
                    map2[label] = 1;
                else
                    map2[label] = map2[label] + 1;
            }

            int dif_cnt = 0;
            foreach (string label in map1.Keys)
            {
                int cnt1 = map1[label];
                int cnt2 = 0;
                if (map2.ContainsKey(label))
                    cnt2 = map2[label];
                dif_cnt += Math.Abs(cnt1 - cnt2);
            }

            if (dif_cnt > 1)
                return false;
            else
                return true;
        }

        public int subPatternType(Graph sp)
        {
            bool flag = false;
            for (int u = 0; u < sp.n; ++u)
            {
                if (sp.getDeg(u) == 0 && u != sp.pivot)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
                return 1;   //sp has no isolated node or the isolated node is the pivot
            else
                return 0;   //sp has one isolated node which is not the pivot
        }

        public List<Graph> generateNewPatterns()
        {
            new_patterns.Clear();
            if (!qualified())
                return new_patterns;

            for (int u = 0; u < Pattern2.n; ++u)
            {
                for (int i = 0; i < Pattern2.adj[u].Count; ++i)
                {
                    int v = Pattern2.adj[u][i];
                    if (u <= v)
                    {
                        Graph subPattern = Pattern2.removeEdge(u, v);
                        int type = subPatternType(subPattern);
                        if (type == 0)
                        {
                            //todo  
                        }
                        else if (type == 1)
                        {
                            //todo
                        }
                    }
                }
            }

            return new_patterns;
        }

    }
}
