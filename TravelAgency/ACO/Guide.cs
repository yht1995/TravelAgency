using System;
using System.Collections.Generic;
using TravelAgency.Graph;

namespace TravelAgency.ACO
{
    public class Guide
    {
        private readonly AdjacencyGraph map;

        public Guide(AdjacencyGraph map)
        {
            this.map = map;
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