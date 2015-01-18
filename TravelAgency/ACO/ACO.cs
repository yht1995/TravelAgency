namespace TravelAgency.ACO
{
    // ReSharper disable once InconsistentNaming
    public class ACO
    {
        private readonly Ant[] antArray;
        public readonly Ant bestAnt;
        private readonly Constants constants;
        private readonly Guide guide;
        private readonly double[,] trialMartix;

        public ACO(Guide guide, Request request,Constants constants)
        {
            trialMartix = new double[constants.maxCityNum, constants.maxCityNum];
            antArray = new Ant[constants.AntCount];
            for (var j = 0; j < constants.AntCount; j++)
            {
                antArray[j] = new Ant(guide, request, constants, ref trialMartix);
            }
            this.guide = guide;
            bestAnt = new Ant(guide, request, constants, ref trialMartix);
            this.constants = constants;
        }

        public void InitData()
        {
            bestAnt.Initial();
            bestAnt.lValue = 0.0;
            for (var i = 0; i < guide.CityList.Count; i++)
            {
                for (var j = 0; j < guide.CityList.Count; j++)
                {
                    trialMartix[i, j] = 1.0;
                }
            }
        }

        private void UpdateTrial()
        {
            var dbTempAry = new double[guide.CityList.Count, guide.CityList.Count];

            for (var i = 0; i < guide.CityList.Count; i++)
            {
                for (var j = 0; j < guide.CityList.Count; j++)
                {
                    dbTempAry[i, j] = 0.0;
                }
            }

            for (var i = 0; i < constants.AntCount; i++)
            {
                for (var j = 1; j < antArray[i].movedCityCount; j++)
                {
                    var m = antArray[i].path[j];
                    var n = antArray[i].path[j - 1];
                    dbTempAry[n, m] += constants.dbq*antArray[i].lValue/1000;
                    dbTempAry[m, n] = dbTempAry[n, m];
                }
            }

            for (var i = 0; i < guide.CityList.Count; i++)
            {
                for (var j = 0; j < guide.CityList.Count; j++)
                {
                    trialMartix[i, j] = trialMartix[i, j]*constants.rou + dbTempAry[i, j];
                }
            }
        }

        public void Search()
        {
            for (var i = 0; i < constants.ItCount; i++)
            {
                for (var j = 0; j < constants.AntCount; j++)
                {
                    antArray[j].Search();
                    if (!(antArray[j].lValue > bestAnt.lValue)) continue;
                    bestAnt.lValue = antArray[j].lValue;

                    for (var ii = 0; ii < antArray[j].movedCityCount; ii++)
                    {
                        bestAnt.path[ii] = antArray[j].path[ii];
                    }
                    bestAnt.tagList.Clear();
                    for (var jj = 0; jj < antArray[j].tagList.Count; jj++)
                    {
                        bestAnt.tagList.Add(antArray[j].tagList[jj]);
                    }

                    bestAnt.dbCost = antArray[j].dbCost;
                    bestAnt.realMovedCount = antArray[j].realMovedCount;
                    bestAnt.movedCityCount = antArray[j].movedCityCount;
                }
            }
            UpdateTrial();
        }
    }
}