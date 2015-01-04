using TravelAgency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    
    
    /// <summary>
    ///这是 LatitudeTest 的测试类，旨在
    ///包含所有 LatitudeTest 单元测试
    ///</summary>
    [TestClass()]
    public class LatitudeTest
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
        ///ToString 的测试
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Latitude target = new Latitude(); // TODO: 初始化为适当的值
            target.FromString("北纬 12.1");
            string expected = "北纬 12.10"; // TODO: 初始化为适当的值
            string actual = target.ToString();
            Assert.AreEqual(target.Value, 12.1);
            Assert.AreEqual(expected, actual);

            target.FromString("南纬 12.1");
            expected = "南纬 12.10"; // TODO: 初始化为适当的值
            actual = target.ToString();
            Assert.AreEqual(target.Value, -12.1);
            Assert.AreEqual(expected, actual);

            target.FromString("南纬25.1");
            expected = "南纬 25.10"; // TODO: 初始化为适当的值
            actual = target.ToString();
            Assert.AreEqual(target.Value, -25.1);
            Assert.AreEqual(expected, actual);
        }
    }
}
