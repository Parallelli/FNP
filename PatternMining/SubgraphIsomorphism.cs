﻿using System;
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
                    for (int r1 = 1; r1 <= r; ++r1)
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
                if (checkEdges())
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
