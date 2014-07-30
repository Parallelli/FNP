using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PatternMining
{
    class Graph
    {
        public Graph() 
        { 
            n = 0; 
            m = 0;
            adj = new List<List<int>>();
            labels = new List<string>();
            deg = new List<int>();
            pivot = 0;
        }

        public void buildGraph(string file)
        {
            string[] lines = File.ReadAllLines(file);
            int idCnt = 0;
            int edgeCnt = 0;
            Dictionary<string, int> idMap = new Dictionary<string, int>();
            Console.WriteLine("start building graph");
            foreach (string line in lines)
            {
                edgeCnt++;
                string[] tokens = line.Split();
                // ID, Label, ID, Label
                int id1, id2;
                bool hasShow1 = false;
                bool hasShow2 = false;
                if (idMap.ContainsKey(tokens[0]))
                {
                    id1 = idMap[tokens[0]];
                    hasShow1 = true;
                }
                else
                {
                    id1 = idCnt++;
                    labels.Add(tokens[1]);
                    idMap.Add(tokens[0], id1);
                }
               
                if (idMap.ContainsKey(tokens[2]))
                {
                    id2 = idMap[tokens[2]];
                    hasShow2 = true;
                }
                else
                {
                    id2 = idCnt++;
                    idMap.Add(tokens[2], id2);
                    try
                    {
                        labels.Add(tokens[3]);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine(line);
                        Console.WriteLine(e.StackTrace);
                        
                    }
                }
                if (hasShow1)
                {
                    adj[id1].Add(id2);
                    deg[id1]++;
                }
                else
                {
                    List<int> tmp = new List<int>();
                    tmp.Add(id2);
                    adj.Add(tmp);
                    deg.Add(1);
                }
                if (hasShow2)
                {
                    adj[id2].Add(id1);
                    deg[id2]++;
                }
                else
                {
                    List<int> tmp = new List<int>();
                    tmp.Add(id1);
                    adj.Add(tmp);
                    deg.Add(1);
                }
            }
            
            n = idCnt;
            m = edgeCnt;
        }
        public void printGraph()
        {
            int cnt = adj.Count;
            Console.WriteLine("count: " + cnt + " nodes: " + n + " edges: " + m);
            string test = Console.ReadLine();
            for(int i = 0; i < adj.Count; i++)
            {
                //Console.ReadLine();
                string outline = i + " ";
                foreach(int node in adj[i])
                {
                    outline += (node + " ");
                }
                Console.WriteLine(outline);
            }
        }
        public Graph removeEdge(int from, int to) // return a new graph after removing an edge
        {
           Graph g = new Graph();

           g.n = this.n;
           g.m = this.m - 1;
           g.pivot = (this.pivot);

           for (int u = 0; u < this.n; ++u)
           {
               g.labels.Add(this.labels[u]);
               if (u == from || u == to)
                   g.deg.Add(this.getDeg(u) - 1);
               else
                   g.deg.Add(this.getDeg(u));
               for (int i = 0; i < this.adj[u].Count; ++i)
               {
                   int v = this.adj[u][i];
                   if (!(u == from && v == to) && !(u == to && v == from))
                       g.adj[u].Add(v);
               }
           }

           return g;
        }

        public List<List<int>> adj;
        public List<string> labels;
        public List<int> deg;
        public int n { set; get; }
        public int m { set; get; }
        public int pivot { set; get; }
       
        public int getDeg(int nodeID)
        {
            return deg[nodeID];
        }
        public string getLabel(int nodeID)
        {
            return labels[nodeID];
        }
      
        internal void buildGraph(List<string> labelSeq) //build graph from a path, i.e. a-p-a
        {
            int nodeCnt = labelSeq.Count;
            int edgeCnt = labelSeq.Count - 1;
            for (int i = 0; i < labelSeq.Count; i++)
            {
                List<int> tmp = new List<int>();
                tmp.Add(i);
                labels[i] = labelSeq[i];
                if (i - 1 >= 0) tmp.Add(i - 1);
                if (i + 1 < labelSeq.Count) tmp.Add(i + 1);
                adj.Add(tmp);
                if (i > 0 && i < labelSeq.Count - 1) deg.Add(2);
                else deg.Add(1);
            }
            this.n = nodeCnt;
            this.m = edgeCnt;
            this.pivot = 0;
        }
    }
}
