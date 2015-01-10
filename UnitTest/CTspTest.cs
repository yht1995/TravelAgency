using TravelAgency;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    
    
    /// <summary>
    ///这是 CTspTest 的测试类，旨在
    ///包含所有 CTspTest 单元测试
    ///</summary>
    [TestClass()]
    public class CTspTest
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
        ///CTsp 构造函数 的测试
        ///</summary>
        [TestMethod()]
        public void CTspConstructorTest()
        {
            string path = "D:\\document\\大二上\\数据结构\\第四次大作业\\map.map"; // TODO: 初始化为适当的值
            AdjacencyGraph map = new AdjacencyGraph();
            FileIO.ImportFormBinMap(path, map);
            map.UpdataAdjacencyMartix();
            Guide guide = new Guide(); // TODO: 初始化为适当的值
            guide.CityList = map.VertexList;
            guide.Dict = map.Dictionary;
            guide.Expense = map.AdjacencyMartix;
            path = "D:\\document\\大二上\\数据结构\\第四次大作业\\测试数据.txt";
            List<Request> ReqList = FileIO.loadRequestFromTxt(path,ref guide); // TODO: 初始化为适当的值
            for (int i = 0; i < ReqList.Count; i++)
            {
                Plan temp = new Plan();
                CTsp tsp = new CTsp(guide, ReqList[i]);
                tsp.InitData();
                tsp.Search();
                temp.VisualizeInfo(ref temp,ref tsp,ReqList[i],ref guide);
                string s = temp.value;
            }
            Assert.Inconclusive("TODO: 实现用来验证目标的代码");
        }
    }
}
