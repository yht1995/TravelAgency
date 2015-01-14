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
    public delegate void OnVertexClickedEventHandler (City e);

    public class Visualization
    {
        private Canvas canva;
        private AdjacencyGraph map;
        private double Xoffset;
        private double Yoffset;
        private double scale;
        private Dictionary<Ellipse, City> dictionary;
        private List<Ellipse> highLightedCity;
        private List<Line> highLightedEdge;

        public event OnVertexClickedEventHandler OnVertexClickedEvent;

        public Visualization(Canvas canva,ref AdjacencyGraph map)
        {
            this.canva = canva;
            this.map = map;
            dictionary = new Dictionary<Ellipse, City>();
            highLightedCity = new List<Ellipse>();
            highLightedEdge = new List<Line>();
        }
        public void DrawGraph(AdjacencyGraph graph)
        {
            map.UpdateMinMax();
            canva.Children.Clear();
            dictionary.Clear();
            double Width = canva.Width / (City.GetXmax() - City.GetXmin());
            if (Width == 0 || Width == Double.PositiveInfinity)
            {
                Width = 20;
            }
            double Height = canva.Height / (City.GetYmax() - City.GetYmin());
            if (Height == 0 || Width == Double.PositiveInfinity)
            {
                Height = 20;
            }
            scale = Math.Min(Width,Height);
            Xoffset = (City.GetXmax() + City.GetXmin()) / 2 - canva.Width / scale / 2;
            Yoffset = (City.GetYmax() + City.GetYmin()) / 2 - canva.Height / scale / 2;
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
        public Point GetGeoLocaltion(Point canvaPos)
        {
            return new Point(canvaPos.X/scale+Xoffset,(canva.Height-canvaPos.Y)/scale+Yoffset);
        }
        private Ellipse DrawVertex(City vertex)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Yellow;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Black;
            ellipse.Height = 0.6 * scale;
            ellipse.Width = 0.6 * scale;
            Canvas.SetZIndex(ellipse, 2);
            Canvas.SetLeft(ellipse, (vertex.GetCenterX() - Xoffset) * scale - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, (vertex.GetCenterY() - Yoffset) * scale - ellipse.Height / 2);
            ellipse.MouseDown += ellipse_MouseDown;
            ContextMenu contextMenu = new ContextMenu();
            MenuItem editCity = new MenuItem { Header = "编辑城市" };
            editCity.Click += editCity_Click;
            contextMenu.Items.Add(editCity);
            MenuItem deleteCity = new MenuItem { Header = "删除城市" };
            deleteCity.Click += deleteCity_Click;
            contextMenu.Items.Add(deleteCity);
            ellipse.ContextMenu = contextMenu;
            canva.Children.Add(ellipse);

            TextBlock text = new TextBlock();
            text.Text = vertex.Name;
            text.FontSize = scale/2;
            Canvas.SetZIndex(text, 2);
            Canvas.SetLeft(text, (vertex.GetCenterX() - Xoffset) * scale + ellipse.Width / 4);
            Canvas.SetBottom(text, (vertex.GetCenterY() - Yoffset) * scale + ellipse.Height / 4);
            canva.Children.Add(text);
            return ellipse;
        }
        private Line DrawEdge(Edge edge)
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
        void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            City city;
            dictionary.TryGetValue(sender as Ellipse, out city);
            OnVertexClickedEvent(city);
            HighLightVertexAndShortEdge(city);
        }
        void editCity_Click(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(sender as MenuItem)) as Ellipse;
            City city;
            dictionary.TryGetValue(ellipse, out city);
            if (city != null)
            {
                try
                {
                    CityEdit cityEdit = new CityEdit(city,ref map);
                    cityEdit.Title = "编辑城市";
                    cityEdit.ShowDialog();
                    DrawGraph(map);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        void deleteCity_Click(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(sender as MenuItem)) as Ellipse;
            City city;
            dictionary.TryGetValue(ellipse, out city);
            try
            {
                map.RemoveVertex(city);
                DrawGraph(map);
            }
            catch (System.Exception)
            {
                MessageBox.Show("无法删除");
            }
        }
        private void HighLightVertexAndShortEdge(City city)
        {
            ClearHighLight();
            city.ellipse.Fill = Brushes.YellowGreen;
            this.highLightedCity.Add(city.ellipse);
            foreach (City end in map.VertexList)
            {
                HighLightShortEdge(city, end);
            }
        }
        public void HighLightShortEdge(City Start, City end)
        {
            List<Edge> result = map.ShortestPath(Start, end);
            if (result == null)
            {
                return;
            }
            for (int i = 0; i < result.Count; i++)
            {
                if (i == 0)
                {
                    Canvas.SetZIndex(result[i].line, 1);
                    result[i].line.StrokeThickness = 3;
                    result[i].line.Stroke = System.Windows.Media.Brushes.DarkBlue;
                }
                else
                {
                    Canvas.SetZIndex(result[i].line, 1);
                    result[i].line.StrokeThickness = 3;
                    result[i].line.Stroke = System.Windows.Media.Brushes.OrangeRed;
                }
                highLightedEdge.Add(result[i].line);
            }
        }
        public void ClearHighLight()
        {
            foreach (Ellipse e in highLightedCity)
            {
                e.Fill = Brushes.Yellow;
            }
            foreach (Line l in highLightedEdge)
            {
                Canvas.SetZIndex(l, 0);
                l.StrokeThickness = 0.5;
                l.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
            }
            highLightedCity.Clear();
            highLightedEdge.Clear();
        }
    }
}
