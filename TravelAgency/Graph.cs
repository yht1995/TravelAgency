﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace TravelAgency
{
    public class Edge<TVertex, TEdge>
        where TVertex:IVertexVisualization
    {
        private TEdge value;
        private TVertex start;
        private TVertex end;

        public TEdge Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Edge(TVertex start,TVertex end,TEdge edge)
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

        private System.Windows.Shapes.Line line;

        public System.Windows.Shapes.Line Line
        {
            get { return line; }
            set { line = value; }
        }
    }

    [Serializable]
    public class AdjacencyGraph<TVertex, TEdge>
        where  TVertex : IVertexVisualization
        where  TEdge : IEquatable<TEdge>, new()
    {
        public List<TVertex> vertexList;
        public List<TVertex> VertexList
        {
            get { return vertexList; }
            set { vertexList = value; }
        }
        public List<List<TEdge>> adjacencyMartix;
        public List<List<TEdge>> AdjacencyMartix
        {
            get { return adjacencyMartix; }
            set { adjacencyMartix = value; }
        }

        public AdjacencyGraph()
        {
            this.vertexList = new List<TVertex>();
            this.adjacencyMartix = new List<List<TEdge>>();
        }

        public void AddVertex(TVertex vertex)
        {
            this.vertexList.Add(vertex);
            foreach (List<TEdge> line in this.adjacencyMartix)
            {
                line.Add(new TEdge());
            }
            List<TEdge> newline = new List<TEdge>();
            for (int i = 0; i < this.vertexList.Count;i++ )
            {
                newline.Add(new TEdge());
            }
            this.adjacencyMartix.Add(newline);
        }

        public void RemoveVertex(TVertex vertex)
        {
            int index = this.vertexList.IndexOf(vertex);
            foreach (List<TEdge> line in this.adjacencyMartix)
            {
                line.RemoveAt(index);
            }
            this.adjacencyMartix.RemoveAt(index);
            this.vertexList.RemoveAt(index);
        }

        public void AddEdge(TVertex start, TVertex end, TEdge edge)
        {
            int indexStart = this.vertexList.IndexOf(start);
            int indexEnd = this.vertexList.IndexOf(end);
            this.adjacencyMartix[indexStart][indexEnd] = edge;
            this.adjacencyMartix[indexEnd][indexStart] = edge;
        }

        public void AddEdge(int indexStart, int indexEnd, TEdge edge)
        {
            this.adjacencyMartix[indexStart][indexEnd] = edge;
            this.adjacencyMartix[indexEnd][indexStart] = edge;
        }

        public void RemoveEdge(TVertex start, TVertex end)
        {
            int indexStart = this.vertexList.IndexOf(start);
            int indexEnd = this.vertexList.IndexOf(end);
            this.adjacencyMartix[indexStart][indexEnd] = default(TEdge);
            this.adjacencyMartix[indexEnd][indexStart] = default(TEdge);
        }

        public List<Edge<TVertex, TEdge>> GetEdgesofVertex(TVertex vertex)
        {
            List<Edge<TVertex, TEdge>> result = new List<Edge<TVertex, TEdge>>();
            int index = this.vertexList.IndexOf(vertex);
            for (int i = 0; i < adjacencyMartix[index].Count; i++)
            {
                if (!adjacencyMartix[index][i].Equals(default(TEdge)))
                {
                    result.Add(new Edge<TVertex, TEdge>(vertex, vertexList[i], adjacencyMartix[index][i]));
                }
            }
            return result;
        }

        public void Clear()
        {
            this.VertexList.Clear();
            this.adjacencyMartix.Clear();
        }
    }
}