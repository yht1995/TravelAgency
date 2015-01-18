using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Shapes;

namespace TravelAgency.Graph
{
    /// <summary>
    ///     城市类
    /// </summary>
    [Serializable]
    public class City : IEquatable<City>
    {
        [NonSerialized] 
        public Ellipse ellipse;

        public double GetCenterX()
        {
            return longitude;
        }

        public double GetCenterY()
        {
            return latitude;
        }

        #region 私有成员

        private string name;
        private double longitude; //经度
        private double latitude; //纬度
        private int transitFees;
        private List<Edge> neighborList;
        private List<string> tags;

        #endregion

        #region 属性

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int TransitFees
        {
            get { return transitFees; }
            set
            {
                if (value < 0)
                {
                    throw (new Exception("中转费非法"));
                }
                transitFees = value;
            }
        }

        public List<Edge> NeighborList
        {
            get { return neighborList; }
            set { neighborList = value; }
        }

        public List<string> Tags
        {
            get { return tags; }
            set { tags = value; }
        }

        #endregion

        #region 公有方法

        public City()
        {
            neighborList = new List<Edge>();
            tags = new List<string>();
        }

        public City(string name, string longitude, string latitude, string transitFees)
        {
            this.name = name;
            Latitude = LatitudeClass.FromString(latitude);
            Longitude = LongitudeClass.FromString(longitude);
            int result;
            if (!int.TryParse(transitFees, out result))
            {
                throw (new Exception("中转费非法"));
            }
            this.transitFees = result;
            neighborList = new List<Edge>();
            tags = new List<string>();
        }

        public void AddEdge(City end, int edge)
        {
            var e = new Edge(this, end, edge);
            if (!neighborList.Contains(e))
            {
                neighborList.Add(e);
            }
        }

        public void RemoveEdge(City end)
        {
            neighborList.RemoveAll(a => (a.End == end));
        }

        public Edge GetEdge(City end)
        {
            return (neighborList.Find(a => (a.End == end)));
        }

        public bool Equals(City other)
        {
            return (Name == other.Name);
        }

        public void AddTag(string tag)
        {
            if (tagList.Contains(tag))
            {
                tags.Add(tag);
            }
        }

        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        public void ClearTag()
        {
            tags.Clear();
        }

        #endregion

        #region 静态方法和成员

        public static double GetXmin()
        {
            return longitudeMin;
        }

        public static double GetXmax()
        {
            return longitudeMax;
        }

        public static double GetYmin()
        {
            return latitudeMin;
        }

        public static double GetYmax()
        {
            return latitudeMax;
        }

        public static double longitudeMin = Double.PositiveInfinity;
        public static double longitudeMax = Double.NegativeInfinity;
        public static double latitudeMin = Double.PositiveInfinity;
        public static double latitudeMax = Double.NegativeInfinity;
        public static List<string> tagList = new List<string>();
        public static Dictionary<string, int> tagDictionary = new Dictionary<string, int>();

        public static void AddTagType(string tag)
        {
            if (!tagList.Contains(tag))
            {
                tagList.Add(tag);
                tagDictionary.Add(tag, tagList.Count - 1);
            }
        }

        #endregion
    }

    /// <summary>
    ///     纬度类
    /// </summary>
    public static class LatitudeClass
    {
        /// <summary>
        ///     从字符串转换成经度，函数会抛出异常
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>经度值</returns>
        public static double FromString(string s)
        {
            if (!Regex.IsMatch(s, @"^[南北]纬(\s)*\d"))
            {
                throw (new Exception("纬度输入错误"));
            }
            var temp = Regex.Replace(s, @"北纬(\s)*", "");
            temp = Regex.Replace(temp, @"南纬(\s)*", "-");
            double result;
            if (!double.TryParse(temp, out result))
            {
                throw (new Exception("纬度输入错误"));
            }
            if (result > 90 || result < -90)
            {
                throw (new Exception("纬度非法"));
            }
            return result;
        }

        public static string ToString(double value)
        {
            var result = "";
            if (value > 0)
            {
                result = "北纬 ";
            }
            else if (value < 0)
            {
                result = "南纬 ";
            }
            return result + Math.Abs(value).ToString("F");
        }
    }

    /// <summary>
    ///     经度类
    /// </summary>
    public static class LongitudeClass
    {
        /// <summary>
        ///     从字符串转换成纬度，函数会抛出异常
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns>纬度值</returns>
        public static double FromString(string s)
        {
            if (!Regex.IsMatch(s, @"^[东西]经(\s)*\d"))
            {
                throw (new Exception("经度输入错误"));
            }
            var temp = Regex.Replace(s, @"东经(\s)*", "");
            temp = Regex.Replace(temp, @"西经(\s)*", "-");
            double result;
            if (!double.TryParse(temp, out result))
            {
                throw (new Exception("经度输入错误"));
            }
            if (result > 180 || result < -180)
            {
                throw (new Exception("经度非法"));
            }
            return result;
        }

        public static string ToString(double value)
        {
            var result = "";
            if (value > 0)
            {
                result = "东经 ";
            }
            else if (value < 0)
            {
                result = "西经 ";
            }
            return result + Math.Abs(value).ToString("F");
        }
    }
}