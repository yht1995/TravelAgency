using QuickGraph;
using QuickGraph.Algorithms;
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
    using Graph = UndirectedGraph<City, TaggedUndirectedEdge<City, int>>;
    public interface IVertexVisualization
    {
        double GetCenterX();
        double GetCenterY();

        Ellipse ellipse
        {
            get;
            set;
        }
    }

    public interface IEdgeVisualization
    {
        double GetStartX();
        double GetStartY();
        double GetEndX();
        double GetEndY();
        Line line
        {
            get;
            set;
        }
    }

    public class Visualization
    {
        private Canvas canva;

        public Canvas Canva
        {
            set { canva = value; }
        }

        public Visualization(Canvas canva)
        {
            this.canva = canva;
        }

        public void DrawMap(Graph map)
        {
            foreach (City city in map.Vertices)
            {
                DrawVertex(city);
            }
        }

        private void DrawVertex(City city)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = mySolidColorBrush;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 10;
            ellipse.Width = 10;
            Canvas.SetBottom(ellipse, city.Latitude.Value*10 + 900);
            Canvas.SetLeft(ellipse, city.Longitude.Value*10 + 1800);
            canva.Children.Add(ellipse);
        }
    }
}
