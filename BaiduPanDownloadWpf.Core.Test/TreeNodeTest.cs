using BaiduPanDownloadWpf.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.Test
{
    [TestClass]
    public class TreeNodeTest
    {
        private TreeNode<TestInfo> _tree;
        private TreeNode<TestInfo> _treeWithChildrenProvider;
        private TreeNode<TestInfo> _treeWithChildrenProviderAsync;


        public TreeNodeTest()
        {
            _tree = new TreeNode<TestInfo>(new TestInfo { Name = "ROOT", Info = "ROOT" });
            _treeWithChildrenProvider = new TreeNode<TestInfo>(new TestInfo { Name = "ROOT", Info = "ROOT" }) { ChildrenProvider = GetChildrenProvider };
            _treeWithChildrenProviderAsync = new TreeNode<TestInfo>(new TestInfo { Name = "ROOT", Info = "ROOT" }) { ChildrenProviderAsync = GetChildrenProviderAsync };
        }

        private Task<IEnumerable<TestInfo>> GetChildrenProviderAsync(TestInfo arg)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<TestInfo> GetChildrenProvider(TestInfo arg)
        {
            for (int i = 65; i < 75; i++)
            {
                var name = ((char)i).ToString();
                yield return new TestInfo { Name = name, Info = $"{arg.Info} --> {name}" };
            }
        }

        [TestMethod]
        public void ChildrenTest()
        {

        }


        [TestMethod]
        public void CheckChildrenManageWayIndependence()
        {
            var testInfo1 = new TestInfo { Name = "TEST1", Info = "TEST" };
            var testInfo2 = new TestInfo { Name = "TEST2", Info = "TEST" };
            var testInfo3 = new TestInfo { Name = "TEST3", Info = "TEST" };
            var testInfo4 = new TestInfo { Name = "TEST4", Info = "TEST" };
            var testInfo5 = new TestInfo { Name = "TEST5", Info = "TEST" };
            _tree.AddChild(testInfo1);
            _tree.AddChild(testInfo2);
            _tree.AddChild(testInfo3);
            _tree.Children.FirstOrDefault().AddChild(testInfo4);
            _tree.Children.FirstOrDefault().AddChild(testInfo5);


            try
            {
                var temp = _treeWithChildrenProvider.ChildrenAsync;
                throw new Exception();
            }
            catch (InvalidOperationException e)
            {
               
            }


            try
            {
                var temp = _treeWithChildrenProviderAsync.Children;
                throw new Exception();
            }
            catch (InvalidOperationException e)
            {

            }
        }
    }
    class TestInfo
    {
        public string Name { get; set; }
        public string Info { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Info}";
        }
    }
}
