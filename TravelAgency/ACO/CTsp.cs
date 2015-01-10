using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TravelAgency
{
    public class CTsp
    {
        CAnt[,] m_cAntAry;
        public CAnt m_cBestAnt;
        public Guide theGuide;
        public Request theReq;
        

        public CTsp(Guide guide, Request req)
        {
            m_cAntAry = new CAnt[Constants.N_THREAD, Constants.N_ANT_COUNT];
            for (int i = 0; i < Constants.N_THREAD; i++)
            {
                for (int j = 0; j < Constants.N_ANT_COUNT; j++)
                {
                    m_cAntAry[i,j] = new CAnt(ref guide, ref req);
                }
                    
            }
            theGuide = guide;
            theReq = req;
            m_cBestAnt = new CAnt(ref guide, ref req);
        }

        public void InitData()
        {
            m_cBestAnt.Initial();
            m_cBestAnt.estimateValue = 0.0;
            for (int i = 0; i < theGuide.CityList.Count; i++)
            {
                for (int j = 0; j < theGuide.CityList.Count; j++)
                {
                    Constants.g_Trial[i, j] = 1.0;
                }
            }
        }

        public void UpdateTrial()
        {
            double[,] dbTempAry = new double[theGuide.CityList.Count,theGuide.CityList.Count];

            for (int i = 0; i < theGuide.CityList.Count; i++)
            {
                for (int j = 0; j < theGuide.CityList.Count; j++)
                {
                    dbTempAry[i, j] = 0.0;
                }
            }

            int m = 0;
            int n = 0;
            for (int i = 0; i < Constants.N_THREAD; i++)
            {
                for (int ii = 0; ii < Constants.N_ANT_COUNT; ii++)
                {
                    for (int j = 1; j < m_cAntAry[i, ii].m_nMovedCityCount; j++)
                    {
                        m = m_cAntAry[i, ii].m_nPath[j];
                        n = m_cAntAry[i, ii].m_nPath[j - 1];
                        dbTempAry[n, m] += Constants.DBQ * m_cAntAry[i, ii].estimateValue / 1000;
                        dbTempAry[m, n] = dbTempAry[n, m];
                    }
                }
            }

            for (int i = 0; i < theGuide.CityList.Count; i++)
            {
                for (int j = 0; j < theGuide.CityList.Count; j++)
                {
                    Constants.g_Trial[i, j] = Constants.g_Trial[i, j] * Constants.ROU + dbTempAry[i, j];
                }
            }
        }

        public void Search()
        {
            for (int i = 0; i < Constants.N_IT_COUNT; i++)
            {
                for(int j = 0; j < Constants.N_ANT_COUNT;j++)
                {
                    Parallel.For(0, Constants.N_THREAD, (k) => 
                    {
                        m_cAntAry[k, j].Search();
                    });

                    for(int k = 0; k < Constants.N_THREAD; k++)
                    {
                        if (m_cAntAry[k,j].estimateValue > m_cBestAnt.estimateValue)
                        {
                            m_cBestAnt.estimateValue = m_cAntAry[k,j].estimateValue;

                            for (int ii = 0; ii < m_cAntAry[k,j].m_nMovedCityCount; ii++)
                            {
                                m_cBestAnt.m_nPath[ii] = m_cAntAry[k,j].m_nPath[ii];
                            }
                            m_cBestAnt.tagList.Clear();
                            for (int jj = 0; jj < m_cAntAry[k,j].tagList.Count; jj++)
                            {
                                m_cBestAnt.tagList.Add(m_cAntAry[k,j].tagList[jj]);
                            }
                      
                            m_cBestAnt.m_dbCost = m_cAntAry[k,j].m_dbCost;
                            m_cBestAnt.m_nRealMovedCount = m_cAntAry[k,j].m_nRealMovedCount;
                            m_cBestAnt.m_nMovedCityCount = m_cAntAry[k,j].m_nMovedCityCount;
                        }
                    }                   
                }
                UpdateTrial();
            }
        }
    }
}
