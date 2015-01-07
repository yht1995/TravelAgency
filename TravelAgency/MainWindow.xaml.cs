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

namespace TravelAgency
{
    public partial class MainWindow : Window
    {
        AdjacencyGraph map = new AdjacencyGraph();
        Visualization visual;

        public MainWindow()
        {
            InitializeComponent();
            visual = new Visualization(this.canva,ref map);
            visual.OnVertexClickedEvent += visual_OnVertexClickedEvent;
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

        private void visual_OnVertexClickedEvent(City city)
        {
            this.cityName.Text = city.Name.ToString();
            this.latitude.Text = LatitudeClass.ToString(city.Latitude);
            this.longitude.Text = LongitudeClass.ToString(city.Longitude);
            this.transitFee.Text = city.TransitFees.ToString();
            this.neighborList.Items.Clear();
            foreach(Edge e in map.GeintsofVertex(city))
            {
                this.neighborList.Items.Add(e.End.Name == city.Name?e.Start.Name:e.End.Name);
            }
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
                cityEdit.ShowDialog();
                visual.DrawGraph(map);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
