using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;

namespace TravelAgency
{
    public class Latitude
    {
        private double longitude;
        public double Value
        {
            get { return longitude; }
            set
            {
                if (value > 90 || value < -90)
                {
                    throw (new Exception("纬度非法"));
                }
                longitude = value;
            }
        }

        public void FromString(string s)
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
            Value = result;
        }

        override public string ToString()
        {
            string result = "";
            if (Value > 0)
            {
                result = "北纬 ";
            }
            else if (Value < 0)
            {
                result = "南纬 ";
            }
            return result + Math.Abs(Value).ToString("F");
        }
    }

    public class Longitude
    {
        private double longitude;
        public double Value
        {
            get { return longitude; }
            set
            {
                if (value > 180 || value < -180)
                {
                    throw (new Exception("经度非法"));
                }
                longitude = value;
            }
        }

        public void FromString(string s)
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
            Value = result;
        }

        override public string ToString()
        {
            string result = "";
            if (Value > 0)
            {
                result = "东经 ";
            }
            else if (Value < 0)
            {
                result = "西经 ";
            }
            return result + Math.Abs(Value).ToString("F");
        }
    }

    public class City
        : IEquatable<City>, IVertexVisualization
    {
        private string name;
        private Longitude longitude;   //经度

        public Longitude Longitude
        {
            get { return longitude; }
            private set { longitude = value; }
        }
        private Latitude latitude;  //纬度

        public Latitude Latitude
        {
            get { return latitude; }
            private set { latitude = value; }
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
            this.latitude = new Latitude();
            this.longitude = new Longitude();
            this.name = name;
            this.longitude.FromString(longitude);
            this.latitude.FromString(latitude);
            int result = new int();
            if (!int.TryParse(transitFees,out result))
            {
                throw (new Exception("中转费非法"));
            }
            this.transitFees = result;
        }

        bool IEquatable<City>.Equals(City other)
        {
            return (this.name == other.name &&
                this.latitude.Value == other.latitude.Value &&
                this.longitude.Value == other.longitude.Value);
        }

        double IVertexVisualization.GetCenterX()
        {
            return Longitude.Value;
        }

        double IVertexVisualization.GetCenterY()
        {
            return Latitude.Value;
        }

        System.Windows.Shapes.Ellipse ellipse;
        System.Windows.Shapes.Ellipse IVertexVisualization.ellipse
        {
            get{return ellipse;}
            set{ellipse = value;}
        }
    }
}
