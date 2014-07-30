using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{   
    class BuildingBlock
    {
        public BuildingBlock() { VID = new Dictionary<PathPattern, PathSet>(); }
        public List<Graph> getBuildingBlockGraph(Graph graph)
        {
            List<PathPattern> bb = createBuildingBlocks(graph);
            List<Graph> bbGraphs = new List<Graph>();
            foreach (PathPattern pp in bb)
            {
                Graph bbGraph = new Graph();
                bbGraph.buildGraph(pp.getPathPattern());
                foreach (int v in pp.vid)
                    bbGraph.vid.Add(v);
                bbGraphs.Add(bbGraph);
            }
            return bbGraphs;
        }

        private List<PathPattern> createBuildingBlocks(Graph graph)
        {
            List<PathPattern> buildingBlocks = new List<PathPattern>();
            Dictionary<int,int> dict = new Dictionary<int,int>();
            
            Dictionary<string, int> countNextPath = new Dictionary<string, int>();
            Dictionary<string, PathSet> labelPathset = new Dictionary<string, PathSet>();

            Queue<PathPattern> Q = new Queue<PathPattern>();
            PathPattern empty = new PathPattern(); //when empty path, VID has nothing
            Q.Enqueue(empty);
            while (Q.Count > 0)
            {
                PathPattern cur = Q.Dequeue();
                labelPathset.Clear();
                if (cur.getPatternSize() >= GlobalVar.radius) continue;
                if (cur.getPatternSize() == 0) //empty path to extend
                {
                    for (int i = 0; i < graph.n; i++)
                    {
                        var curCnt = 0;
                        if (countNextPath.TryGetValue(graph.getLabel(i), out curCnt))
                        {
                            countNextPath[graph.getLabel(i)]++;
                            Path cur_path = new Path();
                            cur_path.appendNode(i);
                            labelPathset[graph.getLabel(i)].addPath(cur_path);
                        }
                        else
                        {
                            countNextPath.Add(graph.getLabel(i), 1);
                            Path cur_path = new Path();
                            cur_path.appendNode(i);
                            PathSet cur_pathSet = new PathSet();
                            cur_pathSet.addPath(cur_path);
                            labelPathset.Add(graph.getLabel(i), cur_pathSet);
                        }
                    }
                }
                else // non-empty pathPattern
                {
                    try
                    {
                        PathSet ps = VID[cur];
                        foreach (Path path in ps.getPathSet())
                        {
                            int lastNodeInPath = path.getLastNode();
                            try
                            {
                                foreach (int neighbor in graph.adj[lastNodeInPath]) 
                                {
                                    if (!path.hasNode(neighbor)) // increase cur node's label to count[]
                                    {
                                        var curCnt = 0;
                                        if (countNextPath.TryGetValue(graph.getLabel(neighbor), out curCnt))
                                        {
                                            countNextPath[graph.getLabel(neighbor)]++;      //wrong support counting
                                            Path cur_path = new Path();
                                            cur_path.appendNode(neighbor);
                                            labelPathset[graph.getLabel(neighbor)].addPath(cur_path);
                                        }
                                        else
                                        {
                                            countNextPath.Add(graph.getLabel(neighbor), 1);
                                            Path cur_path = new Path();
                                            cur_path.appendNode(neighbor);
                                            PathSet cur_pathSet = new PathSet();
                                            cur_pathSet.addPath(cur_path);
                                            labelPathset.Add(graph.getLabel(neighbor), cur_pathSet);
                                        }
                                    }
                                }
                            }
                            catch(IndexOutOfRangeException e)
                            {
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("no such pattern in VID dictionary!\nCheck your code.");
                        Console.WriteLine(e.StackTrace);
                    }
                }

                foreach (KeyValuePair<string,int> entry in countNextPath)
                {
                    if (entry.Value >= GlobalVar.minSup)
                    {
                        PathPattern newPattern = cur;
                        newPattern.appendLabel(entry.Key);
                        Q.Enqueue(newPattern);
                        buildingBlocks.Add(newPattern);
                        //update VID
                        try
                        {
                            PathSet newPS = labelPathset[entry.Key];
                            if (VID.ContainsKey(newPattern))
                            {
                                foreach (Path p in newPS.getPathSet())
                                {
                                    VID[newPattern].addPath(p);
                                    newPattern.vid.Add(p.getFirstNode());   //construct new pattern's vid set
                                }
                            }
                            else
                                VID.Add(newPattern, newPS);
                        }
                        catch (KeyNotFoundException e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                }
            }
            return buildingBlocks;
        }
        public Dictionary<PathPattern, PathSet> VID;
    }
}
