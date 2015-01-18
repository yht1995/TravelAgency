using System;

namespace TravelAgency.ACO
{
    public class Constants
    {
        private double paraAlpha = 3.0;
        private double paraBeta = 1.0;
        private double paraEta = 5.0;
        public readonly double alpha = 1.0;
        public readonly double beta = 2.0;
        public readonly double rou = 0.5;
        public readonly double dbq = 100.0;
        public int maxCityNum = 50;
        private int antCount = 80;
        private int itCount = 30;
        public int randMax = 0x7fff;
        public int searchStep = 18;
        public Random rand = new Random(DateTime.Now.Millisecond);

        public double ParaAlpha
        {
            get { return paraAlpha; }
            set { paraAlpha = value; }
        }

        public double ParaBeta
        {
            get { return paraBeta; }
            set { paraBeta = value; }
        }

        public double ParaEta
        {
            get { return paraEta; }
            set { paraEta = value; }
        }

        public int AntCount
        {
            get { return antCount; }
            set { antCount = value; }
        }

        public int ItCount
        {
            get { return itCount; }
            set { itCount = value; }
        }
    }
}