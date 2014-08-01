using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Gadgets
{
    class Program
    {
        static void Main(string[] args)
        {
            //f1: paper-author
            //f2: paper-conf
            //f3: conf-name (as label)
            string f1, f2, f3;
            //f1 = Console.ReadLine();
            //f2 = Console.ReadLine();
            //f3 = Console.ReadLine();
            f1 = @"C:\scratch\github\data\paper_author.txt";
            f2 = @"C:\scratch\github\data\paper_conf.txt";
            f3 = @"C:\scratch\github\data\dblp_new.txt";
            
            Dictionary<string, string> conf = getConfDict(@"C:\scratch\github\data\conf.txt");
            string[] lines = System.IO.File.ReadAllLines(f1);
            StreamWriter writer = new StreamWriter(f3);
            foreach (string line in lines)
            {
                string[] tokens = line.Split();
                string outline = "p"+tokens[0] + "\t" + "paper" + "\t" + "a"+tokens[1] + "\t" + "author";
                writer.WriteLine(outline);
            }
            //
            //start read paper-conf
            lines = File.ReadAllLines(f2);
            foreach (string line in lines)
            {
                string[] tokens = line.Split();
                string outline = "p"+tokens[0] + "\t" + "paper" + "\t" + "c"+tokens[1] + "\t" + conf[tokens[1]];
                writer.WriteLine(outline);
            }
            writer.Close();
        }

        private static Dictionary<string, string> getConfDict(string f2)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines(f2);
            foreach (string line in lines)
            {
                string[] tokens = line.Split();
                dict.Add(tokens[0], tokens[1]);
            }
            return dict;
        }
    }
}
