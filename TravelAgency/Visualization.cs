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

        public event OnVertexClickedEventHandler OnVertexClickedEvent;

        public Visualization(Canvas canva,ref AdjacencyGraph map)
        {
            this.canva = canva;
            this.map = map;
            dictionary = new Dictionary<Ellipse, City>();
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

        void ellipse_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;
            City city;
            dictionary.TryGetValue(ellipse, out city);
            if (ellipse.IsMouseDirectlyOver)
            {
                HighLightVertexAndShortEdge(city, true);
            } 
            else
            {
                HighLightVertexAndShortEdge(city, false);
            }
        }

        void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            City city;
            dictionary.TryGetValue(sender as Ellipse, out city);
            if (city != null)
            {
                OnVertexClickedEvent(city);
            }
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
            canva.Children.Add(ellipse);
            ellipse.MouseDown += ellipse_MouseDown;
            ContextMenu contextMenu = new ContextMenu();
            MenuItem editCity = new MenuItem { Header = "编辑城市" };
            editCity.Click += editCity_Click;
            contextMenu.Items.Add(editCity);
            MenuItem deleteCity = new MenuItem { Header = "删除城市" };
            deleteCity.Click += deleteCity_Click;
            contextMenu.Items.Add(deleteCity);
            ellipse.ContextMenu = contextMenu;
            ellipse.IsMouseDirectlyOverChanged += ellipse_IsMouseDirectlyOverChanged;
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

        private void HighLightVertexAndShortEdge(City city,bool isHighLight)
        {
            if (isHighLight)
            {
                city.ellipse.Fill = Brushes.YellowGreen;
                foreach (City end in map.VertexList)
                {
                    List<Edge> result = map.ShortestPath(city, end);
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
                    }
                }
            }
            else
            {
                city.ellipse.Fill = Brushes.Yellow;
                foreach (City end in map.VertexList)
                {
                    List<Edge> result = map.ShortestPath(city, end);
                    if (result == null)
                    {
                        return;
                    }
                    foreach (Edge edge in result)
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
}
