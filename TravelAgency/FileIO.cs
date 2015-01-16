using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using TravelAgency.ACO;
using TravelAgency.Graph;
using Application = Microsoft.Office.Interop.Excel.Application;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace TravelAgency
{
    /// <summary>
    ///     文件读写类，静态类
    /// </summary>
    public static class FileIO
    {
        /// <summary>
        ///     导入地图
        /// </summary>
        /// <param name="map">导入目标</param>
        /// <returns></returns>
        public static bool ImportMap(AdjacencyGraph map)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "地图数据文件|*.map|Excel文件|*.xlsx";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "map";
            if (openFileDialog.ShowDialog() == true)
            {
                var filename = openFileDialog.FileName;
                if (filename.Substring(filename.LastIndexOf(".", StringComparison.Ordinal) + 1) == "xlsx")
                {
                    map.Clear();
                    ImportFormExcel(filename, map);
                }
                else if (filename.Substring(filename.LastIndexOf(".", StringComparison.Ordinal) + 1) == "map")
                {
                    map.Clear();
                    ImportFormBinMap(filename, map);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     导出地图
        /// </summary>
        /// <param name="map">数据来源</param>
        public static void ExportMap(AdjacencyGraph map)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "选择文件";
            saveFileDialog.Filter = "地图数据文件|*.map";
            saveFileDialog.FileName = string.Empty;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.DefaultExt = "map";
            if (saveFileDialog.ShowDialog() == true)
            {
                var filename = saveFileDialog.FileName;
                Stream fStream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
                var binFormat = new BinaryFormatter();
                binFormat.Serialize(fStream, map.VertexList);
                binFormat.Serialize(fStream, map.Dictionary);
                binFormat.Serialize(fStream, City.tagList);
                binFormat.Serialize(fStream, City.tagDictionary);
                fStream.Close();
            }
        }

        /// <summary>
        ///     从二进制流导入地图
        /// </summary>
        /// <param name="path">二进制文件路径</param>
        /// <param name="map">导入目标</param>
        public static void ImportFormBinMap(string path, AdjacencyGraph map)
        {
            Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var binFormat = new BinaryFormatter();
            fStream.Position = 0;
            map.VertexList = (List<City>) binFormat.Deserialize(fStream);
            map.Dictionary = (Dictionary<string, int>) binFormat.Deserialize(fStream);
            City.tagList = (List<string>) binFormat.Deserialize(fStream);
            City.tagDictionary = (Dictionary<string, int>) binFormat.Deserialize(fStream);
            fStream.Close();
        }

        /// <summary>
        ///     从电子表格导入地图
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="map">导入目标</param>
        private static void ImportFormExcel(string path, AdjacencyGraph map)
        {
            if (path == null)
            {
                return;
            }
            var app = new Application();
            var workbook = app.Workbooks.Open(path);
            Worksheet mapSheet = workbook.Sheets[1];

            var i = 2;
            while (mapSheet.Cells[i, 1].Value2 != null)
            {
                string longitude = mapSheet.Cells[i, 2].Value2;
                string latitude = mapSheet.Cells[i, 1].Value2;
                string transitFees = Convert.ToString(mapSheet.Cells[i, 3].Value2);
                string name = mapSheet.Cells[i, 4].Value2;
                var city = new City(name, longitude, latitude, transitFees);
                map.AddVertex(city);
                var j = 5;
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
                var city = map.VertexList.Find(delegate(City c) { return c.Name == mapSheet.Cells[i, 1].Value2; });
                var j = 2;
                while (mapSheet.Cells[i, j].Value2 != null)
                {
                    city.AddTag(mapSheet.Cells[i, j].Value2);
                    j++;
                }
                i++;
            }
            app.Quit();
            MessageBox.Show("导入成功！");
        }

        public static void ExportPathData(string path, List<Path> pathData, int depth)
        {
            var fileInfo = new FileInfo(path);
            var directory = fileInfo.Directory;
            if (directory != null && !directory.Exists)
            {
                directory.Create();
            }
            var i = 0;
            do
            {
                i++;
                fileInfo = new FileInfo(path + i + ".path");
            } while (fileInfo.Exists);
            var fStream = new FileStream(path + "0.path", FileMode.Append, FileAccess.Write);
            var writer = new StreamWriter(fStream);
            writer.WriteLine(depth + " " + i + " " + pathData.Count);
            writer.Flush();
            fStream.Flush();
            fStream.Close();
            fStream = new FileStream(path + i + ".path", FileMode.Create, FileAccess.Write);
            var bitWriter = new BinaryWriter(fStream, Encoding.Default);
            bitWriter.Write(pathData.Count);
            foreach (var p in pathData)
            {
                bitWriter.Write(p.nameIndex.Count);
                foreach (var name in p.nameIndex)
                {
                    bitWriter.Write(name);
                }
                bitWriter.Write(p.tag.Count);
                foreach (var tag in p.tag)
                {
                    bitWriter.Write(tag);
                }
                bitWriter.Write(p.transit);
                bitWriter.Write(p.CityCount);
            }
            bitWriter.Flush();
            fStream.Flush();
            fStream.Close();
        }

        public static List<Path> ImportPathData(string path)
        {
            Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(fStream, Encoding.Default);
            var size = reader.ReadInt32();
            var pathList = new List<Path>();
            for (var i = 0; i < size; i++)
            {
                var p = new Path();
                var nameCount = reader.ReadInt32();
                for (var j = 0; j < nameCount; j++)
                {
                    p.nameIndex.Add(reader.ReadInt32());
                }
                var tagCount = reader.ReadInt32();
                for (var j = 0; j < tagCount; j++)
                {
                    p.tag.Add(reader.ReadInt32());
                }
                p.transit = reader.ReadInt32();
                p.CityCount = reader.ReadInt32();
                pathList.Add(p);
            }
            fStream.Close();
            return pathList;
        }

        /// <summary>
        ///     导入需求文件
        /// </summary>
        /// <param name="guide">导入目标</param>
        /// <returns></returns>
        public static List<Request> LoadRequestFromTxt(Guide guide)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择文件",
                Filter = "需求文件|*.txt",
                FileName = string.Empty,
                FilterIndex = 1,
                RestoreDirectory = true,
                DefaultExt = "txt"
            };
            var requestList = new List<Request>();
            if (openFileDialog.ShowDialog() != true) return requestList;
            var reader = new StreamReader(openFileDialog.FileName, Encoding.Default);
            while (reader.Peek() > 0)
            {
                var temp = reader.ReadLine();
                if (temp == null) continue;
                var subStr = temp.Split('|');
                var newReq = new Request {name = subStr[0]};
                if (guide.Dict.ContainsKey(subStr[1]))
                {
                    newReq.start = guide.Dict[subStr[1]];
                }
                int ii;
                if (Int32.TryParse(subStr[3], out ii))
                {
                    newReq.cityNum = ii;
                }
                if (Int32.TryParse(subStr[4], out ii))
                {
                    newReq.total = ii;
                }
                var subSubStr = subStr[2].Split(' ');
                foreach (var subSubSubStr in subSubStr.Select(str => str.Split(',')))
                {
                    newReq.tagList.Add(subSubSubStr[0]);
                    newReq.rateList.Add(Int32.Parse(subSubSubStr[1]));
                }
                requestList.Add(newReq);
            }
            return requestList;
        }
    }
}