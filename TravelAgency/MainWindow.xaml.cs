using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using TravelAgency.ACO;
using TravelAgency.Graph;

namespace TravelAgency
{
    public partial class MainWindow
    {
        private readonly ObservableCollection<Plan> planList;
        private readonly ObservableCollection<ShortPath> shortPath;
        private readonly ObservableCollection<Tag> tagList;
        private readonly Visualization visual;
        private AdjacencyGraph map = new AdjacencyGraph();
        private City selectedCity;

        public MainWindow()
        {
            InitializeComponent();
            visual = new Visualization(Canva, map);
            visual.OnVertexClickedEvent += visual_OnVertexClickedEvent;
            shortPath = new ObservableCollection<ShortPath>();
            ShortList.ItemsSource = shortPath;
            planList = new ObservableCollection<Plan>();
            PlanListView.ItemsSource = planList;
            tagList = new ObservableCollection<Tag>();
            TagListView.ItemsSource = tagList;
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (!FileIo.ImportMap(map)) return;
            visual.DrawGraph(map);
            tagList.Clear();
            foreach (var tag in City.tagList)
            {
                tagList.Add(new Tag(tag, 0));
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            FileIo.ExportMap(map);
        }

        private void AddCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var geoLoc = visual.GetGeoLocaltion(Mouse.GetPosition(Canva));
                if (double.IsPositiveInfinity(geoLoc.X))
                {
                    geoLoc.X = 25;
                    geoLoc.Y = 40;
                }
                var city = new City("城市名称", LongitudeClass.ToString(geoLoc.X), LatitudeClass.ToString(geoLoc.Y), "100");
                var cityEdit = new CityEdit(city, ref map) {Title = "添加城市"};
                cityEdit.ShowDialog();
                visual.DrawGraph(map);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void visual_OnVertexClickedEvent(City city)
        {
            selectedCity = city;
            CityName.Text = city.Name;
            Latitude.Text = LatitudeClass.ToString(city.Latitude);
            Longitude.Text = LongitudeClass.ToString(city.Longitude);
            TransitFee.Text = city.TransitFees.ToString();
            CityTagList.Items.Clear();
            foreach (var tag in city.Tags)
            {
                CityTagList.Items.Add(tag);
            }
            shortPath.Clear();
            foreach (var end in map.VertexList)
            {
                var path = map.ShortestPath(city, end);
                if (path != null)
                {
                    shortPath.Add(new ShortPath(city, end, path));
                }
            }
        }

        private void shortList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            visual.ClearHighLight();
            var s = ShortList.SelectedValue as ShortPath;
            if (s == null) return;
            var start = map.FindCitybyName(s.Start);
            var end = map.FindCitybyName(s.End);
            visual.HighLightShortEdge(start, end);
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            map.UpdataAdjacencyMartix();
            var guide = new Guide(map);
            var taglist = tagList.Where(tag => tag.rate != 0).ToList();
            try
            {
                #region throw

                if (taglist.Count == 0)
                {
                    throw (new Exception("请选择城市标签"));
                }
                if (Convert.ToInt32(ExpTotal.Text) <= 0)
                {
                    throw (new Exception("出去玩是要花钱的"));
                }
                if (Convert.ToInt32(ExpCityNum.Text) <= 0 || Convert.ToInt32(ExpCityNum.Text) > 10)
                {
                    throw (new Exception("这么多城市玩的过来吗？"));
                }
                if (selectedCity == null)
                {
                    throw (new Exception("请在地图上选择出发城市"));
                }

                #endregion

                var request = new Request(name.Text, map.GetCityIndex(selectedCity), Convert.ToInt32(ExpCityNum.Text),
                    Convert.ToInt32(ExpTotal.Text), taglist);
                var aco = new ACO.ACO(guide, request);
                aco.InitData();
                aco.Search();
                planList.Add(new Plan(aco, guide, request));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchFile_Click(object sender, RoutedEventArgs e)
        {
            map.UpdataAdjacencyMartix();
            var guide = new Guide(map);
            var reqList = FileIo.LoadRequestFromTxt(guide);
            ProgressBar.Value = 0;
            ProgressBar.Visibility = Visibility.Visible;
            double value = 0;
            foreach (var request in reqList)
            {
                var tsp = new ACO.ACO(guide, request);
                tsp.InitData();
                tsp.Search();
                planList.Add(new Plan(tsp, guide, request));
                ProgressBar.Dispatcher.
                    Invoke(new Action<DependencyProperty, object>(ProgressBar.SetValue),
                        DispatcherPriority.Background,
                        RangeBase.ValueProperty,
                        value);
                // ReSharper disable once PossibleLossOfFraction
                value += 100/reqList.Count;
            }
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void planListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var plan = PlanListView.SelectedValue as Plan;
            if (plan == null) return;
            name.Text = plan.Name;
            ExpCityNum.Text = plan.ExpCityNum;
            ExpTotal.Text = plan.ExpTotal;
            tagList.Clear();
            foreach (var tag in plan.ExpTagList)
            {
                tagList.Add(tag);
            }
            visual.ClearHighLight();
            foreach (var city in plan.Path)
            {
                visual.HighLightVertex(city);
            }
            for (var i = 0; i < plan.Path.Count - 1; i++)
            {
                visual.HighLightShortEdge(plan.Path[i], plan.Path[i + 1]);
            }
        }

        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            selectedCity = null;
            planList.Clear();
            name.Text = String.Empty;
            ExpCityNum.Text = String.Empty;
            ExpTotal.Text = String.Empty;
            tagList.Clear();
            foreach (var tag in City.tagList)
            {
                tagList.Add(new Tag(tag, 0));
            }
            visual.ClearHighLight();
        }
    }

    /// <summary>
    ///     最短路径类，用于填充界面的ListView
    /// </summary>
    public class ShortPath
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="start">起始城市</param>
        /// <param name="end">目标城市</param>
        /// <param name="path">路径列表</param>
        public ShortPath(City start, City end, List<Edge> path)
        {
            Start = start.Name;
            End = end.Name;
            Path += start.Name;
            Fee = 0;
            foreach (var e in path)
            {
                Path += "->" + e.End.Name;
                Fee += e.Value + e.End.TransitFees;
            }
            Fee -= end.TransitFees;
        }

        public string Start { get; private set; }
        public string End { get; private set; }
        public string Path { get; private set; }
        public int Fee { get; private set; }
    }

    /// <summary>
    ///     请求类，用户提供的搜索要求
    /// </summary>
    public class Request
    {
        public int cityNum;
        public string name;
        public List<int> rateList;
        public int start;
        public List<String> tagList;
        public int total;

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
            start = startCityIndex;
            this.cityNum = cityNum;
            this.total = total;
            foreach (var tag in taglist)
            {
                AddTag(tag);
            }
        }

        private void AddTag(Tag tag)
        {
            tagList.Add(tag.tag);
            rateList.Add(tag.rate);
        }
    }

    /// <summary>
    ///     规划，系统给出的路径规划
    /// </summary>
    public class Plan
    {
        public Plan(ACO.ACO tsp, Guide guide, Request theReq)
        {
            RealTagList = String.Empty;
            ExpTagList = new List<Tag>();
            Path = new List<City>();
            int ii;
            Name = theReq.name;
            Begin = guide.CityList[theReq.start].Name;
            ExpCityNum = theReq.cityNum.ToString();
            ExpTotal = theReq.total.ToString();
            for (ii = 0; ii < theReq.tagList.Count; ii++)
            {
                ExpTagList.Add(new Tag(theReq.tagList[ii], theReq.rateList[ii]));
            }
            RealCityNum = tsp.bestAnt.realMovedCount.ToString();
            RealTotal = tsp.bestAnt.dbCost.ToString();
            Value = tsp.bestAnt.estimateValue.ToString(CultureInfo.InvariantCulture);
            for (ii = 0; ii < tsp.bestAnt.tagList.Count; ii++)
            {
                RealTagList += tsp.bestAnt.tagList[ii] + "、";
            }
            if (RealTagList.Length >= 2)
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                RealTagList.Substring(0, RealTagList.Length - 2);
            }
            for (ii = 0; ii < tsp.bestAnt.movedCityCount; ii++)
            {
                Path.Add(guide.CityList[tsp.bestAnt.path[ii]]);
            }
        }

        public String Name { get; set; }
        public String Begin { get; set; }
        public String ExpCityNum { get; set; }
        public String RealCityNum { get; set; }

        public String PathString
        {
            get { return Path.Aggregate("", (current, city) => current + (city.Name + " ")); }
        }

        public List<City> Path { get; set; }
        public String ExpTotal { get; set; }
        public String RealTotal { get; set; }
        public String Value { get; set; }
        public List<Tag> ExpTagList { get; set; }
        public String RealTagList { get; set; }
    }

    /// <summary>
    ///     兴趣标签类
    /// </summary>
    public class Tag
    {
        public Tag(string tag, int rate)
        {
            this.tag = tag;
            this.rate = rate;
        }

        /// <summary>
        ///     标签名称
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string tag { get; set; }

        /// <summary>
        ///     标签分级
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int rate { get; set; }
    }
}