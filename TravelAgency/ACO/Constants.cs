using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency
{
    static class Constants
    {
        public const int MaxCityNum = 50;
        public const int InitialValue = 99999;
        public const double para_ALPHA = 3.0;//3.0
        public const double para_BETA = 1.0;//1.0
        public const double para_EITA = 5.0;//5.0
        
        //蚁群算法的参数
        public static int searchStep = 18;
        public static double ALPHA = 1.0;
        public static double BETA = 2.0;
        public static double ROU = 0.5;

        public static int N_THREAD= 1;
        public static int N_ANT_COUNT = 80;
        public static int N_IT_COUNT = 30;

        public static double DBQ = 100.0;
        public static double DB_MAX = 10e9;

        public static int RAND_MAX = 0x7fff;

        public static Random rand = new Random(System.DateTime.Now.Millisecond);
        public static double [,] g_Trial = new double[MaxCityNum,MaxCityNum]; 
        

        public static double rnd(double dbLow, double dbUpper)
        {
            double dbTemp = (double)rand.Next(RAND_MAX) / ((double)RAND_MAX + 1.0);
            return dbLow + dbTemp * (dbUpper - dbLow);
        }
    }
}
