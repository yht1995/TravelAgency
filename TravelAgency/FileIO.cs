﻿using System;
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
                binFormat.Serialize(fStream, map.Dictionary);
                binFormat.Serialize(fStream, City.tagList);
                binFormat.Serialize(fStream, City.tagDictionary);
                fStream.Close();
            }
        }
        public static void ImportFormBinMap(string path, AdjacencyGraph map)
        {
            map.Clear();
            Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryFormatter binFormat = new BinaryFormatter();
            fStream.Position = 0;
            map.VertexList = (List<City>)binFormat.Deserialize(fStream);
            map.Dictionary = (Dictionary<string, int>)binFormat.Deserialize(fStream);
            City.tagList = (List<string>)binFormat.Deserialize(fStream);
            City.tagDictionary = (Dictionary<string,int>)binFormat.Deserialize(fStream);
            fStream.Close();
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
                City city = map.VertexList.Find(delegate(City c)
                {
                    return c.Name == mapSheet.Cells[i, 1].Value2;
                });
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

        public static void ExportPathData(string path,List<Path> pathData,int depth)
        {
            FileInfo fileInfo = new FileInfo(path);
            var directory = fileInfo.Directory;
            if (!directory.Exists)
            {
                directory.Create();
            }
            int i = 0;
            do
            {
                i++;
                fileInfo = new FileInfo(path + i.ToString() + ".path");
            } while (fileInfo.Exists);
            FileStream fStream = new FileStream(path + "0.path", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fStream);
            writer.WriteLine(depth.ToString() + " " + i.ToString() + " " + pathData.Count.ToString());
            writer.Flush();
            fStream.Flush();
            fStream.Close();
            fStream = new FileStream(path + i.ToString() + ".path", FileMode.Create, FileAccess.Write);
            BinaryWriter bitWriter = new BinaryWriter(fStream,Encoding.Default);
            bitWriter.Write(pathData.Count);
            foreach (Path p in pathData)
            {
                bitWriter.Write(p.NameIndex.Count);
                foreach (int name in p.NameIndex)
                {
                    bitWriter.Write(name);
                }
                bitWriter.Write(p.Tag.Count);
                foreach (int tag in p.Tag)
                {
                    bitWriter.Write(tag);
                }
                bitWriter.Write(p.Transit);
                bitWriter.Write(p.CityCount);
            }
            bitWriter.Flush();
            fStream.Flush();
            fStream.Close();
        }

        public static List<Path> ImportPathData(string path)
        {
            Stream fStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fStream,Encoding.Default);
            int size = reader.ReadInt32();
            List<Path> pathList = new List<Path>();
            for (int i = 0; i < size; i++)
            {
                Path p = new Path();
                int nameCount = reader.ReadInt32();
                for (int j = 0; j < nameCount; j++)
                {
                    p.NameIndex.Add(reader.ReadInt32());
                }
                int tagCount = reader.ReadInt32();
                for (int j = 0; j < tagCount; j++)
                {
                    p.Tag.Add(reader.ReadInt32());
                }
                p.Transit = reader.ReadInt32();
                p.CityCount = reader.ReadInt32();
                pathList.Add(p);
            }
            fStream.Close();
            return pathList;
        }
    }
}
