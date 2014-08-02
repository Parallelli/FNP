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
        public Graph SubPattern;
        public Graph G;
        public List<Graph> new_patterns;
        public List<Graph> old_patterns;
        public List<Graph> nonFreq_patterns;
        public List<int>[] potential;
        public List<int>[] Nodes1;
        public List<int>[] Nodes2;
        public int[] Map;
        public bool[] Used;
        public int from, to;
        public int isoNode;
        public int noMapNode;

        public PatternPairOperation(Graph Pattern1, Graph Pattern2, Graph G, List<Graph> old_patterns)
        {
            if (Pattern1.n < Pattern2.n)
            {
                this.Pattern1 = Pattern2;
                this.Pattern2 = Pattern1;
            }
            else
            {
                this.Pattern1 = Pattern1;
                this.Pattern2 = Pattern2;
            }
            this.G = G;
            new_patterns = new List<Graph>();
            nonFreq_patterns = new List<Graph>();
            this.old_patterns = old_patterns;
            Map = new int[this.Pattern2.n];
            Used = new bool[this.Pattern1.n];
            Nodes1 = new List<int>[GlobalVar.radius + 1];
            Nodes2 = new List<int>[GlobalVar.radius + 1];
            for (int i = 0; i <= GlobalVar.radius; ++i)
            {
                Nodes1[i] = new List<int>();
                Nodes2[i] = new List<int>();
            }
            potential = new List<int>[this.Pattern2.n];
            for (int i = 0; i < this.Pattern2.n; ++i)
            {
                potential[i] = new List<int>();
            }
        }

        public bool qualified()
        {
            if (Math.Abs(Pattern1.n - Pattern2.n) > 1)
                return false;

            if (!Pattern1.getLabel(Pattern1.pivot).Equals(Pattern2.getLabel(Pattern2.pivot)))
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

            if (dif_cnt > 2)
                return false;
            else
                return true;
        }

        public void findIsoNode(Graph sp)
        {
            isoNode = -1;
            //bool flag = false;
            for (int u = 0; u < sp.n; ++u)
            {
                if (sp.getDeg(u) == 0 && u != sp.pivot)
                {
                    isoNode = u;
                    //flag = true;
                    break;
                }
            }
            //if (flag)
            //    return 1;   //sp has no isolated node or the isolated node is the pivot
            //else
            //    return 0;   //sp has one isolated node which is not the pivot
        }

        public void findPotential()
        {
            for (int u = 0; u < SubPattern.n; ++u)
            {
                potential[u].Clear();
            }

            potential[SubPattern.pivot].Add(Pattern1.pivot);

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
                            if (SubPattern.getLabel(v).Equals(Pattern1.getLabel(u)) && Pattern1.getDeg(u) >= SubPattern.getDeg(v) 
                                && Pattern1.getDeg(u) <= SubPattern.getDeg(v) + 1)
                                potential[v].Add(u);
                        }
                    }
                }
            }
        }

        public bool checkEdges()
        {
            if (isoNode == -1)
            {
                int u1 = Map[from];
                int u2 = Map[to];
                if (Pattern1.adj[u1].Contains(u2))
                    return false;
            }
            for (int u = 0; u < SubPattern.n; ++u)
            {
                if (u != isoNode)
                {
                    int u1 = Map[u];
                    for (int i = 0; i < SubPattern.adj[u].Count; ++i)
                    {
                        int v = SubPattern.adj[u][i];
                        if (v != isoNode)
                        {
                            int v1 = Map[v];
                            if (!Pattern1.adj[u1].Contains(v1))
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool validPattern(Graph p)
        {
            //Dictionary<string, int> count = new Dictionary<string, int>();
            //for (int u = 0; u < p.n; ++u)
            //{
            //    string label = p.getLabel(u);
            //    if (!count.ContainsKey(label))
            //        count[label] = 1;
            //    else
            //        count[label] = count[label] + 1;
            //}
            //foreach (string label in count.Keys)
            //{
            //    if ((double)(count[label]) >= 0.8 * (double)(p.n))
            //    {
            //        Console.WriteLine("Too many nodes with same label");
            //        return false;
            //    }
            //}

            bool[] vis = new bool[p.n];
            for (int i = 0; i < vis.Length; ++i)
                vis[i] = false;
            int[] que = new int[p.n];
            int front = 0, rear = 0;
            que[rear++] = p.pivot;
            vis[p.pivot] = true;

            int step = 0;
            while (front < rear)
            {
                int tmp_rear = rear;
                while (front < tmp_rear)
                {
                    int u = que[front++];
                    for (int i = 0; i < p.adj[u].Count; ++i)
                    {
                        int v = p.adj[u][i];
                        if (vis[v] == false)
                        {
                            vis[v] = true;
                            que[rear++] = v;
                        }
                    }
                }
                step++;
            }
            if (step > GlobalVar.radius + 1)
            {
                Console.WriteLine("New Pattern is to large");
                return false;
            }

            bool flag = false;
            for (int i = 0; i < old_patterns.Count; ++i)
            {
                SubgraphIsomorphism si = new SubgraphIsomorphism(old_patterns[i], p);
                if (si.containPattern())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                Console.WriteLine("New Pattern already exists");
                return false;
            }
            for (int i = 0; i < new_patterns.Count; ++i)
            {
                SubgraphIsomorphism si = new SubgraphIsomorphism(new_patterns[i], p);
                if (si.containPattern())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                Console.WriteLine("New Pattern already exists");
                return false;
            }

            for (int i = 0; i < nonFreq_patterns.Count; ++i)
            {
                SubgraphIsomorphism si = new SubgraphIsomorphism(nonFreq_patterns[i], p);
                if (si.containPattern())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                Console.WriteLine("New Pattern is not frequent");
                return false;
            }

            HashSet<int> checkNodes = new HashSet<int>();
            foreach (int v in Pattern1.vid)
                if (Pattern2.vid.Contains(v))
                    checkNodes.Add(v);
            int cnt = 0;
            int num = 0;
            foreach (int v in checkNodes)
            {
                if (cnt + checkNodes.Count - num < GlobalVar.minSup)
                    break;
                num++;
                G.pivot = v;
                SubgraphIsomorphism si = new SubgraphIsomorphism(G, p);
                if (si.containPattern())
                {
                    cnt++;
                    p.vid.Add(v);
                }
            }
            /*foreach (int v in Pattern1.vid)
            {
                if (Pattern2.vid.Contains(v))
                {
                    G.pivot = v;
                    SubgraphIsomorphism si = new SubgraphIsomorphism(G, p);
                    if (si.containPattern())
                    {
                        cnt++;
                        p.vid.Add(v);
                    }
                }
            }*/
            if (cnt < GlobalVar.minSup)
            {
                Console.WriteLine("Support of New Pattern is less than the threshold " + cnt);
                nonFreq_patterns.Add(p);
                return false;
            }
            return true;
        }

        public void join()
        {
            if (isoNode != -1)
            {
                for (int u = 0; u < Pattern1.n; ++u)
                {
                    if (Used[u] == false)
                    {
                        noMapNode = u;
                        break;
                    }
                }
            }
            if (isoNode == -1 || (isoNode != -1 && Pattern1.getLabel(noMapNode).Equals(SubPattern.getLabel(isoNode))))
            {
                int from1, to1;
                if (isoNode != -1)
                {
                    if (isoNode == from)
                    {
                        from1 = noMapNode;
                        to1 = Map[to];
                    }
                    else
                    {
                        from1 = Map[from];
                        to1 = noMapNode;
                    }
                }
                else
                {
                    from1 = Map[from];
                    to1 = Map[to];
                }
                if (!Pattern1.adj[from1].Contains(to1))
                {
                    Graph newG = Pattern1.insertEdge(from1, to1);
                    Console.WriteLine("test new pattern");
                    //newG.printGraph();
                    if (validPattern(newG))
                    {
                        new_patterns.Add(newG);
                        Console.WriteLine("Find a new Pattern");
                    }
                }
            }
            if(isoNode != -1)
            {
                int to1;
                if (isoNode == from)
                    to1 = Map[to];
                else
                    to1 = Map[from];
                Graph newG = Pattern1.insertEdge(SubPattern.getLabel(isoNode), to1);
                Console.WriteLine("test new pattern");
                newG.printGraph();
                if (validPattern(newG))
                {
                    new_patterns.Add(newG);
                    Console.WriteLine("Find a new Pattern");
                }
            }
        }

        public void dfs(int index)
        {
            if (index >= SubPattern.n)
            {
                if (checkEdges())
                {
                    join();
                }
                return;
            }
            if (index == isoNode)
                dfs(index + 1);
            for (int i = 0; i < potential[index].Count; ++i)
            {
                int u = potential[index][i];
                if (!Used[u])
                {
                    Map[index] = u;
                    Used[u] = true;
                    dfs(index + 1);
                    Used[u] = false;
                }
            }

        }

        public void joinPatterns()
        {
            for (int i = 0; i <= GlobalVar.radius; ++i)
            {
                Nodes1[i].Clear();
                Nodes2[i].Clear();
            }

            bool[] vis = new bool[Pattern1.n];
            for (int i = 0; i < vis.Length; ++i)
            {
                vis[i] = false;
            }
            int[] que = new int[Pattern1.n];
            int front = 0, rear = 0;
            que[rear++] = Pattern1.pivot;
            vis[Pattern1.pivot] = true;

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

                    if (step < GlobalVar.radius)
                    {
                        for (int i = 0; i < Pattern1.adj[u].Count; ++i)
                        {
                            int v = Pattern1.adj[u][i];
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
            int depth = step-1;

            for (int i = 0; i < vis.Length; ++i)
            {
                vis[i] = false;
            }
            front = rear = 0;
            que[rear++] = SubPattern.pivot;
            vis[SubPattern.pivot] = true;

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

                    if (step < GlobalVar.radius)
                    {
                        for (int i = 0; i < SubPattern.adj[u].Count; ++i)
                        {
                            int v = SubPattern.adj[u][i];
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

            for (int v = 0; v < SubPattern.n; ++v)
            {
                if (vis[v] == false)
                    Nodes2[depth].Add(v);
            }

            findPotential();

            for (int i = 0; i < SubPattern.n; ++i)
            {
                if (potential[i].Count == 0 && i != isoNode)
                    return;
            }
            for (int i = 0; i < Pattern1.n; ++i)
            {
                Used[i] = false;
            }
            dfs(0);
        }

        public List<Graph> generateNewPatterns()
        {
            if (!qualified())
                return new_patterns;

            for (int u = 0; u < Pattern2.n; ++u)
            {
                for (int i = 0; i < Pattern2.adj[u].Count; ++i)
                {
                    int v = Pattern2.adj[u][i];
                    if (u <= v)
                    {
                        from = u;
                        to = v;
                        SubPattern = Pattern2.removeEdge(u, v);
                        //Console.WriteLine("\nSubPattern:");
                        //SubPattern.printGraph();
                        findIsoNode(SubPattern);
                        //Console.WriteLine("IsoNode=" + isoNode + "\n");
                        joinPatterns();
                    }
                }
            }

            return new_patterns;
        }
    }
}
