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

    /// <summary>
    /// 可视化类，用于绘制地图
    /// </summary>
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
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="canva">画布</param>
        /// <param name="map">地图</param>
        public Visualization(Canvas canva,AdjacencyGraph map)
        {
            this.canva = canva;
            this.map = map;
            dictionary = new Dictionary<Ellipse, City>();
            highLightedCity = new List<Ellipse>();
            highLightedEdge = new List<Line>();
        }
        /// <summary>
        /// 绘制地图
        /// </summary>
        /// <param name="graph">地图数据</param>
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
                foreach (Edge edge in graph.EdgesofVertex(vertex))
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
        /// <summary>
        /// 换算地理坐标
        /// </summary>
        /// <param name="canvaPos">画布坐标</param>
        /// <returns>地理坐标</returns>
        public Point GetGeoLocaltion(Point canvaPos)
        {
            return new Point(canvaPos.X/scale+Xoffset,(canva.Height-canvaPos.Y)/scale+Yoffset);
        }
        /// <summary>
        /// 高亮节点
        /// </summary>
        /// <param name="city">要高亮的节点</param>
        public void HighLightVertex(City city)
        {
            city.ellipse.Fill = Brushes.YellowGreen;
            this.highLightedCity.Add(city.ellipse);
        }
        /// <summary>
        /// 高亮最短路径
        /// </summary>
        /// <param name="start">起始节点</param>
        /// <param name="end">终止节点</param>
        public void HighLightShortEdge(City start, City end)
        {
            List<Edge> result = map.ShortestPath(start, end);
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
        /// <summary>
        /// 撤销高亮
        /// </summary>
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
        /// <summary>
        /// 绘制节点
        /// </summary>
        /// <param name="vertex">节点</param>
        /// <returns>绘制的图像元素</returns>
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
        /// <summary>
        /// 绘制边
        /// </summary>
        /// <param name="edge">边</param>
        /// <returns>绘制的图形元素</returns>
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
        private void ellipse_MouseDown(object sender, MouseEventArgs e)
        {
            City city;
            dictionary.TryGetValue(sender as Ellipse, out city);
            OnVertexClickedEvent(city);
            HighLightVertexAndShortEdge(city);
        }
        private void editCity_Click(object sender, RoutedEventArgs e)
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
        private void deleteCity_Click(object sender, RoutedEventArgs e)
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
        /// <summary>
        /// 高亮节点和最近的路径
        /// </summary>
        /// <param name="city">要高亮的节点</param>
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
    }
}
