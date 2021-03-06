﻿using System;
using System.Windows.Shapes;

namespace TravelAgency.Graph
{
    /// <summary>
    ///     边
    /// </summary>
    [Serializable]
    public class Edge
    {
        private City start;
        private City end;
        private int value;

        public Edge(City start, City end, int edge)
        {
            this.start = start;
            this.end = end;
            value = edge;
        }

        /// <summary>
        ///     起点
        /// </summary>
        public City Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        ///     终点
        /// </summary>
        public City End
        {
            get { return end; }
            set { end = value; }
        }

        /// <summary>
        ///     边的值
        /// </summary>
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #region 绘图接口

        /// <summary>
        ///     绘制边的图形元素
        /// </summary>
        [NonSerialized] public Line line;

        public double GetStartX()
        {
            return start.GetCenterX();
        }

        public double GetStartY()
        {
            return start.GetCenterY();
        }

        public double GetEndX()
        {
            return end.GetCenterX();
        }

        public double GetEndY()
        {
            return end.GetCenterY();
        }

        #endregion
    }
}