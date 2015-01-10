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
    public class Plan
    {
        //方案类里面有游客姓名、起点、终点城市、历经城市数、路径、总费用、估价值以及标签。
        public String name { get; set; }
        public String begin { get; set; }
        public String expCityNum { get; set; }
        public String realCityNum { get; set; }
        public String path { get; set; }
        public String expTotal { get; set; }
        public String realTotal { get; set; }
        public String value { get; set; }
        public String expTagList { get; set; }
        public String realTagList { get; set; }

        public void VisualizeInfo(ref Plan plan, ref CTsp tsp, Request theReq, ref Guide guide)
        {
            int ii = 0;
            plan.name = theReq.name;
            plan.begin = guide.CityList[theReq.start].Name;
            plan.expCityNum = theReq.cityNum.ToString();
            plan.expTotal = theReq.total.ToString();
            for (ii = 0; ii < theReq.tagList.Count; ii++)
            {
                plan.expTagList += theReq.tagList[ii] + "_" + theReq.rateList[ii].ToString() + "、";
            }
            if (plan.expTagList.Length >= 2)
                plan.expTagList.Substring(0, plan.expTagList.Length - 2);
            plan.realCityNum = tsp.m_cBestAnt.m_nRealMovedCount.ToString();
            plan.realTotal = tsp.m_cBestAnt.m_dbCost.ToString();
            plan.value = tsp.m_cBestAnt.estimateValue.ToString();
            for (ii = 0; ii < tsp.m_cBestAnt.tagList.Count; ii++)
            {
                plan.realTagList += tsp.m_cBestAnt.tagList[ii] + "、";
            }
            if (plan.realTagList.Length >= 2)
                plan.realTagList.Substring(0, plan.realTagList.Length - 2);
            for (ii = 0; ii < tsp.m_cBestAnt.m_nMovedCityCount; ii++)
            {
                plan.path += guide.CityList[tsp.m_cBestAnt.m_nPath[ii]].Name + "->";
            }
            plan.path = plan.path.Substring(0, plan.path.Length - 2);
        }
    }
    public class Request
    {
        //请求【Request】:【起点】【终点】【城市数】【花费】【三大标签】
        public String name { get; set; }
        public int start { get; set; }
        public int cityNum { get; set; }
        public int total { get; set; }
        public List<String> tagList;
        public List<int> rateList;
    }
    public class Path
    {
        public List<int> NameIndex;
        public List<int> Tag;
        public int Transit;
        private int cityCount;
        public float Lvaule;
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
        private float alaph = 3;
        private float beta = 1;
        private float eta = 5;
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
            float max = float.NegativeInfinity;
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
            float max = float.NegativeInfinity;
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

        private float CalcSValue(Path real,int target)
        {
            return -((Math.Abs((float)real.CityCount - target)) / target);
        }

        private float CalcTValue(Path real, int target)
        {
            float T = ((float)target - real.Transit) / target;
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
