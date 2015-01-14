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
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace TravelAgency
{
    public partial class MainWindow : Window
    {
        private AdjacencyGraph map = new AdjacencyGraph();
        private Visualization visual;
        private ObservableCollection<ShortPath> shortPath;
        public MainWindow()
        {
            InitializeComponent();
            visual = new Visualization(this.canva,ref map);
            visual.OnVertexClickedEvent += visual_OnVertexClickedEvent;
            this.shortPath = new ObservableCollection<ShortPath>();
            shortList.ItemsSource = this.shortPath;
        }
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (FileIO.ImportMap(map))
            {
                visual.DrawGraph(map);
            }
        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            FileIO.ExportMap(map);
        }
        private void AddCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Point geoLoc = visual.GetGeoLocaltion(Mouse.GetPosition(canva));
                if (geoLoc.X == Double.PositiveInfinity)
                {
                    geoLoc.X = 25;
                    geoLoc.Y = 40;
                }
                City city = new City("城市名称", LongitudeClass.ToString(geoLoc.X), LatitudeClass.ToString(geoLoc.Y), "100");
                CityEdit cityEdit = new CityEdit(city,ref map);
                cityEdit.Title = "添加城市";
                cityEdit.ShowDialog();
                visual.DrawGraph(map);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void visual_OnVertexClickedEvent(City city)
        {
            this.cityName.Text = city.Name.ToString();
            this.latitude.Text = LatitudeClass.ToString(city.Latitude);
            this.longitude.Text = LongitudeClass.ToString(city.Longitude);
            this.transitFee.Text = city.TransitFees.ToString();
            this.tagList.Items.Clear();
            foreach(string tag in city.Tags)
            {
                this.tagList.Items.Add(tag);
            }
            this.shortPath.Clear();
            foreach (City end in map.VertexList)
            {
                List<Edge> path =  map.ShortestPath(city, end);
                if (path != null)
                {
                    shortPath.Add(new ShortPath(city, end, path));
                }
            }
        }
        private void shortList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.visual.ClearHighLight();
            ShortPath s = this.shortList.SelectedValue as ShortPath;
            if (s != null)
            {
                City start = map.FindCitybyName(s.Start);
                City end = map.FindCitybyName(s.End);
                this.visual.HighLightShortEdge(start, end);
            }
        }
    }

    /// <summary>
    /// 最短路径类，用于填充界面的ListView
    /// </summary>
    class ShortPath
    {
        public string Start { get; private set; }
        public string End { get; private set; }
        public string Path { get; private set; }
        public int Fee { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Start">起始城市</param>
        /// <param name="End">目标城市</param>
        /// <param name="path">路径列表</param>
        public ShortPath(City Start, City End, List<Edge> path)
        {
            this.Start = Start.Name;
            this.End = End.Name;
            this.Path += Start.Name;
            this.Fee = 0;
            foreach (Edge e in path)
            {
                this.Path += "->" + e.End.Name;
                Fee += e.Value + e.End.TransitFees;
            }
            this.Fee -= End.TransitFees;
        }
    }

    public class Plan
    {
        public String Name { get; set; }
        public String Begin { get; set; }
        public String ExpCityNum { get; set; }
        public String RealCityNum { get; set; }
        public String Path { get; set; }
        public String ExpTotal { get; set; }
        public String RealTotal { get; set; }
        public String Value { get; set; }
        public String ExpTagList { get; set; }
        public String RealTagList { get; set; }

        public Plan(ACO tsp,Guide guide, Request theReq)
        {
            this.RealTagList = "";
            this.ExpTagList = "";
            this.Path = "";
            int ii = 0;
            this.Name = theReq.name;
            this.Begin = guide.CityList[theReq.start].Name;
            this.ExpCityNum = theReq.cityNum.ToString();
            this.ExpTotal = theReq.total.ToString();
            for (ii = 0; ii < theReq.tagList.Count; ii++)
            {
                this.ExpTagList += theReq.tagList[ii] + "_" + theReq.rateList[ii].ToString() + "、";
            }
            if (this.ExpTagList.Length >= 2)
            {
                this.ExpTagList.Substring(0, this.ExpTagList.Length - 2);
            }
            this.RealCityNum = tsp.m_cBestAnt.m_nRealMovedCount.ToString();
            this.RealTotal = tsp.m_cBestAnt.m_dbCost.ToString();
            this.Value = tsp.m_cBestAnt.estimateValue.ToString();
            for (ii = 0; ii < tsp.m_cBestAnt.tagList.Count; ii++)
            {
                this.RealTagList += tsp.m_cBestAnt.tagList[ii] + "、";
            }
            if (this.RealTagList.Length >= 2)
            {
                this.RealTagList.Substring(0, this.RealTagList.Length - 2);
            }
            for (ii = 0; ii < tsp.m_cBestAnt.m_nMovedCityCount; ii++)
            {
                this.Path += guide.CityList[tsp.m_cBestAnt.m_nPath[ii]].Name + "->";
            }
            this.Path = this.Path.Substring(0, this.Path.Length - 2);
        }
    }
}
