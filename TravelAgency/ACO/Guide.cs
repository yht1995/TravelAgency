using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency
{
    public class Guide
    {
        private AdjacencyGraph map;
        public List<City> CityList
        {
            get { return map.VertexList; }
        }
        public Dictionary<String, int> Dict
        {
            get { return map.Dictionary; }
        }
        public int[,] Expense
        {
            get { return map.AdjacencyMartix; }
        }
        public int[,] Shortest;
        public List<int> [,] Path;
        public List<String> tagList;
        public double[] Parameter { get; set; }
        public int CityNum;

        public Guide(AdjacencyGraph map)
        {
            this.map = map;
            this.CityNum = map.VertexList.Count;
            Parameter = new double[3];
            Parameter[0] = Constants.para_ALPHA;
            Parameter[1] = Constants.para_BETA;
            Parameter[2] = Constants.para_EITA;
            Shortest = new int[CityNum, CityNum];
            Path = new List<int>[CityNum, CityNum];
            tagList = new List<String>();
        }
    }
}

