using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{
    class SubgraphIsomorphism   //check if g2 is a subgraph of g1, both g1 and g2 are connected graphs
    {
        public Graph g1;
        public Graph g2;
        public List<int>[] Nodes1;
        public List<int>[] Nodes2;
        public List<int>[] potential;
        public int[] Map;
        public bool Isomorphic;
        public bool[] Used;

        public SubgraphIsomorphism(Graph g1, Graph g2) 
        {
            this.g1 = g1;
            this.g2 = g2;
            Nodes1 = new List<int>[GlobalVar.radius+1];
            Nodes2 = new List<int>[GlobalVar.radius+1];
            for (int i = 0; i <= GlobalVar.radius; ++i)
            {
                Nodes1[i] = new List<int>();
                Nodes2[i] = new List<int>();
            }
            potential = new List<int>[g2.n];
            for (int i = 0; i < g2.n; ++i)
            {
                potential[i] = new List<int>();
            }
            Map = new int[g2.n];
            Isomorphic = false;
            Used = new bool[g1.n];
        }

        public bool checkEdges()
        {
            for (int u = 0; u < g2.n; ++u)
            {
                int u1 = Map[u];
                for (int i = 0; i < g2.adj[u].Count; ++i)
                {
                    int v = g2.adj[u][i];
                    int v1 = Map[v];
                    if (!g1.adj[u1].Contains(v1))
                        return false;
                }
            }
            return true;
        }

        public void findPotential()
        {
            for (int u = 0; u < g2.n; ++u)
            {
                potential[u].Clear();
            }

            potential[g2.pivot].Add(g1.pivot);

            for (int r = 1; r <= GlobalVar.radius; ++r)
            {
                for (int i = 0; i < Nodes2[r].Count; ++i)
                {
                    int v = Nodes2[r][i];
                    int st = 1;
                    if (g1.n == g2.n && g1.m == g2.m)
                        st = r;
                    for (int r1 = st; r1 <= r; ++r1)
                    {
                        for (int i1 = 0; i1 < Nodes1[r1].Count; ++i1)
                        {
                            int u = Nodes1[r1][i1];
                            if(g2.getLabel(v).Equals(g1.getLabel(u)) && g1.getDeg(u) >= g2.getDeg(v))
                                potential[v].Add(u);
                        }
                    }
                }
            }
        }

        public void dfs(int index)
        {
            if (index >= g2.n)
            {
                //if (checkEdges())
                Isomorphic = true;
                return;
            }
            for (int i = 0; i < potential[index].Count; ++i)
            {
                int u = potential[index][i];

                if (!Used[u])
                {
                    bool flag = false;
                    for (int v = 0; v < index; ++v)
                    {
                        if (g2.adj[v].Contains(index))
                        {
                            if (!g1.adj[Map[v]].Contains(u))
                            {
                                flag = true;
                                break;
                            }
                        }
                    }

                    if (!flag)
                    {
                        Map[index] = u;
                        Used[u] = true;
                        dfs(index + 1);
                        if (Isomorphic)
                            return;
                        Used[u] = false;
                    }
                }
            }

        }

        public bool containPattern()
        {
            Isomorphic = false;
            if (g1.n < g2.n) return false;
            if (!g1.getLabel(g1.pivot).Equals(g2.getLabel(g2.pivot)))
                return false;
            if (g1.n != g2.n && g1.m == g2.m)
                return false;
            if (g1.n == g2.n && g1.m != g2.m)
                return false;

            if (g1.n == g2.n && g1.m == g2.m)
            {
                Dictionary<string, int> map1 = new Dictionary<string, int>();
                Dictionary<string, int> map2 = new Dictionary<string, int>();
                for (int u = 0; u < g1.n; ++u)
                {
                    string label = g1.getLabel(u);
                    if (!map1.ContainsKey(label))
                        map1[label] = 1;
                    else
                        map1[label] = map1[label] + 1;
                }
                for (int u = 0; u < g2.n; ++u)
                {
                    string label = g2.getLabel(u);
                    if (!map2.ContainsKey(label))
                        map2[label] = 1;
                    else
                        map2[label] = map2[label] + 1;
                }
                foreach (string label in map1.Keys)
                {
                    int cnt1 = map1[label];
                    int cnt2 = 0;
                    if (map2.ContainsKey(label))
                        cnt2 = map2[label];
                    if (cnt1 != cnt2)
                        return false;
                }
            }

            Dictionary<string, int> count1 = new Dictionary<string, int>();
            Dictionary<string, int> count2 = new Dictionary<string, int>();

            int pivot_g = g1.pivot;
            int pivot_p = g2.pivot;

            bool[] vis = new bool[g1.n];
            for (int i = 0; i < vis.Length; ++i)
            {
                vis[i] = false;
            }
            int[] que = new int[g1.n];
            int front =0, rear = 0;           
            que[rear++] = pivot_p;
            vis[pivot_p] = true;

            int step = 0;
            for (int i = 0; i < Nodes2.Length; ++i)
            {
                Nodes2[i].Clear();
            }
            while (front < rear)
            {
                int tmp_rear = rear;
                while (front < tmp_rear)
                {
                    int u = que[front++];
                    Nodes2[step].Add(u);

                    string label = g2.getLabel(u);
                    if (count2.ContainsKey(label))
                        count2[label] = count2[label] + 1;
                    else
                        count2[label] = 1;

                    if (step < GlobalVar.radius)
                    {
                        for (int i = 0; i < g2.adj[u].Count; ++i)
                        {
                            int v = g2.adj[u][i];
                            if (vis[v] == false)
                            {
                                vis[v] = true;
                                que[rear++] = v;
                            }
                        }
                    }
                }
                step++;
            }
            int depth = step - 1;
            int cnt_p = rear;

            for (int i = 0; i < vis.Length; ++i)
            {
                vis[i] = false;
            }
            front = rear = 0;           
            que[rear++] = pivot_g;
            vis[pivot_g] = true;

            step = 0;
            for (int i = 0; i < Nodes1.Length; ++i)
            {
                Nodes1[i].Clear();
            }
            while (front < rear)
            {
                int tmp_rear = rear;
                while (front < tmp_rear)
                {
                    int u = que[front++];
                    Nodes1[step].Add(u);
                    string label = g1.getLabel(u);
                    if (count1.ContainsKey(label))
                        count1[label] = count1[label] + 1;
                    else
                        count1[label] = 1;

                    if (step < depth)
                    {
                        for (int i = 0; i < g1.adj[u].Count; ++i)
                        {
                            int v = g1.adj[u][i];
                            if (vis[v] == false)
                            {
                                vis[v] = true;
                                que[rear++] = v;
                            }
                        }
                    }
                }
                step++;
            }
            int cnt_g = rear;
            if (cnt_g < cnt_p)
                return false;

            foreach (string label in count2.Keys)
            {
                int c2 = count2[label];
                int c1 = 0;
                if (count1.ContainsKey(label))
                    c1 = count1[label];
                if (c1 < c2)
                    return false;
            }

            findPotential();

            for (int i = 0; i < g2.n; ++i)
            {
                if (potential[i].Count == 0)
                    return false;
            }
            for (int i = 0; i < g1.n; ++i)
            {
                Used[i] = false;
            }
            dfs(0);
            return Isomorphic;
        }
    }
}
