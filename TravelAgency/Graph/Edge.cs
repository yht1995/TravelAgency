using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Shapes;

namespace TravelAgency
{
    /// <summary>
    /// 边
    /// </summary>
    [Serializable]
    public class Edge
    {
        private int value;
        private City start;
        private City end;

        /// <summary>
        /// 起点
        /// </summary>
        public City Start
        {
            get { return start; }
            set { start = value; }
        }
        /// <summary>
        /// 终点
        /// </summary>
        public City End
        {
            get { return end; }
            set { end = value; }
        }
        /// <summary>
        /// 边的值
        /// </summary>
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public Edge(City start, City end, int edge)
        {
            this.start = start;
            this.end = end;
            this.value = edge;
        }
        #region 绘图接口
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
        /// <summary>
        /// 绘制边的图形元素
        /// </summary>
        [NonSerialized]
        public Line line;
    }
}
