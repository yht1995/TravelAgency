using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgency
{
    /// <summary>
    /// 图类
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    /// <typeparam name="H">边类型</typeparam>
    public class Graph<T,H> : IEnumerable
    {
        private NodeList<T> nodeSet;

        public Graph() : this(null) { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null)
                this.nodeSet = new NodeList<T>();
            else
                this.nodeSet = nodeSet;
        }

        public IEnumerator GetEnumerator()
        {
            return nodeSet.GetEnumerator();
        }

        public void AddNode(GraphNode<T,H> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
        }

        public void AddNode(T value)
        {
            // adds a node to the graph
            nodeSet.Add(new GraphNode<T,H>(value));
        }
        /// <summary>
        /// 添加单向边
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="edge">边</param>
        public void AddDirectedEdge(GraphNode<T,H> from, GraphNode<T,H> to, H edge)
        {
            from.Neighbors.Add(to);
            from.Edges.Add(edge);
        }

        /// <summary>
        /// 添加双向边
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="edge">边</param>
        public void AddUndirectedEdge(GraphNode<T,H> from, GraphNode<T,H> to, H edge)
        {
            from.Neighbors.Add(to);
            from.Edges.Add(edge);

            to.Neighbors.Add(from);
            to.Edges.Add(edge);
        }

        public bool Contains(T value)
        {
            return nodeSet.FindByValue(value) != null;
        }
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="value">节点</param>
        /// <returns></returns>
        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            GraphNode<T,H> nodeToRemove = (GraphNode<T,H>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T,H> gnode in nodeSet)
            {
                int index = gnode.Neighbors.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                    gnode.Edges.RemoveAt(index);
                }
            }

            return true;
        }

        public NodeList<T> Nodes
        {
            get
            {
                return nodeSet;
            }
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }
    }
}
