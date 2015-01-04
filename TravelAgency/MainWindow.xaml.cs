using QuickGraph;
using QuickGraph.Algorithms;
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
    /// <summary>
    /// This is a Window that uses NetworkView to display a flow-chart.
    /// </summary>

    using Graph = UndirectedGraph<City, TaggedUndirectedEdge<City, int>>;
    public partial class MainWindow : Window
    {
        Graph map = new Graph();
        List<City> cityList = new List<City>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileIO file = new FileIO();
            file.ImportFormExcel(map,cityList);
            Visualization visualization = new Visualization(canva);
            visualization.DrawMap(map);
        }
    }
}
