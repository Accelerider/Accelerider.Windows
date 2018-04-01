using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accelerider.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace Accelerider.Windows.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        [TestMethod()]
        public void MainWindowViewModelTest()
        {
            var iTest = Mock.Create<ITest>();
            var test = Mock.Create<Test>(iTest);

            Mock.Arrange(() => iTest.TestName).Returns("FUCK!");

            Assert.AreEqual(new Test(iTest).TestName, "FUCK!");
            Assert.AreEqual(test.TestName, "FUCK!");
        }

        [TestMethod()]
        public void OnLoadedTest()
        {

        }
    }

    public interface ITest
    {
        string TestName { get; }
    }

    public class Test
    {
        private readonly ITest _test;

        public string TestName => _test.TestName;

        public Test(ITest test)
        {
            _test = test;
        }
    }
}