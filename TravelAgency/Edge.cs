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
    [Serializable]
    public class Edge
    {
        private int value;
        private City start;

        public City Start
        {
            get { return start; }
            set { start = value; }
        }
        private City end;

        public City End
        {
            get { return end; }
            set { end = value; }
        }

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
        [NonSerialized]
        public Line line;
    }
}
