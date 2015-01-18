using System;
using System.Collections.Generic;

namespace TravelAgency.Graph
{
    /// <summary>
    ///     图类
    /// </summary>
    [Serializable]
    public class AdjacencyGraph
    {
        private int[,] adjacencyMartix;
        private int[,] costMartix;
        private Dictionary<string, int> dictionary;
        private int[,] pathMartix;

        /// <summary>
        ///     节点列表
        /// </summary>
        private List<City> vertexList;

        public AdjacencyGraph()
        {
            vertexList = new List<City>();
            dictionary = new Dictionary<string, int>();
        }

        public Dictionary<string, int> Dictionary
        {
            get { return dictionary; }
            set { dictionary = value; }
        }

        public int[,] AdjacencyMartix
        {
            get { return adjacencyMartix; }
            set { adjacencyMartix = value; }
        }

        public List<City> VertexList
        {
            get { return vertexList; }
            set { vertexList = value; }
        }

        public void AddVertex(City vertex)
        {
            if (VertexList.Contains(vertex)) return;
            VertexList.Add(vertex);
            dictionary.Add(vertex.Name, VertexList.Count - 1);
        }

        public void RemoveVertex(City vertex)
        {
            foreach (var edge in vertex.NeighborList)
            {
                edge.End.RemoveEdge(vertex);
            }
            VertexList.Remove(vertex);
            Dictionary.Remove(vertex.Name);
        }

        public void AddEdge(City start, City end, int edge)
        {
            if (start.GetEdge(end) == null)
            {
                start.AddEdge(end, edge);
            }
            if (end.GetEdge(start) == null)
            {
                end.AddEdge(start, edge);
            }
        }

        public void AddEdge(int start, int end, int edge)
        {
            var cstart = vertexList[start];
            var cend = vertexList[end];
            AddEdge(cstart, cend, edge);
        }

        public void RemoveEdge(City start, City end)
        {
            start.RemoveEdge(end);
            end.RemoveEdge(start);
        }

        public int GetEdge(City start, City end)
        {
            try
            {
                var edge = start.GetEdge(end).Value;
                return edge;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public List<Edge> EdgesofVertex(City vertex)
        {
            return vertex.NeighborList;
        }

        public void Clear()
        {
            VertexList.Clear();
            Dictionary.Clear();
        }

        public int GetCityIndex(City city)
        {
            int result;
            if (dictionary.TryGetValue(city.Name, out result))
            {
                return result;
            }
            return -1;
        }

        public City FindCitybyName(string name)
        {
            return VertexList.Find(a => a.Name == name);
        }

        /// <summary>
        ///     更新邻接矩阵
        /// </summary>
        public void UpdataAdjacencyMartix()
        {
            UpdateDictionary();
            adjacencyMartix = new int[vertexList.Count, vertexList.Count];
            for (var i = 0; i < VertexList.Count; i++)
            {
                for (var j = 0; j < VertexList.Count; j++)
                {
                    adjacencyMartix[i, j] = 1000000;
                }
            }
            foreach (var city in vertexList)
            {
                foreach (var edge in city.NeighborList)
                {
                    var indexStart = vertexList.IndexOf(edge.Start);
                    var indexEnd = vertexList.IndexOf(edge.End);
                    adjacencyMartix[indexStart, indexEnd] = edge.Value;
                }
            }
        }

        /// <summary>
        ///     执行Floyd算法
        /// </summary>
        public void Floyd()
        {
            UpdateDictionary();
            costMartix = new int[vertexList.Count, vertexList.Count];
            pathMartix = new int[vertexList.Count, vertexList.Count];
            for (var i = 0; i < VertexList.Count; i++)
            {
                for (var j = 0; j < VertexList.Count; j++)
                {
                    pathMartix[i, j] = i;
                    costMartix[i, j] = 1000000;
                }
            }
            foreach (var city in vertexList)
            {
                foreach (var edge in city.NeighborList)
                {
                    var indexStart = vertexList.IndexOf(edge.Start);
                    var indexEnd = vertexList.IndexOf(edge.End);
                    costMartix[indexStart, indexEnd] = edge.Value +
                                                       (vertexList[indexStart].TransitFees +
                                                        vertexList[indexEnd].TransitFees)/2;
                }
            }
            for (var k = 0; k < vertexList.Count; k++)
            {
                for (var i = 0; i < vertexList.Count; i++)
                {
                    for (var j = 0; j < vertexList.Count; j++)
                    {
                        if (costMartix[i, k] + costMartix[k, j] < costMartix[i, j])
                        {
                            costMartix[i, j] = costMartix[i, k] + costMartix[k, j];
                            pathMartix[i, j] = pathMartix[k, j];
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     获得最短路径
        /// </summary>
        /// <param name="start">出发城市</param>
        /// <param name="end">目标城市</param>
        /// <returns>路径列表</returns>
        public List<Edge> ShortestPath(City start, City end)
        {
            if (start == end)
            {
                return null;
            }
            if (VertexList.Count < 2)
            {
                return null;
            }
            var s = new Stack<City>();
            int indexEnd;
            var indexStart = dictionary[start.Name];
            do
            {
                indexEnd = dictionary[end.Name];
                if (costMartix[indexStart, indexEnd] == 1000000)
                {
                    return null;
                }
                s.Push(end);
                end = vertexList[pathMartix[indexStart, indexEnd]];
            } while (pathMartix[indexStart, indexEnd] != indexStart);
            var result = new List<Edge>();
            while (s.Count != 0)
            {
                var edge = start.GetEdge(s.Pop());
                result.Add(edge);
                start = edge.End;
            }
            return result;
        }

        /// <summary>
        ///     更新极值
        /// </summary>
        public void UpdateMinMax()
        {
            City.longitudeMin = Double.PositiveInfinity;
            City.longitudeMax = Double.NegativeInfinity;
            City.latitudeMin = Double.PositiveInfinity;
            City.latitudeMax = Double.NegativeInfinity;
            foreach (var c in VertexList)
            {
                if (c.Latitude > City.latitudeMax)
                {
                    City.latitudeMax = c.Latitude;
                }
                if (c.Latitude < City.latitudeMin)
                {
                    City.latitudeMin = c.Latitude;
                }
                if (c.Longitude > City.longitudeMax)
                {
                    City.longitudeMax = c.Longitude;
                }
                if (c.Longitude < City.longitudeMin)
                {
                    City.longitudeMin = c.Longitude;
                }
            }
        }

        /// <summary>
        ///     更新字典
        /// </summary>
        private void UpdateDictionary()
        {
            dictionary.Clear();
            for (var i = 0; i < vertexList.Count; i++)
            {
                var city = vertexList[i];
                dictionary.Add(city.Name, i);
            }
        }
    }
}