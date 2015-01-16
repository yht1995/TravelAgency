using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TravelAgency.Graph;

namespace TravelAgency
{
    public delegate void OnVertexClickedEventHandler(City e);

    /// <summary>
    ///     可视化类，用于绘制地图
    /// </summary>
    public class Visualization
    {
        private readonly Canvas canva;
        private readonly Dictionary<Ellipse, City> dictionary;
        private readonly List<Ellipse> highLightedCity;
        private readonly List<Line> highLightedEdge;
        private AdjacencyGraph map;
        private double scale;
        private double xoffset;
        private double yoffset;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="canva">画布</param>
        /// <param name="map">地图</param>
        public Visualization(Canvas canva, AdjacencyGraph map)
        {
            this.canva = canva;
            this.map = map;
            dictionary = new Dictionary<Ellipse, City>();
            highLightedCity = new List<Ellipse>();
            highLightedEdge = new List<Line>();
        }

        public event OnVertexClickedEventHandler OnVertexClickedEvent;

        /// <summary>
        ///     绘制地图
        /// </summary>
        /// <param name="graph">地图数据</param>
        public void DrawGraph(AdjacencyGraph graph)
        {
            map.UpdateMinMax();
            canva.Children.Clear();
            dictionary.Clear();
            var width = canva.Width/(City.GetXmax() - City.GetXmin());
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (width == 0 || double.IsPositiveInfinity(width))
            {
                width = 20;
            }
            var height = canva.Height/(City.GetYmax() - City.GetYmin());
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (height == 0 || double.IsPositiveInfinity(width))
            {
                height = 20;
            }
            scale = Math.Min(width, height);
            xoffset = (City.GetXmax() + City.GetXmin())/2 - canva.Width/scale/2;
            yoffset = (City.GetYmax() + City.GetYmin())/2 - canva.Height/scale/2;
            foreach (var edge in graph.VertexList.SelectMany(graph.EdgesofVertex))
            {
                edge.line = DrawEdge(edge);
            }
            foreach (var vertex in graph.VertexList)
            {
                vertex.ellipse = DrawVertex(vertex);
                dictionary.Add(vertex.ellipse, vertex);
            }
            map.Floyd();
        }

        /// <summary>
        ///     换算地理坐标
        /// </summary>
        /// <param name="canvaPos">画布坐标</param>
        /// <returns>地理坐标</returns>
        public Point GetGeoLocaltion(Point canvaPos)
        {
            return new Point(canvaPos.X/scale + xoffset, (canva.Height - canvaPos.Y)/scale + yoffset);
        }

        /// <summary>
        ///     高亮节点
        /// </summary>
        /// <param name="city">要高亮的节点</param>
        public void HighLightVertex(City city)
        {
            city.ellipse.Fill = Brushes.YellowGreen;
            highLightedCity.Add(city.ellipse);
        }

        /// <summary>
        ///     高亮最短路径
        /// </summary>
        /// <param name="start">起始节点</param>
        /// <param name="end">终止节点</param>
        public void HighLightShortEdge(City start, City end)
        {
            var result = map.ShortestPath(start, end);
            if (result == null)
            {
                return;
            }
            for (var i = 0; i < result.Count; i++)
            {
                if (i == 0)
                {
                    Panel.SetZIndex(result[i].line, 1);
                    result[i].line.StrokeThickness = 3;
                    result[i].line.Stroke = Brushes.DarkBlue;
                }
                else
                {
                    Panel.SetZIndex(result[i].line, 1);
                    result[i].line.StrokeThickness = 3;
                    result[i].line.Stroke = Brushes.OrangeRed;
                }
                highLightedEdge.Add(result[i].line);
            }
        }

        /// <summary>
        ///     撤销高亮
        /// </summary>
        public void ClearHighLight()
        {
            foreach (var e in highLightedCity)
            {
                e.Fill = Brushes.Yellow;
            }
            foreach (var l in highLightedEdge)
            {
                Panel.SetZIndex(l, 0);
                l.StrokeThickness = 0.5;
                l.Stroke = Brushes.LightSteelBlue;
            }
            highLightedCity.Clear();
            highLightedEdge.Clear();
        }

        /// <summary>
        ///     绘制节点
        /// </summary>
        /// <param name="vertex">节点</param>
        /// <returns>绘制的图像元素</returns>
        private Ellipse DrawVertex(City vertex)
        {
            var ellipse = new Ellipse
            {
                Fill = Brushes.Yellow,
                StrokeThickness = 2,
                Stroke = Brushes.Black,
                Height = 0.6*scale,
                Width = 0.6*scale
            };
            Panel.SetZIndex(ellipse, 2);
            Canvas.SetLeft(ellipse, (vertex.GetCenterX() - xoffset)*scale - ellipse.Width/2);
            Canvas.SetBottom(ellipse, (vertex.GetCenterY() - yoffset)*scale - ellipse.Height/2);
            ellipse.MouseDown += ellipse_MouseDown;
            var contextMenu = new ContextMenu();
            var editCity = new MenuItem {Header = "编辑城市"};
            editCity.Click += editCity_Click;
            contextMenu.Items.Add(editCity);
            var deleteCity = new MenuItem {Header = "删除城市"};
            deleteCity.Click += deleteCity_Click;
            contextMenu.Items.Add(deleteCity);
            ellipse.ContextMenu = contextMenu;
            canva.Children.Add(ellipse);

            var text = new TextBlock {Text = vertex.Name, FontSize = scale/2};
            Panel.SetZIndex(text, 2);
            Canvas.SetLeft(text, (vertex.GetCenterX() - xoffset)*scale + ellipse.Width/4);
            Canvas.SetBottom(text, (vertex.GetCenterY() - yoffset)*scale + ellipse.Height/4);
            canva.Children.Add(text);
            return ellipse;
        }

        /// <summary>
        ///     绘制边
        /// </summary>
        /// <param name="edge">边</param>
        /// <returns>绘制的图形元素</returns>
        private Line DrawEdge(Edge edge)
        {
            var line = new Line
            {
                StrokeThickness = 0.5,
                Stroke = Brushes.LightSteelBlue,
                X1 = (edge.GetStartX() - xoffset)*scale,
                X2 = (edge.GetEndX() - xoffset)*scale,
                Y1 = canva.Height - ((edge.GetStartY() - yoffset)*scale),
                Y2 = canva.Height - ((edge.GetEndY() - yoffset)*scale),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            canva.Children.Add(line);
            return line;
        }

        private void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            City city = null;
            if (sender != null) dictionary.TryGetValue((Ellipse) sender, out city);
            if (OnVertexClickedEvent != null) OnVertexClickedEvent(city);
            HighLightVertexAndShortEdge(city);
        }

        private void editCity_Click(object sender, RoutedEventArgs e)
        {
            var ellipse =
                (Ellipse) ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent((MenuItem) sender));
            City city;
            dictionary.TryGetValue(ellipse, out city);
            if (city == null) return;
            try
            {
                var cityEdit = new CityEdit(city, ref map) {Title = "编辑城市"};
                cityEdit.ShowDialog();
                DrawGraph(map);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteCity_Click(object sender, RoutedEventArgs e)
        {
            var ellipse =
                (Ellipse) ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent((MenuItem) sender));
            City city;
            dictionary.TryGetValue(ellipse, out city);
            try
            {
                map.RemoveVertex(city);
                DrawGraph(map);
            }
            catch (Exception)
            {
                MessageBox.Show("无法删除");
            }
        }

        /// <summary>
        ///     高亮节点和最近的路径
        /// </summary>
        /// <param name="city">要高亮的节点</param>
        private void HighLightVertexAndShortEdge(City city)
        {
            ClearHighLight();
            city.ellipse.Fill = Brushes.YellowGreen;
            highLightedCity.Add(city.ellipse);
            foreach (var end in map.VertexList)
            {
                HighLightShortEdge(city, end);
            }
        }
    }
}