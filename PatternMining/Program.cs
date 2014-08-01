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
            GlobalVar.minSup = 50;
            GlobalVar.radius = 2;
            //GlobalVar.inputFilePath = @"D:\Nodes Similarity\Neighbor Pattern\data_dblp_patternmining\dblp.txt";  //C:\scratch\github\data
            //GlobalVar.inputFilePath = @"C:\scratch\github\data\dblp_new.txt";
            //StreamWriter writer = new StreamWriter(@"C:\scratch\github\data\frequnetPatterns.txt");
            StreamWriter writer = new StreamWriter(@"D:\Nodes Similarity\Neighbor Pattern\data_dblp_patternmining\frequnetPatterns.txt");  //C:\scratch\github\data
            GlobalVar.inputFilePath = @"D:\Nodes Similarity\Neighbor Pattern\data_dblp_patternmining\dblp_new.txt";  //C:\scratch\github\data

            //StreamWriter writer = new StreamWriter(@"D:\Nodes Similarity\Neighbor Pattern\data_dblp_patternmining\frequnetPatterns.txt");  //C:\scratch\github\data

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
            int s = 1;
            int pattern_cnt = 0;
            for (int j = 0; j < frequentPatterns[s].Count; ++j)
            {
                Graph tmp = frequentPatterns[s][j];
                writer.WriteLine((pattern_cnt++) + " support:" + tmp.vid.Count);
                int cnt = tmp.adj.Count;
                writer.WriteLine("count: " + cnt + " nodes: " + tmp.n + " edges: " + tmp.m + " pivot: " + tmp.pivot);
                for (int i = 0; i < tmp.adj.Count; i++)
                {
                    //Console.ReadLine();
                    string outline = i + " " + tmp.getLabel(i) + " ";
                    foreach (int node in tmp.adj[i])
                    {
                        outline += (node + " ");
                    }
                    writer.WriteLine(outline);
                }
                writer.Flush();

                string vidFile = "D:/Nodes Similarity/Neighbor Pattern/data_dblp_patternmining/patternVid/" + (pattern_cnt-1) + ".txt";
                StreamWriter vidWriter = new StreamWriter(vidFile);
                foreach (int v in tmp.vid)
                    vidWriter.WriteLine(v);
                vidWriter.Close();
            }
            while (true)
            {
                int new_size = s + 1;
                Console.WriteLine("Generating " + new_size + " edges pattern, #SubPatterns=" + frequentPatterns[s].Count);
                while (frequentPatterns.Count < new_size + 1)
                    frequentPatterns.Add(new List<Graph>());
                bool flag = false;
                for (int i = 0; i < frequentPatterns[s].Count; ++i)
                {
                    for (int j = i; j < frequentPatterns[s].Count; ++j)
                    {
                        int num = 0;
                        foreach (int v in frequentPatterns[s][i].vid)
                            if (frequentPatterns[s][j].vid.Contains(v))
                                num++;

                        Console.WriteLine(i + " " + j + " " + num);
                        //frequentPatterns[s][i].printGraph();
                        //frequentPatterns[s][j].printGraph();
                        PatternPairOperation ppo = new PatternPairOperation(frequentPatterns[s][i], frequentPatterns[s][j], graph,
                            frequentPatterns[new_size]);
                        List<Graph> newPatterns = ppo.generateNewPatterns();
                        if (newPatterns.Count > 0)
                            flag = true;
                        foreach (Graph g in newPatterns)
                            frequentPatterns[new_size].Add(g);
                    }
                }
                if (!flag && new_size > GlobalVar.radius)
                    break;
                else
                {
                    for (int j = 0; j < frequentPatterns[new_size].Count; ++j)
                    {
                        Graph tmp = frequentPatterns[new_size][j];
                        writer.WriteLine((pattern_cnt++) + " support:" + tmp.vid.Count);
                        int cnt = tmp.adj.Count;
                        writer.WriteLine("count: " + cnt + " nodes: " + tmp.n + " edges: " + tmp.m + " pivot: " + tmp.pivot);
                        for (int i = 0; i < tmp.adj.Count; i++)
                        {
                            //Console.ReadLine();
                            string outline = i + " " + tmp.getLabel(i) + " ";
                            foreach (int node in tmp.adj[i])
                            {
                                outline += (node + " ");
                            }
                            writer.WriteLine(outline);
                        }
                        writer.Flush();

                        string vidFile = "D:/Nodes Similarity/Neighbor Pattern/data_dblp_patternmining/patternVid/" + (pattern_cnt - 1) + ".txt";
                        StreamWriter vidWriter = new StreamWriter(vidFile);
                        foreach (int v in tmp.vid)
                            vidWriter.WriteLine(v);
                        vidWriter.Close();
                    }
                }
                s++;
            }


            /*for (int i = 0; i < frequentPatterns.Count; ++i)
            {
                for (int j = 0; j < frequentPatterns[i].Count; ++j)
                {
                    Graph tmp = frequentPatterns[i][j];
                    writer.WriteLine(pattern_cnt++);
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
            }*/

            writer.Close();
        }
    }
}
