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
    public delegate void OnVertexClickedEventHandler<City> (City e);

    public class Visualization
    {
        private Canvas canva;
        private double Xoffset;
        private double Yoffset;
        private double scale;
        private Dictionary<Ellipse, City> dictionary;

        public event OnVertexClickedEventHandler<City> OnVertexClickedEvent;

        public Canvas Canva
        {
            set { canva = value; }
        }

        public Visualization(Canvas canva)
        {
            this.canva = canva;
            dictionary = new Dictionary<Ellipse, City>();
        }

        public void DrawGraph(AdjacencyGraph graph)
        {
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
        }

        public Ellipse DrawVertex(City vertex)
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
            City city;
            dictionary.TryGetValue(ellipse, out city);
            if (ellipse.IsMouseDirectlyOver)
            {
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 0, 0);
                ellipse.Fill = mySolidColorBrush;
                if (city != null)
                {
                    foreach (Edge edge in city.NeighborList)
                    {
                        edge.line.StrokeThickness = 4;
                        edge.line.Stroke = System.Windows.Media.Brushes.Yellow;
                    }
                }
            }
            else
            {
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                ellipse.Fill = mySolidColorBrush;
                if (city != null)
                {
                    foreach (Edge edge in city.NeighborList)
                    {
                        edge.line.StrokeThickness = 1;
                        edge.line.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
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

        public Line DrawEdge(Edge edge)
        {
            Line line = new Line();
            line.StrokeThickness = 1;
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
