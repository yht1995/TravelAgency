﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace TravelAgency
{
    /// <summary>
    /// CityEdit.xaml 的交互逻辑
    /// </summary>
    public partial class CityEdit : Window
    {
        private ObservableCollection<Edge> edgeList;
        private AdjacencyGraph map;
        private City city;

        public CityEdit(City city,ref AdjacencyGraph map)
        {
            InitializeComponent();
            edgeList = new ObservableCollection<Edge>();
            this.city = city;
            this.longtitudeSign.SelectedIndex = city.Longitude > 0 ? 0 : 1;
            this.latitudeSign.SelectedIndex = city.Latitude > 0 ? 0 : 1;
            this.latitude.Text = Math.Abs(city.Latitude).ToString("F");
            this.longitude.Text = Math.Abs(city.Longitude).ToString("F");
            this.cityName.Text = city.Name;
            this.transitFee.Text = city.TransitFees.ToString();
            this.edgeListView.ItemsSource = edgeList;
            this.map = map;
            foreach (City c in map.VertexList)
            {
                if (city == c)
                {
                }
                else if (city.GetEdge(c)==null)
                {
                    this.cityList.Items.Add(c.Name);
                }
                else
                {
                    edgeList.Add(city.GetEdge(c));
                }
            }
            foreach (string tag in City.tagList)
            {
                this.tagList.Items.Add(tag);
                if (city.HasTag(tag))
                {
                    this.tagList.SelectedItems.Add(tag);
                }
            }
        }

        private void AddEdge_Click(object sender, RoutedEventArgs e)
        {
            if (this.cityList.SelectedValue != null)
            {
                City end = map.VertexList.Find(delegate(City c)
                {
                    return c.Name == cityList.SelectedValue.ToString();
                });
                edgeList.Add(new Edge(city, end, 0));
                cityList.Items.Remove(cityList.SelectedItem);
            }
        }

        private void RemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            if (this.edgeListView.SelectedValue != null)
            {
                City end = edgeList[this.edgeListView.SelectedIndex].End;
                edgeList.RemoveAt(this.edgeListView.SelectedIndex);
                cityList.Items.Add(end.Name);
            }
        }
        private void UpdateCity()
        {
            city = new City();
            city.Name = this.cityName.Text;
            city.Longitude = LongitudeClass.FromString(this.longtitudeSign.Text + this.longitude.Text);
            city.Latitude = LatitudeClass.FromString(this.latitudeSign.Text + this.latitude.Text);
            city.TransitFees = Convert.ToInt32(this.transitFee.Text);
            city.ClearTag();
            foreach (string tag in this.tagList.SelectedItems)
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
                foreach (Edge edge in edgeList)
                {
                    map.AddEdge(city, edge.End, edge.Value);
                }
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Cancal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
