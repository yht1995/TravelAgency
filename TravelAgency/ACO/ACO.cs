using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TravelAgency
{
    public class ACO
    {
        CAnt[] antArray;
        public CAnt bestAnt;
        public Guide guide;
        public Request request;
        private Constants constants = new Constants();
        double[,] trialMartix;

        public ACO(Guide guide, Request request)
        {
            trialMartix = new double[constants.MaxCityNum,constants.MaxCityNum];
            antArray = new CAnt[constants.N_ANT_COUNT];
            for (int j = 0; j < constants.N_ANT_COUNT; j++)
            {
                antArray[j] = new CAnt(guide, request, constants, ref trialMartix);
            }
            this.guide = guide;
            this.request = request;
            bestAnt = new CAnt(guide, request, constants, ref trialMartix);
        }

        public void InitData()
        {
            bestAnt.Initial();
            bestAnt.estimateValue = 0.0;
            for (int i = 0; i < guide.CityList.Count; i++)
            {
                for (int j = 0; j < guide.CityList.Count; j++)
                {
                    trialMartix[i, j] = 1.0;
                }
            }
        }

        public void UpdateTrial()
        {
            double[,] dbTempAry = new double[guide.CityList.Count,guide.CityList.Count];

            for (int i = 0; i < guide.CityList.Count; i++)
            {
                for (int j = 0; j < guide.CityList.Count; j++)
                {
                    dbTempAry[i, j] = 0.0;
                }
            }

            int m = 0;
            int n = 0;
                for (int i = 0; i < constants.N_ANT_COUNT; i++)
                {
                    for (int j = 1; j < antArray[i].movedCityCount; j++)
                    {
                        m = antArray[i].path[j];
                        n = antArray[i].path[j - 1];
                        dbTempAry[n, m] += constants.DBQ * antArray[i].estimateValue / 1000;
                        dbTempAry[m, n] = dbTempAry[n, m];
                    }
                }

            for (int i = 0; i < guide.CityList.Count; i++)
            {
                for (int j = 0; j < guide.CityList.Count; j++)
                {
                    trialMartix[i, j] = trialMartix[i, j] * constants.ROU + dbTempAry[i, j];
                }
            }
        }

        public void Search()
        {
            for (int i = 0; i < constants.N_IT_COUNT; i++)
            {
                for(int j = 0; j < constants.N_ANT_COUNT;j++)
                {
                    antArray[j].Search();
                    if (antArray[j].estimateValue > bestAnt.estimateValue)
                    {
                        bestAnt.estimateValue = antArray[j].estimateValue;

                        for (int ii = 0; ii < antArray[j].movedCityCount; ii++)
                        {
                            bestAnt.path[ii] = antArray[j].path[ii];
                        }
                        bestAnt.tagList.Clear();
                        for (int jj = 0; jj < antArray[j].tagList.Count; jj++)
                        {
                            bestAnt.tagList.Add(antArray[j].tagList[jj]);
                        }

                        bestAnt.dbCost = antArray[j].dbCost;
                        bestAnt.realMovedCount = antArray[j].realMovedCount;
                        bestAnt.movedCityCount = antArray[j].movedCityCount;
                    }
                }
            }
            UpdateTrial();
        }  
    }
}
