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
        public List<City> VertexList
        {
            get { return vertexList; }
            set { vertexList = value; }
        }
        //private List<List<int>> adjacencyMartix;
        //public List<List<int>> AdjacencyMartix
        //{
        //    get { return adjacencyMartix; }
        //    set { adjacencyMartix = value; }
        //}

        public AdjacencyGraph()
        {
            this.vertexList = new List<City>();
            this.dictionary = new Dictionary<string, City>();
            //this.adjacencyMartix = new List<List<int>>();
        }

        public void AddVertex(City vertex)
        {
            this.vertexList.Add(vertex);
            dictionary.Add(vertex.Name, vertex);
            //foreach (List<int> line in this.adjacencyMartix)
            //{
            //    line.Add(new int());
            //}
            //List<int> newline = new List<int>();
            //for (int i = 0; i < this.vertexList.Count;i++ )
            //{
            //    newline.Add(new int());
            //}
            //this.adjacencyMartix.Add(newline);
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
            //int indexStart = this.vertexList.IndexOf(start);
            //int indexEnd = this.vertexList.IndexOf(end);
            //this.adjacencyMartix[indexStart][indexEnd] = edge;
            //this.adjacencyMartix[indexEnd][indexStart] = edge;
        }

        public void AddEdge(int start, int end, int edge)
        {
            City cstart, cend;
            cstart = vertexList[start];
            cend = vertexList[end];
            AddEdge(cstart, cend, edge);
            //int indexStart = this.vertexList.IndexOf(start);
            //int indexEnd = this.vertexList.IndexOf(end);
            //this.adjacencyMartix[indexStart][indexEnd] = edge;
            //this.adjacencyMartix[indexEnd][indexStart] = edge;
        }

        public void AddEdge(string start, string end, int edge)
        {
            City cstart, cend;
            dictionary.TryGetValue(start,out cstart);
            dictionary.TryGetValue(end, out cend);
            AddEdge(cstart, cend, edge);
            //int indexStart = this.vertexList.IndexOf(start);
            //int indexEnd = this.vertexList.IndexOf(end);
            //this.adjacencyMartix[indexStart][indexEnd] = edge;
            //this.adjacencyMartix[indexEnd][indexStart] = edge;
        }

        //public void AddEdge(int indexStart, int indexEnd, int edge)
        //{
        //    this.adjacencyMartix[indexStart][indexEnd] = edge;
        //    this.adjacencyMartix[indexEnd][indexStart] = edge;
        //}

        public void RemoveEdge(City start, City end)
        {
            start.RemoveEdge(end);
            end.RemoveEdge(start);
            //int indexStart = this.vertexList.IndexOf(start);
            //int indexEnd = this.vertexList.IndexOf(end);
            //this.adjacencyMartix[indexStart][indexEnd] = default(int);
            //this.adjacencyMartix[indexEnd][indexStart] = default(int);
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
    }
}
