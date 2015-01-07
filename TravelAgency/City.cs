using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;

namespace TravelAgency
{
    public static class LatitudeClass
    {
        public static double FromString(string s)
        {
            if (!Regex.IsMatch(s, @"^[南北]纬(\s)*\d"))
            {
                throw (new Exception("纬度输入错误"));
            }
            string temp = Regex.Replace(s, @"北纬(\s)*", "");
            temp = Regex.Replace(temp, @"南纬(\s)*", "-");
            double result = new double();
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
            string result = "";
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
    
    public class LongitudeClass
    {
        public static double FromString(string s)
        {
            if (!Regex.IsMatch(s, @"^[东西]经(\s)*\d"))
            {
                throw (new Exception("经度输入错误"));
            }
            string temp = Regex.Replace(s, @"东经(\s)*", "");
            temp = Regex.Replace(temp, @"西经(\s)*", "-");
            double result = new double();
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
            string result = "";
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

    [Serializable]
    public class City : IEquatable<City>
    {
        #region 私有成员
        private string name;
        private double longitude;   //经度
        private double latitude;  //纬度
        private int transitFees;
        private List<Edge> neighborList;
        private List<string> tags;
        #endregion

        #region 属性
        public double Longitude
        {
            get { return longitude; }
            set 
            {
                longitude = value; 
                if (this.longitude > City.longitudeMax)
                {
                    City.longitudeMax = this.longitude;
                }
                if (this.longitude < City.longitudeMin)
                {
                    City.longitudeMin = this.longitude;
                }
            }
        }
        public double Latitude
        {
            get { return latitude; }
            set 
            { 
                latitude = value;
                if (this.latitude > City.latitudeMax)
                {
                    City.latitudeMax = this.latitude;
                }
                if (this.latitude < City.latitudeMin)
                {
                    City.latitudeMin = this.latitude;
                }
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int TransitFees
        {
            get { return transitFees; }
            set {
                if (value < 0)
                {
                    throw (new Exception("中转费非法"));
                }
                transitFees = value; }
        }
        public List<Edge> NeighborList
        {
            get { return neighborList; }
            set { neighborList = value; }
        }
        [NonSerialized]
        #endregion
        public Ellipse ellipse;
        #region 公有方法
        public City()
        {
            this.neighborList = new List<Edge>();
            this.tags = new List<string>();
        }

        public City(string name, string longitude, string latitude, string transitFees)
        {
            this.name = name;
            this.Latitude = LatitudeClass.FromString(latitude);
            this.Longitude = LongitudeClass.FromString(longitude);
            int result = new int();
            if (!int.TryParse(transitFees,out result))
            {
                throw (new Exception("中转费非法"));
            }
            this.transitFees = result;
            this.neighborList = new List<Edge>();
            this.tags = new List<string>();
        }

        public void AddEdge(City end, int edge)
        {
            Edge e = new Edge(this, end, edge);
            if (!neighborList.Contains(e))
            {
                this.neighborList.Add(e);
            }
        }

        public void RemoveEdge(City end)
        {
            this.neighborList.RemoveAll(delegate(Edge a)
            {
                return (a.End == end);
            });
        }

        public Edge GetEdge(City end)
        {
            return (this.neighborList.Find(delegate(Edge a)
            {
                return (a.End == end);
            }));
        }

        public bool Equals(City other)
        {
            return (this.Name == other.Name &&
                this.Latitude == other.Latitude &&
                this.Longitude == other.Longitude);
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

        #endregion
        public double GetCenterX()
        {
            return longitude;
        }

        public double GetCenterY()
        {
            return latitude;
        }

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
        public static void AddTagType(string tag)
        {
            if (!tagList.Contains(tag))
            {
                tagList.Add(tag);
            }
        }
        #endregion
    }
}
