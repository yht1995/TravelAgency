using TravelAgency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TravelAgency.Graph;

namespace UnitTest
{
    
    
    /// <summary>
    ///这是 AgencyTest 的测试类，旨在
    ///包含所有 AgencyTest 单元测试
    ///</summary>
    [TestClass()]
    public class AgencyTest
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


        /// <summary>
        ///EnumeratePath 的测试
        ///</summary>
        [TestMethod()]
        public void EnumeratePathTest()
        {
            string path = "D:\\document\\大二上\\数据结构\\第四次大作业\\map.map"; // TODO: 初始化为适当的值
            AdjacencyGraph map = new AdjacencyGraph();
            FileIo.ImportFormBinMap(path, map);
            Agency target = new Agency(); // TODO: 初始化为适当的值
            City start = map.VertexList[0]; // TODO: 初始化为适当的值
            int count = 3; // TODO: 初始化为适当的值
            target.EnumeratePath(map, start, count);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///PrepareData 的测试
        ///</summary>
        //[TestMethod()]
        public void PrepareDataTest()
        {
            string path = "D:\\document\\大二上\\数据结构\\第四次大作业\\map.map"; // TODO: 初始化为适当的值
            AdjacencyGraph map = new AdjacencyGraph();
            FileIo.ImportFormBinMap(path, map);
            Agency target = new Agency(); // TODO: 初始化为适当的值
            target.PrepareData(map);
            Assert.Inconclusive("无法验证不返回值的方法。");
        }

        /// <summary>
        ///BestPath 的测试
        ///</summary>
        //[TestMethod()]
        public void BestPathTest()
        {
            string path = "D:\\document\\大二上\\数据结构\\第四次大作业\\map.map"; // TODO: 初始化为适当的值
            AdjacencyGraph map = new AdjacencyGraph();
            FileIo.ImportFormBinMap(path, map);
            Agency target = new Agency(); // TODO: 初始化为适当的值
            City city = map.VertexList[20]; // TODO: 初始化为适当的值
            int[] rate = new int[30] {0,0,1,0,0,0,0,0,0,0,
                                    0,0,0,0,2,0,0,0,0,0,
                                    0,0,0,0,4,0,0,0,0,0}; // TODO: 初始化为适当的值
            int cityCount = 3; // TODO: 初始化为适当的值
            int transitFee = 12000; // TODO: 初始化为适当的值
            Path actual = target.BestPath(city, rate, cityCount, transitFee);
            Assert.Inconclusive("Lbest=" + actual.lvaule);
        }
    }
}
