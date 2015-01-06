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
    public partial class MainWindow : Window
    {
        AdjacencyGraph<City, int> map = new AdjacencyGraph<City, int>();
        Visualization<City, int> visual;
        public MainWindow()
        {
            InitializeComponent();
            visual = new Visualization<City, int>(this.canva);
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
    }
}
