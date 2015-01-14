using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Concurrent;

namespace TravelAgency
{
    public class Request
    {
        public String name;
        public int start;
        public int cityNum;
        public int total;
        public List<String> tagList;
        public List<int> rateList;

        public Request()
        {
            tagList = new List<String>();
            rateList = new List<int>();
        }
    }
    public class Path
    {
        public List<int> NameIndex;
        public List<int> Tag;
        public int Transit;
        private int cityCount;
        public double Lvaule;
        private static int tagCount = City.tagList.Count;
        public int CityCount
        {
            set { cityCount = value + 1; }
            get { return cityCount - 1; }
        }

        public Path()
        {
            NameIndex = new List<int>();
            Tag = new List<int>();
            Transit = 0;
            cityCount = 0;
        }
        public void AddCity(City city,int cityIndex)
        {
            if (!NameIndex.Contains(cityIndex))
            {
                cityCount++;
            }
            NameIndex.Add(cityIndex);
            foreach(String tag in city.Tags)
            {
                int index;
                City.tagDictionary.TryGetValue(tag, out index);
                if (!Tag.Contains(index))
                {
                    Tag.Add(index);
                }
            }
        }
        public int CalcIValue(int[] rate)
        {
            int a = 0;
            foreach(int tag in Tag)
            {
                a += rate[tag];
            }
            return a;
        }
    }

    public class Agency
    {
        private const string filePath = "E:\\Data";
        private double alaph = 3;
        private double beta = 1;
        private double eta = 5;
        private const int bufSize = 500000;

        private class searchNode
        {
            public searchNode parent;
            public City self;
            public int depth;
            public int cost;
        }
        public void PrepareData(AdjacencyGraph map)
        {
            foreach (City city in map.VertexList)
            {
                for (int i = 1; i <= 5; i++)
                {
                    EnumeratePath(map, city, i);
                }
            }
        }
        public void EnumeratePath(AdjacencyGraph map,City start,int count)
        {
            List<Path> result = new List<Path>();
            Stack<searchNode> s = new Stack<searchNode>();
            searchNode node = new searchNode();
            node.parent = null;
            node.self = start;
            node.depth = 0;
            int bufCount = 0;
            s.Push(node);
            while (s.Count != 0)
            {
                searchNode top = s.Pop();
                if (top.self == start && top.parent != null)
                {
                    searchNode n = top.parent;
                    Path p = new Path();
                    while (n.parent != null)
                    {
                        p.AddCity(n.self,map.GetCityIndex(n.self));
                        p.Transit += (n.cost + n.self.TransitFees);
                        n = n.parent;
                    }
                    p.Transit -= 2 * start.TransitFees;
                    result.Add(p);
                    bufCount++;
                }
                if (top.depth < count)
                {
                    foreach (Edge e in top.self.NeighborList)
                    {
                        searchNode next = new searchNode();
                        next.self = e.End;
                        next.parent = top;
                        next.depth = top.depth + 1;
                        next.cost = e.Value;
                        s.Push(next);
                    }
                }
                else if (top.depth == count)
                {
                    Edge back = start.GetEdge(top.self);
                    if (back != null)
                    {
                        searchNode next = new searchNode();
                        next.self = start;
                        next.parent = top;
                        next.depth = top.depth + 1;
                        next.cost = back.Value;
                        s.Push(next);
                    }
                }
                if (bufCount>=bufSize)
                {
                    FileIO.ExportPathData(filePath + "\\" + start.Name + "\\",result,count);
                    result.Clear();
                    bufCount = 0;
                }
            }
            FileIO.ExportPathData(filePath + "\\" + start.Name + "\\",result,count);
        }
        public Path BestPath(City city, int[] rate, int targetCityCount, int targetTransitFee)
        {
            FileInfo fileInfo;
            int i = 0;
            do
            {
                i++;
                fileInfo = new FileInfo(filePath + "\\" + city.Name + "\\" + i.ToString() + ".path");
            } while (fileInfo.Exists);
            int tagSum = 0;
            foreach (int tag in rate)
            {
                tagSum += tag;
            }
            BlockingCollection<Path> betterList = new BlockingCollection<Path>();
            for (int tim = 0 ;tim * 8 < i;tim++)
            {
                int end = 8 + 8 * tim < i ? 8 + 8 * tim : i;
                Parallel.For(1 + 8 * tim,end , (index) =>
                {
                    string path = filePath + "\\" + city.Name + "\\" + index.ToString() + ".path";
                    List<Path> pathList = FileIO.ImportPathData(path);
                    Parallel.ForEach(pathList, p =>
                    {
                        p.Lvaule = alaph * p.CalcIValue(rate) / tagSum
                            + beta * CalcSValue(p, targetCityCount)
                            + eta * CalcTValue(p, targetTransitFee);
                    });
                    betterList.Add(MaxLValuePath(pathList));
                });
            }
            return MaxLValuePath(betterList);
        }
        private Path MaxLValuePath(BlockingCollection<Path> pathList)
        {
            Path bestPath = null;
            double max = double.NegativeInfinity;
            foreach (Path p in pathList)
            {
                if (p.Lvaule > max)
                {
                    max = p.Lvaule;
                    bestPath = p;
                }
            }
            return bestPath;
        }
        private Path MaxLValuePath(List<Path> pathList)
        {
            Path bestPath = null;
            double max = double.NegativeInfinity;
            foreach (Path p in pathList)
            {
                if (p.Lvaule > max)
                {
                    max = p.Lvaule;
                    bestPath = p;
                }
            }
            return bestPath;
        }
        private double CalcSValue(Path real,int target)
        {
            return -((Math.Abs((double)real.CityCount - target)) / target);
        }
        private double CalcTValue(Path real, int target)
        {
            double T = ((double)target - real.Transit) / target;
            if (target >= real.Transit)
            {
                return T;
            }
            else
            {
                return 3 * T;
            }
        }
    }
}
