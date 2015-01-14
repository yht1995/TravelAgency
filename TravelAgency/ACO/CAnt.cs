using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TravelAgency
{
    public class CAnt
    {
        public int[] path;           //蚂蚁走的路径
        public int movedCityCount;          //已经去过的城市的数量
        public int realMovedCount;          //实际去过的城市的数量
        public int dbCost;         //蚂蚁走过的路径花费(边+节点费)
        public List<String> tagList;    //标签
        public double estimateValue;    //预期估值 
        
        private int currentCityIndex;               //当前所在城市编号
        private bool[] isRouted;
        private double[,] trialMartix;
        
        private Guide guide;
        private Request request;
        private Constants constants;

        private double random(double dbLow, double dbUpper)
        {
            double dbTemp = (double)constants.rand.Next(constants.RAND_MAX) / ((double)constants.RAND_MAX + 1.0);
            return dbLow + dbTemp * (dbUpper - dbLow);
        }

        public CAnt(Guide guide, Request req,Constants constants,ref double[,] g_Trial)
        {
            this.constants = constants;
            this.trialMartix = g_Trial;
            tagList = new List<String>();
            path = new int[guide.CityList.Count];
            isRouted = new bool[guide.CityList.Count];
            tagList = null;
            this.guide = guide;
            this.request = req;
        }

        public void Initial()
        {
            for (int i = 0; i < guide.CityList.Count; i++)
            {
                path[i] = -1;
                isRouted[i] = false;
            }

            dbCost = 0;

            currentCityIndex = request.start;

            tagList = new List<string>();

            path[0] = currentCityIndex;
            movedCityCount = 1;
            realMovedCount = 0;
        }

        public void calValue()
        {
            int index = 0;
            int a = 0;
            int b = 0;
            foreach (String expectTag in request.tagList)
            {
                if(tagList.Contains(expectTag) == true)
                {
                    a += request.rateList[index];
                }

                b += request.rateList[index];
                index = index + 1;
            }
            estimateValue = guide.Parameter[0] * a/b ;

            estimateValue -= guide.Parameter[1] * Math.Abs(request.cityNum - realMovedCount) / request.cityNum;

            if (dbCost < request.total)
                estimateValue += guide.Parameter[2] * (request.total - dbCost) / request.total;
            else
                estimateValue += guide.Parameter[2] * 3 * (request.total - dbCost) / request.total;

        }

        public int ChooseNextCity()
        {
            int nSelectedCity = -1;

            //计算去过和没去过的城市的信息素总和
            double dbTotal = 0.0;
            double[] prob = new double[guide.CityList.Count];

            for (int i = 0; i < guide.CityList.Count; i++)
            {
                //if (m_nAllowedCity[i] == 1 && theGuide.CityList[i] != null)
                if (guide.CityList[i] != null && i !=  currentCityIndex)
                {
                    prob[i] = Math.Pow(trialMartix[currentCityIndex, i], constants.ALPHA) *
                                Math.Pow(1.0 / guide.Expense[currentCityIndex, i], constants.BETA);
                    dbTotal += prob[i];
                }
                else
                {
                    prob[i] = 0.0;
                }
            }

            //进行城市的随机选取
            double dbTemp = 0.0;
            if (dbTotal > 0.0)
            {
                dbTemp = this.random(0.0, dbTotal);

                for (int i = 0; i < guide.CityList.Count; i++)
                {
                    if (i != currentCityIndex && guide.CityList[i] != null)
                    {
                        dbTemp -= prob[i];
                        if (dbTemp < 0.0)
                        {
                            nSelectedCity = i;
                            break;
                        }
                    }
                }
            }

            if (nSelectedCity == -1)
            {
                for (int i = 0; i < guide.CityList.Count; i++)
                {
                    if (i!=currentCityIndex && guide.CityList[i] != null)
                    {
                        nSelectedCity = i;
                        break;
                    }
                }
            }

            return nSelectedCity;
        }

        public void Move()
        {
            int nCityNo = ChooseNextCity();

            path[movedCityCount] = nCityNo;
            currentCityIndex = nCityNo;
            movedCityCount++;
            if (isRouted[nCityNo] == false)
            {
                isRouted[nCityNo] = true;
                realMovedCount++;
            }
        }

        public void CalPathLength()
        {
            dbCost = 0;
            int m = 0;
            int n = 0;
            for (int i = 1; i < movedCityCount ; i++)
            {
                m = path[i];
                n = path[i - 1];
                dbCost += guide.Expense[m, n];
                dbCost += guide.CityList[m].TransitFees;
            }
            dbCost -= guide.CityList[movedCityCount-1].TransitFees;
        }

        public void AddTagList()
        {
            for (int i = 1; i < movedCityCount; i++)
            {
                if (i != request.start)
                {
                    foreach (String newTag in guide.CityList[path[i]].Tags)
                    {
                        if (request.tagList.Contains(newTag) && !tagList.Contains(newTag))
                        {
                            tagList.Add(newTag);
                        }
                    }
                }
            }
        }

        public void Search()
        {
            Initial();
            while (movedCityCount == 1 || (movedCityCount < constants.searchStep && (path[movedCityCount - 1] != request.start)))
            {
                Move();
            }
            if (movedCityCount == constants.searchStep)
            {
                if (path[movedCityCount-1] == request.start)
                {
                    CalPathLength();
                    AddTagList();
                    realMovedCount = realMovedCount - 1;
                    calValue();
                }
            }
            else
            {
                CalPathLength();
                AddTagList();
                realMovedCount = realMovedCount - 1;
                calValue();
            }
        }
    }
}
