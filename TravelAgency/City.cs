using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class City
        : IEquatable<City>,IVertexVisualization
    {
        public static double longitudeMin = Double.PositiveInfinity;
        public static double longitudeMax;
        public static double latitudeMin = Double.PositiveInfinity;
        public static double latitudeMax;

        private string name;
        private double longitude;   //经度
        public double Longitude
        {
            get { return longitude; }
            private set { longitude = value; }
        }
        private double latitude;  //纬度

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        private int transitFees;

        public string Name
        {
            get { return name; }
            private set { name = value; }
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

        public City(string name, string longitude, string latitude, string transitFees)
        {
            this.name = name;
            this.latitude = LatitudeClass.FromString(latitude);
            this.longitude = LongitudeClass.FromString(longitude);
            int result = new int();
            if (!int.TryParse(transitFees,out result))
            {
                throw (new Exception("中转费非法"));
            }
            this.transitFees = result;

            if (this.latitude > City.latitudeMax)
            {
                City.latitudeMax = this.latitude;
            }
            if (this.latitude < City.latitudeMin)
            {
                City.latitudeMin = this.latitude;
            }
            if (this.longitude > City.longitudeMax)
            {
                City.longitudeMax = this.longitude;
            }
            if (this.longitude < City.longitudeMin)
            {
                City.longitudeMin = this.longitude;
            }
        }

        public bool Equals(City other)
        {
            return (this.Name == other.Name &&
                this.Latitude == other.Latitude &&
                this.Longitude == other.Longitude);
        }

        double IVertexVisualization.GetCenterX()
        {
            return longitude;
        }

        double IVertexVisualization.GetCenterY()
        {
            return latitude;
        }
        
        [NonSerialized]
        private System.Windows.Shapes.Ellipse ellipse;

        public System.Windows.Shapes.Ellipse Ellipse
        {
            get { return ellipse; }
            set { ellipse = value; }
        }

        double IVertexVisualization.GetXmin()
        {
            return longitudeMin;
        }

        double IVertexVisualization.GetXmax()
        {
            return longitudeMax;
        }

        double IVertexVisualization.GetYmin()
        {
            return latitudeMin;
        }

        double IVertexVisualization.GetYmax()
        {
            return latitudeMax;
        }
    }
}
