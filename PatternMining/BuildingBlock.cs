using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatternMining
{   
    class BuildingBlock
    {
        public List<Graph> getBuildingBlockGraph(Graph graph)
        {
            List<PathPattern> bb = createBuildingBlocks(graph);
            Console.WriteLine("building block done");
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
            Queue<PathPattern> Q = new Queue<PathPattern>();
            PathPattern empty = new PathPattern(); //when empty path, VID has nothing
            Dictionary<string, List<int>> patternVids = new Dictionary<string, List<int>>();
            Q.Enqueue(empty);
            int cnt = 0;
            while (Q.Count > 0)
            {
                PathPattern cur = Q.Dequeue();
                //var tmpList = cur.getPathPattern();
                //bool tmpSign = false;
                //if (tmpList.Count == 1 && tmpList[0] == "paper")
                //{
                //    tmpSign = true;
                //}
                Console.WriteLine("Extending the " + cnt + "-th building block\t" + cur.getPatternSize());
                cnt++;
                Dictionary<string, int> countNextPath = new Dictionary<string, int>();//next label supp
                patternVids.Clear();
                
                if (cur.getPatternSize() >= GlobalVar.radius) continue;
                if (cur.getPatternSize() == 0) //empty path to extend
                {
                    for (int i = 0; i < graph.n; i++)
                    {
                        var curLabel = graph.getLabel(i);
                        var curCnt = 0;                     
                        if (countNextPath.TryGetValue(curLabel, out curCnt))
                        {
                            countNextPath[curLabel]++;
                            patternVids[graph.getLabel(i)].Add(i);
                        }
                        else
                        {
                            countNextPath.Add(curLabel, 1);
                            patternVids.Add(graph.getLabel(i), new List<int>(i));
                        }                      
                    }
                }
                else // non-empty pathPattern
                {
                    try
                    {
                        foreach (var pivot in cur.vid)
                        {
                            //dfs(pivot)
                            bool[] vis = new bool [graph.n];
                            vis.Initialize();
                            vis[pivot] = true;
                            var newLabels = dfs(graph, pivot, cur.getPathPattern(), cur.getPatternSize(), 1, vis);
                            //if newLabel is not empty
                            //append new label to current pattern 
                            
                            foreach (var newLabel in newLabels)
                            {
                                int curCnt = 0;
                                if (countNextPath.TryGetValue(newLabel, out curCnt))
                                {
                                    countNextPath[newLabel]++;
                                    patternVids[newLabel].Add(pivot);
                                }
                                else
                                {
                                    countNextPath.Add(newLabel, 1);
                                    patternVids.Add(newLabel, new List<int>(pivot));
                                }
                            }                        
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Check your code.");
                        Console.WriteLine(e.StackTrace);
                    }
                }
                //check support
                foreach (KeyValuePair<string, int> entry in countNextPath)
                {
                    var nextLabel = entry.Key;
                    if (entry.Value >= GlobalVar.minSup)
                    {
                        PathPattern newPattern = new PathPattern(cur);
                        newPattern.appendLabel(nextLabel);                    
                        foreach (var cur_vid in patternVids[nextLabel])
                        {
                            newPattern.vid.Add(cur_vid);
                        }
                        Q.Enqueue(newPattern);
                        buildingBlocks.Add(newPattern);
                    }
                }
            }
            return buildingBlocks;
        }

        private List<string> dfs(Graph graph, int id, List<string> existingLabelSeq, int seqSize, int curSize, bool[] vis)
        {
            List<string> res = new List<string>();
            if (curSize == seqSize)
            {
                foreach (int neighbor in graph.adj[id])
                {
                    res.Add(graph.getLabel(neighbor));
                }
            }
            else if (curSize > seqSize) return null;
            else
            {
                foreach (int neighbor in graph.adj[id])
                {
                    if (!vis[neighbor])
                    {
                        string expectedLabel = existingLabelSeq[curSize];
                        string mylabel = graph.getLabel(neighbor);
                        if (expectedLabel.Equals(mylabel))
                        {
                            vis[neighbor] = true;
                            List<string> tmp = dfs(graph, neighbor, existingLabelSeq, seqSize, curSize + 1, vis);
                            if (seqSize == curSize + 1)
                            {
                                foreach (var item in tmp)
                                {
                                    res.Add(item);
                                }
                            }
                            vis[neighbor] = false;
                        }
                    }
                }
            }
            return res;
        }
    }
}
