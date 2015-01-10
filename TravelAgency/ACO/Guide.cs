using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency
{
    public class Guide
    {
        //导游有城市知识(City List)、地图（数字下标的邻接矩阵）
        //导游有各城市到其他城市的最少消费策略（权和二维数组以及路径数组）
        //导游有一个路线表以及其各项指标 【Plan】        
        //导游有一个各方面估价指标预期
        //导游可以更新地理信息()和计算新的消费策略()
        //导游可以根据游客请求【Request】给出推荐方案【Plan】
        
        public List<City> CityList;
        public Dictionary<String, int> Dict;
        public int[,] Expense { get; set; }
        public int[,] Shortest;
        public List<int> [,] Path;

        public List<String> tagList;
        //public Plan GuidePlan;
        public double[] Parameter { get; set; }

        public Guide(int MaxCityNum = Constants.MaxCityNum)
        {
            initial(MaxCityNum);
        }

        public void initial(int MaxCityNum = Constants.MaxCityNum)
        {
            //每一次修改城市信息时，Expense CityList 和Dict都要看看会不会变
            int i, j;
            Expense = new int[MaxCityNum, MaxCityNum];

            for (i = 0; i < Constants.MaxCityNum; i++)
            {
                for (j = 0; j < Constants.MaxCityNum; j++)
                {
                    Expense[i, j] = Constants.InitialValue;
                }
            }

            CityList = new List<City>();

            Dict = new Dictionary<string, int>();
            
            Parameter = new double[3];
            Parameter[0] = Constants.para_ALPHA;
            Parameter[1] = Constants.para_BETA;
            Parameter[2] = Constants.para_EITA;

            Shortest = new int[MaxCityNum, MaxCityNum];

            Path = new List<int>[MaxCityNum, MaxCityNum];

            tagList = new List<String>();
            //  GuidePlan = new Plan();
        }
        //通过更新后的城市信息来计算新的多源最短路及保存路径
        public void generateShortest()
        {
            /*//顶点数        ->nVertexCount 
            //邻接矩阵 Expense -> _arrDis
            //最短距离矩阵 _arrDis -> Shortest
            //最短路径矩阵 _arrPath -> Path(内置矩阵，在这里面把路径赋给Path[,]后就不管了)
            //对应矩阵

            //导入
            //Floyd
            //导出

            int nVertexCount = 0;
            int [,] _arrDis = new int[nVertexCount,nVertexCount];
            int [,] _arrPath = new int[nVertexCount,nVertexCount];*/

             
            int i, j, k;
            int [,] _arrPath = new int[Constants.MaxCityNum,Constants.MaxCityNum];
            for (i = 0; i < CityList.Count; i++)
            {
                for (j = 0; j < CityList.Count; j++)
                {
                    if (Expense[i, j] != Constants.InitialValue)
                        Shortest[i, j] = Expense[i, j] + (CityList[i].TransitFees + CityList[j].TransitFees) / 2;
                    else
                        Shortest[i, j] = Expense[i, j];
                    _arrPath[i, j] = i;
                }
            }

            for (k = 0; k < CityList.Count; ++k)
            {
                for (i = 0; i < CityList.Count; ++i)
                {
                    for (j = 0; j < CityList.Count; ++j)
                    {
                        if (CityList[i] != null && CityList[j] != null && i != j)
                        {
                            if (Shortest[i, k] + Shortest[k, j] < Shortest[i, j])
                            {
                                Shortest[i, j] = Shortest[i, k] + Shortest[k, j];
                                _arrPath[i, j] = _arrPath[k, j];
                            }
                        }
                    }
                }
            }

            for (i = 0; i < CityList.Count; i++)
            {
                for (j = 0; j < CityList.Count; j++)
                {
                    if (Shortest[i, j] != Constants.InitialValue)
                    {
                        Shortest[i,j] = Shortest[i,j] - (CityList[i].TransitFees+CityList[j].TransitFees)/2;
                    }

                    if (i != j && (CityList[i] != null) && (CityList[j] != null))
                    {
                        Path[i,j] = new List<int>();
                        k = j;
                        while(k!=i)
                        {
                           Path[i, j].Add(k); 
                           k = _arrPath[i,k];
                        }
                        Path[i, j].Add(k); 
                        Path[i, j].Reverse();
                    }
                }
            }

        }

        //通过游客给的需求来给出方案
        public void generatePlan(Plan GuidePlan)
        {
        }
    }
}
