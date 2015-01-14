using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TravelAgency
{
    public partial class MainWindow : Window
    {
        private AdjacencyGraph map = new AdjacencyGraph();
        private Visualization visual;
        private ObservableCollection<ShortPath> shortPath;
        private ObservableCollection<Plan> planList;
        private ObservableCollection<Tag> tagList;
        private City selectedCity = null;

        public MainWindow()
        {
            InitializeComponent();
            visual = new Visualization(this.canva,map);
            visual.OnVertexClickedEvent += visual_OnVertexClickedEvent;
            this.shortPath = new ObservableCollection<ShortPath>();
            this.shortList.ItemsSource = this.shortPath;
            this.planList = new ObservableCollection<Plan>();
            this.planListView.ItemsSource = this.planList;
            this.tagList = new ObservableCollection<Tag>();
            this.tagListView.ItemsSource = this.tagList;
            this.progressBar.Visibility = Visibility.Collapsed;
        }
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (FileIO.ImportMap(map))
            {
                visual.DrawGraph(map);
                tagList.Clear();
                foreach (string tag in City.tagList)
                {
                    tagList.Add(new Tag(tag, 0));
                }
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
            this.selectedCity = city;
            this.cityName.Text = city.Name.ToString();
            this.latitude.Text = LatitudeClass.ToString(city.Latitude);
            this.longitude.Text = LongitudeClass.ToString(city.Longitude);
            this.transitFee.Text = city.TransitFees.ToString();
            this.cityTagList.Items.Clear();
            foreach(string tag in city.Tags)
            {
                this.cityTagList.Items.Add(tag);
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
        private void search_Click(object sender, RoutedEventArgs e)
        {
            map.UpdataAdjacencyMartix();
            Guide guide = new Guide(map);
            List<Tag> tagList = new List<Tag>();
            foreach(Tag tag in this.tagList)
            {
                if (tag.rate!=0)
                {
                    tagList.Add(tag);
                }
            }
            try
            {
                #region throw
                if (tagList.Count == 0)
                {
                    throw (new Exception("请选择城市标签"));
                }
                if (Convert.ToInt32(expTotal.Text) <=0)
                {
                    throw (new Exception("出去玩是要花钱的"));
                }
                if (Convert.ToInt32(expCityNum.Text) <= 0 || Convert.ToInt32(expCityNum.Text) > 10)
                {
                    throw (new Exception("这么多城市玩的过来吗？"));
                }
                if (this.selectedCity == null)
                {
                    throw (new Exception("请在地图上选择出发城市"));
                }
                #endregion
                Request request = new Request(name.Text, map.GetCityIndex(selectedCity), Convert.ToInt32(expCityNum.Text), Convert.ToInt32(expTotal.Text), tagList);
                ACO tsp = new ACO(guide, request);
                tsp.InitData();
                tsp.Search();
                planList.Add(new Plan(tsp, guide, request));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);	
            }
        }
        private void searchFile_Click(object sender, RoutedEventArgs e)
        {
            map.UpdataAdjacencyMartix();
            Guide guide = new Guide(map);
            List<Request> ReqList = FileIO.loadRequestFromTxt(guide);
            this.progressBar.Value = 0;
            this.progressBar.Visibility = Visibility.Visible;
            double value = 0;
            for (int i = 0; i < ReqList.Count; i++)
            {
                ACO tsp = new ACO(guide, ReqList[i]);
                tsp.InitData();
                tsp.Search();
                planList.Add(new Plan(tsp, guide, ReqList[i]));
                progressBar.Dispatcher.
                    Invoke(new Action<DependencyProperty,object>(progressBar.SetValue),
                    DispatcherPriority.Background,
                    ProgressBar.ValueProperty,
                    value);
                value += 100 / ReqList.Count;
            }
            this.progressBar.Visibility = Visibility.Collapsed;
        }
        private void planListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Plan plan = this.planListView.SelectedValue as Plan;
            this.name.Text = plan.Name;
            this.expCityNum.Text = plan.ExpCityNum;
            this.expTotal.Text = plan.ExpTotal;
            this.tagList.Clear();
            foreach(Tag tag in plan.ExpTagList)
            {
                this.tagList.Add(tag);
            }
            visual.ClearHighLight();
            foreach (City city in plan.Path)
            {
                visual.HighLightVertex(city);
            }
            for (int i = 0; i < plan.Path.Count - 1; i++)
            {
                visual.HighLightShortEdge(plan.Path[i], plan.Path[i + 1]);
            }
        }
        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            this.selectedCity = null;
            this.planList.Clear();
            this.name.Text = String.Empty;
            this.expCityNum.Text = String.Empty;
            this.expTotal.Text = String.Empty;
            this.tagList.Clear();
            foreach (string tag in City.tagList)
            {
                tagList.Add(new Tag(tag, 0));
            }
            visual.ClearHighLight();
        }
    }

    /// <summary>
    /// 最短路径类，用于填充界面的ListView
    /// </summary>
    public class ShortPath
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
    /// <summary>
    /// 请求类，用户提供的搜索要求
    /// </summary>
    public class Request
    {
        public string name;
        public int start;
        public int cityNum;
        public int total;
        public List<String> tagList;
        public List<int> rateList;

        public Request()
        {
            tagList = new List<String>();
            rateList = new List<int>();
        }

        public Request(string name, int startCityIndex, int cityNum, int total, List<Tag> taglist)
        {
            tagList = new List<String>();
            rateList = new List<int>();
            this.name = name;
            this.start = startCityIndex;
            this.cityNum = cityNum;
            this.total = total;
            foreach (Tag tag in taglist)
            {
                AddTag(tag);
            }
        }

        private void AddTag(Tag tag)
        {
            this.tagList.Add(tag.tag);
            this.rateList.Add(tag.rate);
        }
    }
    /// <summary>
    /// 规划，系统给出的路径规划
    /// </summary>
    public class Plan
    {
        public String Name { get; set; }
        public String Begin { get; set; }
        public String ExpCityNum { get; set; }
        public String RealCityNum { get; set; }
        public String PathString 
        {
            get
            {
                string result = "";
                foreach (City city in Path)
                {
                    result += city.Name + " ";
                }
                return result;
            }
        }
        public List<City> Path { get; set; }
        public String ExpTotal { get; set; }
        public String RealTotal { get; set; }
        public String Value { get; set; }
        public List<Tag> ExpTagList { get; set; }
        public String RealTagList { get; set; }

        public Plan(ACO tsp,Guide guide, Request theReq)
        {
            this.RealTagList = String.Empty;
            this.ExpTagList = new List<Tag>();
            this.Path = new List<City>();
            int ii = 0;
            this.Name = theReq.name;
            this.Begin = guide.CityList[theReq.start].Name;
            this.ExpCityNum = theReq.cityNum.ToString();
            this.ExpTotal = theReq.total.ToString();
            for (ii = 0; ii < theReq.tagList.Count; ii++)
            {
                this.ExpTagList.Add(new Tag(theReq.tagList[ii], theReq.rateList[ii]));
            }
            this.RealCityNum = tsp.bestAnt.realMovedCount.ToString();
            this.RealTotal = tsp.bestAnt.dbCost.ToString();
            this.Value = tsp.bestAnt.estimateValue.ToString();
            for (ii = 0; ii < tsp.bestAnt.tagList.Count; ii++)
            {
                this.RealTagList += tsp.bestAnt.tagList[ii] + "、";
            }
            if (this.RealTagList.Length >= 2)
            {
                this.RealTagList.Substring(0, this.RealTagList.Length - 2);
            }
            for (ii = 0; ii < tsp.bestAnt.movedCityCount; ii++)
            {
                this.Path.Add(guide.CityList[tsp.bestAnt.path[ii]]);
            }
        }
    }
    /// <summary>
    /// 兴趣标签类
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// 标签分级
        /// </summary>
        public int rate { get; set; }

        public Tag(string tag, int rate)
        {
            this.tag = tag;
            this.rate = rate;
        }
    }
}
