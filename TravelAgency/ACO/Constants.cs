using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency
{
    public class Constants
    {
        public int MaxCityNum = 50;
        public const int InitialValue = 99999;
        public const double para_ALPHA = 3.0;
        public const double para_BETA = 1.0;
        public const double para_EITA = 5.0;
    
        //蚁群算法的参数
        public int searchStep = 18;
        public double ALPHA = 1.0;
        public double BETA = 2.0;
        public double ROU = 0.5;

        public int N_ANT_COUNT = 80;
        public int N_IT_COUNT = 30;

        public double DBQ = 100.0;
        public double DB_MAX = 10e9;
        public int RAND_MAX = 0x7fff;

        public Random rand = new Random(System.DateTime.Now.Millisecond);
    }
}
