using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;

namespace TravelAgency
{
    public class City
    {
        private string name;
        private double longitude;   //经度
        private double latitude;  //纬度
        private int transitFees;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
            set {
                if (value > 180 || value < -180)
                {
                    throw (new Exception("经度非法"));
                }
                longitude = value; }
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
            set {
                if (value > 90 || value < -90)
                {
                    throw (new Exception("纬度非法"));
                }
                latitude = value; }
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
            Name = name;
            if (!Regex.IsMatch(latitude, @"^[北南]纬"))
            {
                throw (new Exception("纬度输入错误"));
            }
            string temp = Regex.Replace(latitude, @"北纬\s", "");
            temp = Regex.Replace(temp, @"南纬\s", "-");
            double.TryParse(temp, out this.latitude);
        }
    }
}
