using System;
using System.Collections.ObjectModel;
using System.Windows;
using TravelAgency.Graph;

namespace TravelAgency
{
    /// <summary>
    ///     CityEdit.xaml 的交互逻辑
    /// </summary>
    public partial class CityEdit
    {
        private readonly ObservableCollection<Edge> edgeList;
        private readonly AdjacencyGraph map;
        private City city;

        public CityEdit(City city, ref AdjacencyGraph map)
        {
            InitializeComponent();
            edgeList = new ObservableCollection<Edge>();
            this.city = city;
            LongtitudeSign.SelectedIndex = city.Longitude > 0 ? 0 : 1;
            LatitudeSign.SelectedIndex = city.Latitude > 0 ? 0 : 1;
            Latitude.Text = Math.Abs(city.Latitude).ToString("F");
            Longitude.Text = Math.Abs(city.Longitude).ToString("F");
            CityName.Text = city.Name;
            TransitFee.Text = city.TransitFees.ToString();
            EdgeListView.ItemsSource = edgeList;
            this.map = map;
            foreach (var c in map.VertexList)
            {
                if (city == c)
                {
                }
                else if (city.GetEdge(c) == null)
                {
                    CityList.Items.Add(c.Name);
                }
                else
                {
                    edgeList.Add(city.GetEdge(c));
                }
            }
            foreach (var tag in City.tagList)
            {
                TagList.Items.Add(tag);
                if (city.HasTag(tag))
                {
                    TagList.SelectedItems.Add(tag);
                }
            }
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            if (CityList.SelectedValue == null) return;
            var end = map.VertexList.Find(c => c.Name == CityList.SelectedValue.ToString());
            edgeList.Add(new Edge(city, end, 0));
            CityList.Items.Remove(CityList.SelectedItem);
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            if (EdgeListView.SelectedValue == null) return;
            var end = edgeList[EdgeListView.SelectedIndex].End;
            edgeList.RemoveAt(EdgeListView.SelectedIndex);
            CityList.Items.Add(end.Name);
        }

        private void UpdateCity()
        {
            city = new City
            {
                Name = CityName.Text,
                Longitude = LongitudeClass.FromString(LongtitudeSign.Text + Longitude.Text),
                Latitude = LatitudeClass.FromString(LatitudeSign.Text + Latitude.Text),
                TransitFees = Convert.ToInt32(TransitFee.Text)
            };
            city.ClearTag();
            foreach (string tag in TagList.SelectedItems)
            {
                city.AddTag(tag);
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                map.RemoveVertex(city);
                UpdateCity();
                map.AddVertex(city);
                foreach (var edge in edgeList)
                {
                    map.AddEdge(city, edge.End, edge.Value);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Cancal_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}