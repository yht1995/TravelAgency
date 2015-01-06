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
    }

    public class Vertex<TVertex>
    {
        public TVertex vertex;
        public Ellipse ellipse;

        public Vertex (TVertex vertex,Ellipse ellipse)
        {
            this.vertex = vertex;
            this.ellipse = ellipse;
        }
    }

    public delegate void OnVertexClickedEventHandler<TVertex> (TVertex e);

    public class Visualization<TVertex,TEdge>
        where TVertex:IVertexVisualization
        where TEdge : IEquatable<TEdge>, new()
    {
        private Canvas canva;
        private double Xoffset;
        private double Yoffset;
        private double scale;
        private List<Vertex<TVertex>> vertexList = new List<Vertex<TVertex>>();
        private List<Edge<TVertex, TEdge>> edgeList = new List<Edge<TVertex, TEdge>>();

        public event OnVertexClickedEventHandler<TVertex> OnVertexClickedEvent;

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
                    edge.Line = DrawEdge(edge);
                    edgeList.Add(edge);
                }
            }
            foreach (TVertex vertex in graph.VertexList)
            {
                Ellipse e = DrawVertex(vertex);
                vertexList.Add(new Vertex<TVertex>(vertex,e));
            }
        }

        public Ellipse DrawVertex(TVertex vertex)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = mySolidColorBrush;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 40;
            ellipse.Width = 40;
            Canvas.SetLeft(ellipse, (vertex.GetCenterX() - Xoffset) * scale - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, (vertex.GetCenterY() - Yoffset) * scale - ellipse.Height / 2);
            canva.Children.Add(ellipse);
            ellipse.MouseDown += ellipse_MouseDown;
            ellipse.IsMouseDirectlyOverChanged += ellipse_IsMouseDirectlyOverChanged;
            return ellipse;
        }

        void ellipse_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            if (ellipse.IsMouseDirectlyOver)
            {
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 0, 0);
                ellipse.Fill = mySolidColorBrush;
            }
            else
            {
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                ellipse.Fill = mySolidColorBrush;
            }
        }

        void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Vertex<TVertex> vertex in vertexList)
            {
                if (vertex.ellipse == (Ellipse)sender)
                {
                    OnVertexClickedEvent(vertex.vertex);
                }
            }
        }



        public Line DrawEdge(Edge<TVertex, TEdge> edge)
        {
            Line line = new Line();
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
    }
}
