using System;

namespace TravelAgency.ACO
{
    public class Constants
    {
        public const int InitialValue = 99999;
        public const double ParaAlpha = 3.0;
        public const double ParaBeta = 1.0;
        public const double ParaEita = 5.0;
        public double alpha = 1.0;
        public double beta = 2.0;
        public double dbMax = 10e9;
        public double dbq = 100.0;
        public int maxCityNum = 50;
        public int nAntCount = 80;
        public int nItCount = 30;
        public Random rand = new Random(DateTime.Now.Millisecond);
        public int randMax = 0x7fff;
        public double rou = 0.5;
        public int searchStep = 18;
    }
}