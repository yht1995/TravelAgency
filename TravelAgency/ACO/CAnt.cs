using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelAgency.ACO
{
    public class CAnt
    {
        private readonly Constants constants;
        private readonly Guide guide;
        private readonly bool[] isRouted;
        private readonly Request request;
        private readonly double[,] trialMartix;
        private int currentCityIndex; //当前所在城市编号
        public int dbCost; //蚂蚁走过的路径花费(边+节点费)
        public double estimateValue; //预期估值 
        public int movedCityCount; //已经去过的城市的数量
        public readonly int[] path; //蚂蚁走的路径
        public int realMovedCount; //实际去过的城市的数量
        public List<String> tagList; //标签

        public CAnt(Guide guide, Request req, Constants constants, ref double[,] gTrial)
        {
            this.constants = constants;
            trialMartix = gTrial;
            tagList = new List<String>();
            path = new int[guide.CityList.Count];
            isRouted = new bool[guide.CityList.Count];
            tagList = null;
            this.guide = guide;
            request = req;
        }

        private double Random(double dbLow, double dbUpper)
        {
            var dbTemp = constants.rand.Next(constants.randMax)/(constants.randMax + 1.0);
            return dbLow + dbTemp*(dbUpper - dbLow);
        }

        public void Initial()
        {
            for (var i = 0; i < guide.CityList.Count; i++)
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

        private void CalValue()
        {
            var index = 0;
            var a = 0;
            var b = 0;
            foreach (var expectTag in request.tagList)
            {
                if (tagList.Contains(expectTag))
                {
                    a += request.rateList[index];
                }

                b += request.rateList[index];
                index = index + 1;
            }
            estimateValue = guide.Parameter[0]*a/b;

            estimateValue -= guide.Parameter[1]*Math.Abs(request.cityNum - realMovedCount)/request.cityNum;

            if (dbCost < request.total)
                estimateValue += guide.Parameter[2]*(request.total - dbCost)/request.total;
            else
                estimateValue += guide.Parameter[2]*3*(request.total - dbCost)/request.total;
        }

        private int ChooseNextCity()
        {
            var nSelectedCity = -1;

            //计算去过和没去过的城市的信息素总和
            var dbTotal = 0.0;
            var prob = new double[guide.CityList.Count];

            for (var i = 0; i < guide.CityList.Count; i++)
            {
                //if (m_nAllowedCity[i] == 1 && theGuide.CityList[i] != null)
                if (guide.CityList[i] != null && i != currentCityIndex)
                {
                    prob[i] = Math.Pow(trialMartix[currentCityIndex, i], constants.alpha)*
                              Math.Pow(1.0/guide.Expense[currentCityIndex, i], constants.beta);
                    dbTotal += prob[i];
                }
                else
                {
                    prob[i] = 0.0;
                }
            }

            //进行城市的随机选取
            if (dbTotal > 0.0)
            {
                var dbTemp = Random(0.0, dbTotal);

                for (var i = 0; i < guide.CityList.Count; i++)
                {
                    if (i == currentCityIndex || guide.CityList[i] == null) continue;
                    dbTemp -= prob[i];
                    if (!(dbTemp < 0.0)) continue;
                    nSelectedCity = i;
                    break;
                }
            }

            if (nSelectedCity != -1) return nSelectedCity;
            for (var i = 0; i < guide.CityList.Count; i++)
            {
                if (i == currentCityIndex || guide.CityList[i] == null) continue;
                nSelectedCity = i;
                break;
            }

            return nSelectedCity;
        }

        private void Move()
        {
            var nCityNo = ChooseNextCity();

            path[movedCityCount] = nCityNo;
            currentCityIndex = nCityNo;
            movedCityCount++;
            if (isRouted[nCityNo]) return;
            isRouted[nCityNo] = true;
            realMovedCount++;
        }

        private void CalPathLength()
        {
            dbCost = 0;
            for (var i = 1; i < movedCityCount; i++)
            {
                var m = path[i];
                var n = path[i - 1];
                dbCost += guide.Expense[m, n];
                dbCost += guide.CityList[m].TransitFees;
            }
            dbCost -= guide.CityList[movedCityCount - 1].TransitFees;
        }

        private void AddTagList()
        {
            for (var i = 1; i < movedCityCount; i++)
            {
                if (i == request.start) continue;
                foreach (var newTag in guide.CityList[path[i]].Tags.Where
                    (newTag => request.tagList.Contains(newTag) && !tagList.Contains(newTag)))
                {
                    tagList.Add(newTag);
                }
            }
        }

        public void Search()
        {
            Initial();
            while (movedCityCount == 1 ||
                   (movedCityCount < constants.searchStep && (path[movedCityCount - 1] != request.start)))
            {
                Move();
            }
            if (movedCityCount == constants.searchStep)
            {
                if (path[movedCityCount - 1] != request.start) return;
                CalPathLength();
                AddTagList();
                realMovedCount = realMovedCount - 1;
                CalValue();
            }
            else
            {
                CalPathLength();
                AddTagList();
                realMovedCount = realMovedCount - 1;
                CalValue();
            }
        }
    }
}