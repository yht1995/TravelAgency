﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TravelAgency
{
    public delegate void OnVertexClickedEventHandler (City e);

    public class Visualization
    {
        private Canvas canva;
        private AdjacencyGraph map;
        private double Xoffset;
        private double Yoffset;
        private double scale;
        private Dictionary<Ellipse, City> dictionary;

        public event OnVertexClickedEventHandler OnVertexClickedEvent;

        public Visualization(Canvas canva,AdjacencyGraph map)
        {
            this.canva = canva;
            this.map = map;
            dictionary = new Dictionary<Ellipse, City>();
        }

        public void DrawGraph(AdjacencyGraph graph)
        {
            canva.Children.Clear();
            double Width = canva.Width / (City.GetXmax() - City.GetXmin());
            double Height = canva.Height / (City.GetYmax() - City.GetYmin());
            scale = Math.Min(Width,Height);
            Xoffset = City.GetXmin();
            Yoffset = City.GetYmin();
            foreach (City vertex in graph.VertexList)
            {
                foreach (Edge edge in graph.GeintsofVertex(vertex))
                {
                    edge.line = DrawEdge(edge);
                }
            }
            foreach (City vertex in graph.VertexList)
            {
                vertex.ellipse = DrawVertex(vertex);
                dictionary.Add(vertex.ellipse,vertex);
            }
            map.Floyd();
        }

        public Ellipse DrawVertex(City vertex)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Yellow;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 50;
            ellipse.Width = 50;
            Canvas.SetZIndex(ellipse, 2);
            Canvas.SetLeft(ellipse, (vertex.GetCenterX() - Xoffset) * scale - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, (vertex.GetCenterY() - Yoffset) * scale - ellipse.Height / 2);
            canva.Children.Add(ellipse);
            ellipse.MouseDown += ellipse_MouseDown;
            ellipse.IsMouseDirectlyOverChanged += ellipse_IsMouseDirectlyOverChanged;
            return ellipse;
        }

        public Line DrawEdge(Edge edge)
        {
            Line line = new Line();
            line.StrokeThickness = 0.5;
            line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line.X1 = (edge.GetStartX() - Xoffset) * scale;
            line.X2 = (edge.GetEndX() - Xoffset) * scale;
            line.Y1 = canva.Height - ((edge.GetStartY() - Yoffset) * scale);
            line.Y2 = canva.Height - ((edge.GetEndY() - Yoffset) * scale);
            line.VerticalAlignment = VerticalAlignment.Bottom;
            line.HorizontalAlignment = HorizontalAlignment.Left;
            this.canva.Children.Add(line);
            return line;
        }
        void ellipse_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            City city;
            dictionary.TryGetValue(ellipse, out city);
            if (ellipse.IsMouseDirectlyOver)
            {
                ellipse.Fill = Brushes.YellowGreen;
                if (city != null)
                {
                    foreach (City end in map.VertexList)
                    {
                        List<Edge> result = map.ShortestPath(city, end);
                        for (int i =0 ;i<result.Count;i++)
                        {
                            if (i == 0)
                            {
                                Canvas.SetZIndex(result[i].line, 1);
                                result[i].line.StrokeThickness = 4;
                                result[i].line.Stroke = System.Windows.Media.Brushes.Blue;
                            }
                            else
                            {
                                Canvas.SetZIndex(result[i].line, 1);
                                result[i].line.StrokeThickness = 4;
                                result[i].line.Stroke = System.Windows.Media.Brushes.DeepPink;
                            }
                        }
                    }
                }
            }
            else
            {
                ellipse.Fill = Brushes.Yellow;
                if (city != null)
                {
                    foreach (City end in map.VertexList)
                    {
                        foreach (Edge edge in map.ShortestPath(city, end))
                        {
                            if (edge != null)
                            {
                                Canvas.SetZIndex(edge.line, 0);
                                edge.line.StrokeThickness = 0.5;
                                edge.line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                            }
                            
                        }
                    }
                }
            }
        }

        void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            City city;
            dictionary.TryGetValue(sender as Ellipse, out city);
            if (city != null)
            {
                OnVertexClickedEvent(city);
                foreach (Edge edge in city.NeighborList)
                {
                    edge.line.Stroke = System.Windows.Media.Brushes.Pink;
                }
            }
        }

        public Point GetGeoLocaltion(Point canvaPos)
        {
            return new Point(canvaPos.X/scale+Xoffset,(canva.Height-canvaPos.Y)/scale+Yoffset);
        }
    }
}
