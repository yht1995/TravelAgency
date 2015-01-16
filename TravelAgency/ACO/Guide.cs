using System;
using System.Collections.Generic;
using TravelAgency.Graph;

namespace TravelAgency.ACO
{
    public class Guide
    {
        private readonly AdjacencyGraph map;
        public int cityNum;
        public List<int>[,] path;
        public int[,] shortest;
        public List<String> tagList;

        public Guide(AdjacencyGraph map)
        {
            this.map = map;
            cityNum = map.VertexList.Count;
            Parameter = new double[3];
            Parameter[0] = Constants.ParaAlpha;
            Parameter[1] = Constants.ParaBeta;
            Parameter[2] = Constants.ParaEita;
            shortest = new int[cityNum, cityNum];
            path = new List<int>[cityNum, cityNum];
            tagList = new List<String>();
        }

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

        public double[] Parameter { get; set; }
    }
}