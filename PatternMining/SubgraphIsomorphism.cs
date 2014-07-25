using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{
    class SubgraphIsomorphism
    {
        public Graph g1;
        public Graph g2;
        public int R;
        public List<int>[] Nodes1;
        public List<int>[] Nodes2;
        public int[] Map;
        public bool Isomorphic;
        public bool[] Used;

        public SubgraphIsomorphism(Graph g1, Graph g2, int R) 
        {
            this.g1 = g1;
            this.g2 = g2;
            this.R = R;
            Nodes1 = new List<int>[R];
            Nodes2 = new List<int>[R];
            for (int i = 0; i < R; ++i)
            {
                Nodes1[i] = new List<int>();
                Nodes2[i] = new List<int>();
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
                for (int v = u + 1; v < g2.n; ++v)
                {
                    int v1 = Map[v];
                    if (!(g2.adj[u].Contains(v) && g1.adj[u1].Contains(v1)))
                        return false;
                }
            }
            return true;
        }

        public List<List<int>> findPotential()
        {
            List<List<int>> potential = new List<List<int>>();
            for (int u = 0; u < g2.n; ++u)
            {
                potential.Add(new List<int>());
            }

            for (int i = 0; i < R; ++i)
            {
                for (int i1 = 0; i1 < Nodes1[i].Count; ++i1)
                {
                    int u = Nodes1[i][i1];
                    for (int i2 = 0; i2 < Nodes2[i].Count; ++i2)
                    {
                        int v = Nodes2[i][i2];
                        if (g2.getLabel(v).Equals(g1.getLabel(u)))
                            potential[v].Add(u);
                    }
                }
            }
            return potential;
        }

        public void dfs(List<List<int>> potential, int index)
        {
            if (index >= potential.Count)
            {
                if (checkEdges())
                    Isomorphic = true;
                return;
            }
            for (int i = 0; i < potential[index].Count; ++i)
            {
                int u = potential[index][i];
                if (!Used[u])
                {
                    Map[index] = u;
                    Used[u] = true;
                    dfs(potential, index + 1);
                    if (Isomorphic)
                        return;
                    Used[u] = false;
                }
            }

        }

        public bool containPattern()
        {
            Isomorphic = false;
            if (g1.n < g2.n) return false;

            int pivot_g = g1.getPivot();
            int pivot_p = g2.getPivot();

            bool[] vis = new bool[g1.n];
            for (int i = 0; i < vis.Length; ++i)
            {
                vis[i] = false;
            }
            int[] que = new int[g1.n];
            int front =0, rear = 0;           
            que[rear++] = pivot_g;
            vis[pivot_g] = true;

            int step = 0;
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
                step++;
                if (step >= R)
                    break;
            }

            for (int i = 0; i < vis.Length; ++i)
            {
                vis[i] = false;
            }
            front = rear = 0;           
            que[rear++] = pivot_p;
            vis[pivot_p] = true;

            step = 0;
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
                step++;
                if (step >= R)
                    break;
            }

            List<List<int>> potential = findPotential();

            for (int i = 0; i < potential.Count; ++i)
            {
                if (potential[i].Count == 0)
                    return false;
            }
            for (int i = 0; i < g1.n; ++i)
            {
                Used[i] = false;
            }
            dfs(potential, 0);
            return Isomorphic;
        }
    }
}
