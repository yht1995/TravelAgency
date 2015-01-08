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

    [Serializable]
    public class Path
    {
        public List<String> Name;
        public bool[] Tag;
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
            Name = new List<String>();
            Tag = new bool[tagCount];
            Transit = 0;
            cityCount = 0;
        }

        public void AddCity(City city)
        {
            if (!Name.Contains(city.Name))
            {
                cityCount++;
            }
            Name.Add(city.Name);
            foreach(String tag in city.Tags)
            {
                int index;
                City.tagDictionary.TryGetValue(tag, out index);
                Tag[index] = true;
            }
        }

        public int CalcIValue(int[] rate)
        {
            int a = 0;
            for (int i = 0; i < tagCount; i++)
            {
                if (Tag[i])
                {
                    a += rate[i];
                }
            }
            return a;
        }
    }

    public class Agency
    {
        private const string filePath = "E:\\Data\\";
        private double alaph = 3;
        private double beta = 1;
        private double eta = 5;
        private const int bufSize = 500000;
        private int bufCount;

        private class searchNode
        {
            public searchNode parent;
            public City self;
            public int depth;
            public int cost;
        }

        public void PrepareData(AdjacencyGraph map)
        {
            //Parallel.ForEach(map.VertexList, (city) =>
            //{
            //    List<Path> pathData = new List<Path>();
            //    for (int i = 1; i <= 3; i++)
            //    {
            //        pathData.AddRange(EnumeratePath(map, city, i));
            //    }
            //    FileIO.ExportPathData(filePath + "\\" + city.Name + ".path", pathData);
            //});
            for (int i = 1; i <= 6; i++)
            {
                EnumeratePath(map, map.VertexList[0], i);
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
            bufCount = 0;
            s.Push(node);
            while (s.Count != 0)
            {
                searchNode top = s.Pop();
                if (top.self == start && top.parent != null)
                {
                    searchNode n = top;
                    Path p = new Path();
                    while (n != null)
                    {
                        p.AddCity(n.self);
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
                    FileIO.ExportPathData(filePath + "\\" + start.Name + "\\", result);
                    result.Clear();
                    bufCount = 0;
                }
            }
            FileIO.ExportPathData(filePath + "\\" + start.Name + "\\", result);
        }

        public Path BestPath(City city, int[] rate, int cityCount, int transitFee)
        {
            FileInfo fileInfo;
            int i = 0;
            do
            {
                i++;
                fileInfo = new FileInfo(filePath + "\\" + city.Name + "\\" + i.ToString() + ".path");
            } while (fileInfo.Exists);
            double tagSum = 0;
            foreach (int tag in rate)
            {
                tagSum += tag;
            }
            BlockingCollection<Path> betterList = new BlockingCollection<Path>();
            for (int tim = 0 ;tim * 4<i;tim++)
            {
                int end = 4 + 4 * tim < i ? 4 + 4 * tim : i;
                Parallel.For(1 + 4* tim,end , (index) =>
                {
                    Stream fStream = new FileStream(filePath + "\\" + city.Name + "\\" + index.ToString() + ".path", FileMode.Open, FileAccess.Read);
                    fStream.Position = 0;
                    StreamReader reader = new StreamReader(fStream);
                    while (!reader.EndOfStream)
                    {
                        List<Path> pathList = FileIO.ImportPathData(reader, bufSize);
                        Parallel.ForEach(pathList, p =>
                        {
                            p.Lvaule = alaph * p.CalcIValue(rate) / tagSum
                                + beta * CalcSValue(p, cityCount)
                                + eta * CalcTValue(p, transitFee);
                        });
                        betterList.Add(MaxLValuePath(pathList));
                    }
                    fStream.Close();
                });
            }
            return MaxLValuePath(betterList);
        }

        private Path MaxLValuePath(BlockingCollection<Path> pathList)
        {
            Path bestPath = null;
            double max = Double.NegativeInfinity;
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
            double max = Double.NegativeInfinity;
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
            return -(Convert.ToDouble(Math.Abs(real.CityCount - target)) / target);
        }

        private double CalcTValue(Path real, int target)
        {
            double T = (target - Convert.ToDouble(real.Transit)) / target;
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
