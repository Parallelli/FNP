using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PatternMining
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * set global parameters
             */
            GlobalVar.minSup = 5;
            GlobalVar.radius = 3;
            GlobalVar.inputFilePath = @"C:\scratch\github\data\dblp.txt";

            StreamWriter writer = new StreamWriter(@"C:\scratch\github\data\frequnetPatterns.txt"); 


            Graph graph = new Graph();
            graph.buildGraph(GlobalVar.inputFilePath);
            BuildingBlock bb = new BuildingBlock();
            List<Graph> bbGraphs = bb.getBuildingBlockGraph(graph);

            foreach (Graph bbi in bbGraphs)
            {
                bbi.printGraph();
            }
            Console.WriteLine("building block print done");

            List<List<Graph>> frequentPatterns = new List<List<Graph>>();
            for (int size = 0; size <= GlobalVar.radius; ++size)
            {
                frequentPatterns.Add(new List<Graph>());
            }
            foreach (Graph g in bbGraphs)
            {
                int size = g.m;
                frequentPatterns[size].Add(g);
            }
            Console.WriteLine("BuildingBlocks done");
            int s = 1;
            while (true)
            {
                int new_size = s + 1;
                while (frequentPatterns.Count < new_size + 1)
                    frequentPatterns.Add(new List<Graph>());
                bool flag = false;
                for (int i = 0; i < frequentPatterns[s].Count; ++i)
                {
                    for (int j = i; j < frequentPatterns[s].Count; ++j)
                    {
                        PatternPairOperation ppo = new PatternPairOperation(frequentPatterns[s][i], frequentPatterns[s][j], graph, 
                            frequentPatterns[new_size]);
                        List<Graph> newPatterns = ppo.generateNewPatterns();
                        if (newPatterns.Count > 0)
                            flag = true;
                        foreach (Graph g in newPatterns)
                            frequentPatterns[new_size].Add(g);
                    }
                }
                if (!flag)
                    break;
                s++;
            }

            int pattern_cnt = 0;
            for (int i = 0; i < frequentPatterns.Count; ++i)
            {
                for (int j = 0; j < frequentPatterns[i].Count; ++j)
                {
                    Graph tmp = frequentPatterns[i][j];
                    writer.WriteLine(pattern_cnt);
                    writer.WriteLine(tmp.n + "\t" + tmp.m + "\t" + tmp.pivot);
                    for (int u = 0; u < tmp.n; ++u)
                    {
                        for (int i1 = 0; i1 < tmp.adj[u].Count; ++i1)
                        {
                            int v = tmp.adj[u][i1];
                            writer.WriteLine(u + "\t" + v + "\t" + tmp.getLabel(u) + "\t" + tmp.getLabel(v));
                        }
                    }
                    writer.Flush();
                }
            }

            writer.Close();
        }
    }
}
