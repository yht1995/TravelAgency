using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TravelAgency
{
    public static class FileIO
    {
        public static bool ImportMap(AdjacencyGraph map)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "地图数据文件|*.map|Excel文件|*.xlsx";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "map";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                if (filename.Substring(filename.LastIndexOf(".") + 1) == "xlsx")
                {
                    ImportFormExcel(filename, map);
                }
                else if (filename.Substring(filename.LastIndexOf(".") + 1) == "map")
                {
                    ImportFormBinMap(filename, map);
                }
                return true;
            }
            return false;
        }
        public static void ExportMap(AdjacencyGraph map)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "选择文件";
            saveFileDialog.Filter = "地图数据文件|*.map";
            saveFileDialog.FileName = string.Empty;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.DefaultExt = "map";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                Stream fStream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
                BinaryFormatter binFormat = new BinaryFormatter();
                binFormat.Serialize(fStream, map.VertexList);
                binFormat.Serialize(fStream, City.tagList);
                fStream.Close();
            }
        }
        private static void ImportFormBinMap(string path, AdjacencyGraph map)
        {
            map.Clear();
            Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryFormatter binFormat = new BinaryFormatter();
            fStream.Position = 0;
            map.VertexList = (List<City>)binFormat.Deserialize(fStream);
            City.tagList = (List<string>)binFormat.Deserialize(fStream);
            fStream.Close();
            map.Dictionary.Clear();
            foreach (City c in map.VertexList)
            {
                map.Dictionary.Add(c.Name, c);
            }
        }
        private static void ImportFormExcel(string path,AdjacencyGraph map)
        {
            map.Clear();
            if (path == null)
            {
                return;
            }
            Application app = new Application();
            Workbook workbook = app.Workbooks.Open(path);
            Worksheet mapSheet = workbook.Sheets[1];

            int i = 2;
            while (mapSheet.Cells[i, 1].Value2 != null)
            {
                string longitude = mapSheet.Cells[i, 2].Value2;
                string latitude = mapSheet.Cells[i, 1].Value2;
                string transitFees = Convert.ToString(mapSheet.Cells[i, 3].Value2);
                string name = mapSheet.Cells[i, 4].Value2;
                City city = new City(name, longitude, latitude, transitFees);
                map.AddVertex(city);
                int j = 5;
                while (mapSheet.Cells[i, j + 1].Value2 != null)
                {
                    if (Convert.ToString(mapSheet.Cells[i, j].Value2) != "∞")
                    {
                        int distance = Convert.ToInt32(mapSheet.Cells[i, j].Value2);
                        map.AddEdge(i - 2, j - 5, distance);
                    }
                    j++;
                }
                i++;
            }
            i = 1;
            mapSheet = workbook.Sheets[3];
            while (mapSheet.Cells[i, 1].Value2 != null)
            {
                City.AddTagType(mapSheet.Cells[i, 1].Value2);
                i++;
            }
            i = 1;
            mapSheet = workbook.Sheets[2];
            while (mapSheet.Cells[i, 1].Value2 != null)
            {
                City city = map.FindCityByName(mapSheet.Cells[i, 1].Value2);
                int j = 1;
                while (mapSheet.Cells[i, j + 1].Value2 != null)
                {
                    city.AddTag(mapSheet.Cells[i, j].Value2);
                    j++;
                }
                i++;
            }
            app.Quit();
            System.Windows.Forms.MessageBox.Show("导入成功！");
        }
    }
}
