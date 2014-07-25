using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    
            Graph graph = new Graph();
            graph.buildGraph(GlobalVar.inputFilePath);
            graph.printGraph();
            BuildingBlock bb = new BuildingBlock();
            List<Graph> bbGraphs = bb.getBuildingBlockGraph(graph);
        }
    }
}
