using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Shapes;

namespace TravelAgency
{
    [Serializable]
    public class AdjacencyGraph
    {
        private List<City> vertexList;
        private Dictionary<string, int> dictionary;
        private int[,] adjacencyMartix;
        private int[,] pathMartix;
        private int[,] costMartix;

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

        public AdjacencyGraph()
        {
            this.vertexList = new List<City>();
            this.dictionary = new Dictionary<string, int>();
        }
        public void AddVertex(City vertex)
        {
            if (!VertexList.Contains(vertex))
            {
                VertexList.Add(vertex);
                dictionary.Add(vertex.Name, VertexList.Count - 1);
            }
        }
        public void RemoveVertex(City vertex)
        {
            foreach(Edge edge in vertex.NeighborList)
            {
                edge.End.RemoveEdge(vertex);
            }
            this.VertexList.Remove(vertex);
            Dictionary.Remove(vertex.Name);
        }
        public void AddEdge(City start, City end, int edge)
        {
            if (start.GetEdge(end)==null)
            {
                start.AddEdge(end, edge);
            }
            if (end.GetEdge(start)==null)
            {
                end.AddEdge(start, edge);
            }
        }
        public void AddEdge(int start, int end, int edge)
        {
            City cstart, cend;
            cstart = vertexList[start];
            cend = vertexList[end];
            AddEdge(cstart, cend, edge);
        }
        public void RemoveEdge(City start, City end)
        {
            start.RemoveEdge(end);
            end.RemoveEdge(start);
        }
        public int GetEdge(City start,City end)
        {
            try
            {
                int edge = start.GetEdge(end).Value;
                return edge;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }
        public List<Edge> GeintsofVertex(City vertex)
        {
            return vertex.NeighborList;
        }
        public void Clear()
        {
            this.VertexList.Clear();
            this.Dictionary.Clear();
        }
        public int GetCityIndex(City city)
        {
            return dictionary[city.Name];
        }
        public City FindCitybyName(string name)
        {
            return VertexList.Find(delegate(City a)
            {
                return a.Name == name;
            });
        }
        public void UpdataAdjacencyMartix()
        {
            UpdateDictionary();
            adjacencyMartix = new int[vertexList.Count,vertexList.Count];
            pathMartix = new int[vertexList.Count, vertexList.Count];
            for (int i = 0; i < VertexList.Count;i++ )
            {
                for (int j = 0; j < VertexList.Count;j++ )
                {
                    pathMartix[i, j] = i;
                    adjacencyMartix[i, j] = 1000000;
                }
            }
            foreach (City city in vertexList)
            {
                foreach (Edge edge in city.NeighborList)
                {
                    int indexStart = vertexList.IndexOf(edge.Start);
                    int indexEnd = vertexList.IndexOf(edge.End);
                    adjacencyMartix[indexStart, indexEnd] = edge.Value;
                }
            }
            this.costMartix = (int[,])adjacencyMartix.Clone();
            Array.Copy(adjacencyMartix, costMartix, adjacencyMartix.Length);
        }
        public void Floyd()
        {
            UpdataAdjacencyMartix();
            for (int k = 0; k < vertexList.Count; k++)
            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    for (int j = 0; j < vertexList.Count; j++)
                    {
                        if (costMartix[i,k] + costMartix[k,j] < costMartix[i,j])
                        {
                            costMartix[i, j] = costMartix[i, k] + costMartix[k, j];
                            pathMartix[i,j] = pathMartix[k,j];
                        }
                    }
                }
            }
        }
        public List<Edge> ShortestPath(City start, City end)
        {
            if (start == end)
            {
                return null;
            }
            if (VertexList.Count <2)
            {
                return null;
            }
            Stack<City> s = new Stack<City>();
            int indexStart,indexEnd;
            indexStart = dictionary[start.Name];
            do 
            {
                indexEnd = dictionary[end.Name];
                if (costMartix[indexStart, indexEnd] == 1000000)
                {
                    return null;
                }
                s.Push(end);
                end = vertexList[pathMartix[indexStart,indexEnd]];
            } while (pathMartix[indexStart,indexEnd] != indexStart);
            List<Edge> result = new List<Edge>();
            while (s.Count != 0)
            {
                Edge edge = start.GetEdge(s.Pop());
                result.Add(edge);
                start = edge.End;
            }
            return result;
        }
        public void UpdateMinMax()
        {
            City.longitudeMin = Double.PositiveInfinity;
            City.longitudeMax = Double.NegativeInfinity;
            City.latitudeMin = Double.PositiveInfinity;
            City.latitudeMax = Double.NegativeInfinity;
            foreach (City c in VertexList)
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
        public void UpdateDictionary()
        {
            dictionary.Clear();
            for (int i =0;i<vertexList.Count;i++)
            {
                City city = vertexList[i];
                dictionary.Add(city.Name, i);
            }
        }
    }
}
