using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using QuickGraph;

namespace TravelAgency
{
    using Graph = UndirectedGraph<City, TaggedUndirectedEdge<City, int>>;
    class FileIO
    {
        private string OpenExcel()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "Excel文件|*.xlsx";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                return  openFileDialog.FileName;
            }
            return null;
        }

        public void ImportFormExcel(Graph map,List<City> cityList)
        {
            map.Clear();
            string path = OpenExcel();
            if (path == null)
            {
                return;
            }
            Application app = new Application();
            Workbook workbook = app.Workbooks.Open(path);
            Worksheet mapSheet = workbook.Sheets[1];

            int i = 2;
            while (mapSheet.Cells[i,1].Value2 != null)
            {
                string longitude = mapSheet.Cells[i,2].Value2;
                string latitude = mapSheet.Cells[i,1].Value2;
                string transitFees = Convert.ToString(mapSheet.Cells[i, 3].Value2);
                string name = mapSheet.Cells[i,4].Value2;
                City city = new City(name, longitude, latitude, transitFees);
                cityList.Add(city);
                map.AddVertex(city);
                int j = 5;
                while (mapSheet.Cells[i, j + 1].Value2 != null)
                {
                    if (Convert.ToString(mapSheet.Cells[i, j].Value2) != "∞")
                    {
                        int distance = Convert.ToInt32(mapSheet.Cells[i, j].Value2);
                        map.AddEdge(new TaggedUndirectedEdge<City, int>(cityList[i - 2], cityList[j - 5], distance));
                    }
                    j++;
                }
                i++;
            }
            app.Quit();
            System.Windows.Forms.MessageBox.Show("导入成功！");
        }
    }
}
