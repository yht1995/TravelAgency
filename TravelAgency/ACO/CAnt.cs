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
        public int[] m_nPath;           //蚂蚁走的路径
        //public int[] m_nAllowedCity;    //没去过的城市

        int m_nCurCityNo;               //当前所在城市编号
        public int m_nMovedCityCount;          //已经去过的城市的数量
        public int m_nRealMovedCount;          //实际去过的城市的数量
        public int m_dbCost;         //蚂蚁走过的路径花费(边+节点费)
        public List<String> tagList;    //标签
        public double estimateValue;    //预期估值 
        bool[] isRouted;

        Guide theGuide;
        Request theReq;

        public CAnt(ref Guide guide, ref Request req)
        {
            m_nPath = new int[guide.CityList.Count];
            isRouted = new bool[guide.CityList.Count];
            //m_nAllowedCity = new int[MAXX];
            tagList = null;
            theGuide = guide;
            theReq = req;
        }

        public void Initial()
        {
            for (int i = 0; i < theGuide.CityList.Count; i++)
            {
                //if (theGuide.CityList[i] != null)
                //    m_nAllowedCity[i] = 1;
                //else
                //    m_nAllowedCity[i] = 0;
                m_nPath[i] = -1;
                isRouted[i] = false;
            }

            m_dbCost = 0;

            m_nCurCityNo = theReq.start;

            tagList = new List<string>();

            m_nPath[0] = m_nCurCityNo;
            m_nMovedCityCount = 1;
            m_nRealMovedCount = 0;
        }

        public void calValue()
        {
            int index = 0;
            int 分子 = 0;
            int 分母 = 0;
            foreach (String expectTag in theReq.tagList)
            {
                if(tagList.Contains(expectTag) == true)
                {
                    分子 += theReq.rateList[index];
                }

                分母 += theReq.rateList[index];
                index = index + 1;
            }
            estimateValue = theGuide.Parameter[0] * 分子/分母 ;

            estimateValue -= theGuide.Parameter[1] * Math.Abs(theReq.cityNum - m_nRealMovedCount) / theReq.cityNum;

            if (m_dbCost < theReq.total)
                estimateValue += theGuide.Parameter[2] * (theReq.total - m_dbCost) / theReq.total;
            else
                estimateValue += theGuide.Parameter[2] * 3 * (theReq.total - m_dbCost) / theReq.total;

        }

        public int ChooseNextCity()
        {
            int nSelectedCity = -1;

            //计算去过和没去过的城市的信息素总和
            double dbTotal = 0.0;
            double[] prob = new double[theGuide.CityList.Count];

            for (int i = 0; i < theGuide.CityList.Count; i++)
            {
                //if (m_nAllowedCity[i] == 1 && theGuide.CityList[i] != null)
                if (theGuide.CityList[i] != null && i !=  m_nCurCityNo)
                {
                    prob[i] = Math.Pow(Constants.g_Trial[m_nCurCityNo, i], Constants.ALPHA) *
                                Math.Pow(1.0 / theGuide.Expense[m_nCurCityNo, i], Constants.BETA);
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
                dbTemp = Constants.rnd(0.0, dbTotal);

                for (int i = 0; i < theGuide.CityList.Count; i++)
                {
                    if (i != m_nCurCityNo && theGuide.CityList[i] != null)
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
                for (int i = 0; i < theGuide.CityList.Count; i++)
                {
                    if (i!=m_nCurCityNo && theGuide.CityList[i] != null)
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

            m_nPath[m_nMovedCityCount] = nCityNo;
            m_nCurCityNo = nCityNo;
            m_nMovedCityCount++;
            if (isRouted[nCityNo] == false)
            {
                isRouted[nCityNo] = true;
                m_nRealMovedCount++;
            }
            
        }

        public void CalPathLength()
        {
            m_dbCost = 0;
            int m = 0;
            int n = 0;
            for (int i = 1; i < m_nMovedCityCount ; i++)
            {
                m = m_nPath[i];
                n = m_nPath[i - 1];
                m_dbCost += theGuide.Expense[m, n];
                m_dbCost += theGuide.CityList[m].TransitFees;
            }
            m_dbCost -= theGuide.CityList[m_nMovedCityCount-1].TransitFees;
        }

        public void AddTagList()
        {
            for (int i = 1; i < m_nMovedCityCount; i++)
            {
                if (i != theReq.start)
                {
                    foreach (String newTag in theGuide.CityList[m_nPath[i]].Tags)
                    {
                        if (theReq.tagList.Contains(newTag) && !tagList.Contains(newTag))
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
            while(m_nMovedCityCount ==1 || (m_nMovedCityCount < Constants.searchStep && (m_nPath[m_nMovedCityCount-1] != theReq.start)))
            {
                Move();
            }
            if (m_nMovedCityCount == Constants.searchStep)
            {
                if (m_nPath[m_nMovedCityCount-1] == theReq.start)
                {
                    CalPathLength();
                    AddTagList();
                    m_nRealMovedCount = m_nRealMovedCount - 1;
                    calValue();
                }
            }
            else
            {
                CalPathLength();
                AddTagList();
                m_nRealMovedCount = m_nRealMovedCount - 1;
                calValue();
            }
        }
    }
}
