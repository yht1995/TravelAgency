using System;
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
    public interface IVertexVisualization
    {
        double GetCenterX();
        double GetCenterY();
        double GetXmin();
        double GetXmax();
        double GetYmin();
        double GetYmax();
        Ellipse Ellipse
        {
            get;
            set;
        }
    }

    public class Visualization<TVertex,TEdge>
        where TVertex:IVertexVisualization
        where TEdge : IEquatable<TEdge>, new()
    {
        private Canvas canva;
        private double Xoffset;
        private double Yoffset;
        private double scale;

        public Canvas Canva
        {
            set { canva = value; }
        }

        public Visualization(Canvas canva)
        {
            this.canva = canva;
        }

        public void DrawGraph(AdjacencyGraph<TVertex,TEdge> graph)
        {
            double Width = canva.Width/(graph.VertexList[0].GetXmax() - graph.VertexList[0].GetXmin());
            double Height = canva.Height / (graph.VertexList[0].GetYmax() - graph.VertexList[0].GetYmin());
            scale = Math.Min(Width,Height);
            Xoffset = graph.VertexList[0].GetXmin();
            Yoffset = graph.VertexList[0].GetYmin();
            foreach (TVertex vertex in graph.VertexList)
            {
                foreach (Edge<TVertex, TEdge> edge in graph.GetEdgesofVertex(vertex))
                {
                    DrawEdge(edge);
                }
            }
            foreach (TVertex vertex in graph.VertexList)
            {
                DrawVertex(vertex);
            }
        }

        public void DrawVertex(TVertex vertex)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = mySolidColorBrush;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 20;
            ellipse.Width = 20;
            Canvas.SetLeft(ellipse, (vertex.GetCenterX() - Xoffset) * scale - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, (vertex.GetCenterY() - Yoffset) * scale - ellipse.Height / 2);
            vertex.Ellipse = ellipse;
            canva.Children.Add(ellipse);
        }

        public void DrawEdge(Edge<TVertex, TEdge> edge)
        {
            Line line = new Line();
            line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            line.X1 = (edge.GetStartX() - Xoffset) * scale;
            line.X2 = (edge.GetEndX() - Xoffset) * scale;
            line.Y1 = canva.Height - ((edge.GetStartY() - Yoffset) * scale);
            line.Y2 = canva.Height - ((edge.GetEndY() - Yoffset) * scale);
            line.VerticalAlignment = VerticalAlignment.Bottom;
            line.HorizontalAlignment = HorizontalAlignment.Left;
            edge.Line = line;
            this.canva.Children.Add(line); 
        }
    }
}
