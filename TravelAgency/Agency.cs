using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TravelAgency.Graph;

namespace TravelAgency
{
    public class Path
    {
        private int cityCount;
        public double lvaule;
        public List<int> nameIndex;
        public List<int> tag;
        public int transit;

        public Path()
        {
            nameIndex = new List<int>();
            tag = new List<int>();
            transit = 0;
            cityCount = 0;
        }

        public int CityCount
        {
            set { cityCount = value + 1; }
            get { return cityCount - 1; }
        }

        public void AddCity(City city, int cityIndex)
        {
            if (!nameIndex.Contains(cityIndex))
            {
                cityCount++;
            }
            nameIndex.Add(cityIndex);
            foreach (var s in city.Tags)
            {
                int index;
                City.tagDictionary.TryGetValue(s, out index);
                if (!tag.Contains(index))
                {
                    tag.Add(index);
                }
            }
        }

        public int CalcIValue(int[] rate)
        {
            return tag.Sum(t => rate[t]);
        }
    }

    public class Agency
    {
        private const string FilePath = "E:\\Data";
        private const int BufSize = 500000;
        private const double Alaph = 3;
        private const double Beta = 1;
        private const double Eta = 5;

        public void PrepareData(AdjacencyGraph map)
        {
            foreach (var city in map.VertexList)
            {
                for (var i = 1; i <= 5; i++)
                {
                    EnumeratePath(map, city, i);
                }
            }
        }

        public void EnumeratePath(AdjacencyGraph map, City start, int count)
        {
            var result = new List<Path>();
            var s = new Stack<SearchNode>();
            var node = new SearchNode {parent = null, self = start, depth = 0};
            var bufCount = 0;
            s.Push(node);
            while (s.Count != 0)
            {
                var top = s.Pop();
                if (top.self == start && top.parent != null)
                {
                    var n = top.parent;
                    var p = new Path();
                    while (n.parent != null)
                    {
                        p.AddCity(n.self, map.GetCityIndex(n.self));
                        p.transit += (n.cost + n.self.TransitFees);
                        n = n.parent;
                    }
                    p.transit -= 2*start.TransitFees;
                    result.Add(p);
                    bufCount++;
                }
                if (top.depth < count)
                {
                    foreach (var next in top.self.NeighborList.Select(
                        e => new SearchNode 
                        {self = e.End, parent = top, depth = top.depth + 1, cost = e.Value}))
                    {
                        s.Push(next);
                    }
                }
                else if (top.depth == count)
                {
                    var back = start.GetEdge(top.self);
                    if (back != null)
                    {
                        var next = new SearchNode
                        {self = start, parent = top, depth = top.depth + 1, cost = back.Value};
                        s.Push(next);
                    }
                }
                if (bufCount < BufSize) continue;
                FileIo.ExportPathData(FilePath + "\\" + start.Name + "\\", result, count);
                result.Clear();
                bufCount = 0;
            }
            FileIo.ExportPathData(FilePath + "\\" + start.Name + "\\", result, count);
        }

        public Path BestPath(City city, int[] rate, int targetCityCount, int targetTransitFee)
        {
            FileInfo fileInfo;
            var i = 0;
            do
            {
                i++;
                fileInfo = new FileInfo(FilePath + "\\" + city.Name + "\\" + i + ".path");
            } while (fileInfo.Exists);
            var tagSum = rate.Sum();
            var betterList = new BlockingCollection<Path>();
            for (var tim = 0; tim*8 < i; tim++)
            {
                var end = 8 + 8*tim < i ? 8 + 8*tim : i;
                Parallel.For(1 + 8*tim, end, index =>
                {
                    var path = FilePath + "\\" + city.Name + "\\" + index.ToString() + ".path";
                    var pathList = FileIo.ImportPathData(path);
                    Parallel.ForEach(pathList, p =>
                    {
                        p.lvaule = Alaph*p.CalcIValue(rate)/tagSum
                                   + Beta*CalcSValue(p, targetCityCount)
                                   + Eta*CalcTValue(p, targetTransitFee);
                    });
                    betterList.Add(MaxLValuePath(pathList));
                });
            }
            return MaxLValuePath(betterList);
        }

        private static Path MaxLValuePath(IEnumerable<Path> pathList)
        {
            Path bestPath = null;
            double[] max = {double.NegativeInfinity};
            foreach (var p in pathList.Where(p => p.lvaule > max[0]))
            {
                max[0] = p.lvaule;
                bestPath = p;
            }
            return bestPath;
        }

        private static Path MaxLValuePath(List<Path> pathList)
        {
            Path bestPath = null;
            double[] max = {double.NegativeInfinity};
            foreach (var p in pathList.Where(p => p.lvaule > max[0]))
            {
                max[0] = p.lvaule;
                bestPath = p;
            }
            return bestPath;
        }

        private static double CalcSValue(Path real, int target)
        {
            return -((Math.Abs((double) real.CityCount - target))/target);
        }

        private static double CalcTValue(Path real, int target)
        {
            var T = ((double) target - real.transit)/target;
            if (target >= real.transit)
            {
                return T;
            }
            return 3*T;
        }

        private class SearchNode
        {
            public int cost;
            public int depth;
            public SearchNode parent;
            public City self;
        }
    }
}