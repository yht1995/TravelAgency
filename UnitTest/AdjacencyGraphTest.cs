using TravelAgency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    
    
    /// <summary>
    ///这是 AdjacencyGraphTest 的测试类，旨在
    ///包含所有 AdjacencyGraphTest 单元测试
    ///</summary>
    [TestClass()]
    public class AdjacencyGraphTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void AddVertexEdgeTest()
        {
            AdjacencyGraph map = new AdjacencyGraph();
            City city1 = new City("北京", "东经 12.1", "北纬 23.2", "122");
            City city2 = new City("上海", "东经 12.1", "北纬 23.2", "122");
            map.AddVertex(city1);
            map.AddVertex(city2);
            map.AddEdge(city1, city2, 122);
            Assert.AreEqual(map.GetEdge(city1,city2), 122);
        }

        /// <summary>
        ///RemoveEdge 的测试
        ///</summary>

        [TestMethod()]
        public void RemoveEdgeTest()
        {
            AdjacencyGraph map = new AdjacencyGraph();
            City city1 = new City("北京", "东经 12.1", "北纬 23.2", "122");
            City city2 = new City("上海", "东经 12.1", "北纬 23.2", "122");
            map.AddVertex(city1);
            map.AddVertex(city2);
            map.AddEdge(city1, city2, 122);
            Assert.AreEqual(map.GetEdge(city1, city2), 122);
            map.RemoveEdge(city1, city2);
            Assert.AreEqual(map.GetEdge(city1, city2), -1);
        }

        /// <summary>
        ///RemoveVertex 的测试
        ///</summary>
        [TestMethod()]
        public void RemoveVertexTest()
        {
            AdjacencyGraph map = new AdjacencyGraph();
            City city1 = new City("北京", "东经 12.1", "北纬 23.2", "122");
            City city2 = new City("上海", "东经 12.1", "北纬 23.2", "122");
            map.AddVertex(city1);
            map.AddVertex(city2);
            map.AddEdge(city1, city2, 122);
            Assert.AreEqual(map.GetEdge(city1, city2), 122);
            map.RemoveVertex(city1);
            Assert.AreEqual(map.VertexList.Count, 1);
        }
    }
}
