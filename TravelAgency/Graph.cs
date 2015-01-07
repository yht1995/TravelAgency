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
    public class Edge
    {
        private int value;
        private City start;

        public City Start
        {
            get { return start; }
            set { start = value; }
        }
        private City end;

        public City End
        {
            get { return end; }
            set { end = value; }
        }

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Edge(City start, City end, int edge)
        {
            this.start = start;
            this.end = end;
            this.value = edge;
        }

        public double GetStartX()
        {
            return start.GetCenterX();
        }

        public double GetStartY()
        {
            return start.GetCenterY();
        }

        public double GetEndX()
        {
            return end.GetCenterX();
        }

        public double GetEndY()
        {
            return end.GetCenterY();
        }
        [NonSerialized]
        public Line line;
    }

    [Serializable]
    public class AdjacencyGraph
    {
        private List<City> vertexList;
        private Dictionary<string, City> dictionary;
        private int[,] adjacencyMartix;
        private int[,] pathMartix;

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
            this.dictionary = new Dictionary<string, City>();
        }

        public void AddVertex(City vertex)
        {
            this.vertexList.Add(vertex);
            dictionary.Add(vertex.Name, vertex);
        }

        public void RemoveVertex(City vertex)
        {
            foreach(Edge edge in vertex.NeighborList)
            {
                edge.End.RemoveEdge(vertex);
            }
            this.VertexList.Remove(vertex);
            dictionary.Remove(vertex.Name);
        }

        public void AddEdge(City start, City end, int edge)
        {
            start.AddEdge(end, edge);
            end.AddEdge(start, edge);
        }

        public void AddEdge(int start, int end, int edge)
        {
            City cstart, cend;
            cstart = vertexList[start];
            cend = vertexList[end];
            AddEdge(cstart, cend, edge);
        }

        public void AddEdge(string start, string end, int edge)
        {
            City cstart, cend;
            dictionary.TryGetValue(start,out cstart);
            dictionary.TryGetValue(end, out cend);
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
                int edge = start.GetEdge(end);
                return edge;
            }
            catch (System.Exception )
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
        }

        private void UpdataAdjacencyMartix()
        {
            adjacencyMartix = new int[vertexList.Count,vertexList.Count];
            pathMartix = new int[vertexList.Count, vertexList.Count];
            for (int i = 0; i < VertexList.Count;i++ )
            {
                for (int j = 0; j < VertexList.Count;j++ )
                {
                    pathMartix[i, j] = i;
                    if (i==j)
                    {
                        adjacencyMartix[i, j] = 0;
                    }
                    else
                    {
                        adjacencyMartix[i, j] = Int32.MaxValue;
                    }
                }
            }
            foreach (City city in vertexList)
            {
                foreach (Edge edge in city.NeighborList)
                {
                    int indesStart = vertexList.IndexOf(edge.Start);
                    int indexEnd = vertexList.IndexOf(edge.End);
                    adjacencyMartix[indesStart, indexEnd] = edge.Value;
                }
            }
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
                        if (adjacencyMartix[i,k] + adjacencyMartix[k,j] < adjacencyMartix[i,j])
                        {
                            adjacencyMartix[i, j] = adjacencyMartix[i, k] + adjacencyMartix[k, j];
                            pathMartix[i,j] = pathMartix[k,j];
                        }
                    }
                }
            }
        }
    }
}
