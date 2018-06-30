using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accelerider.Windows.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Tests
{
    [TestClass()]
    public class ObservableSortedCollectionTests
    {
        [TestMethod()]
        public void ObservableSortedCollectionTest()
        {

        }

        [TestMethod()]
        public void ObservableSortedCollectionTest1()
        {

        }

        [TestMethod()]
        public void UpdateTest()
        {
            var temp = new[] { "gsa", "dfb", "sar", "ac", "fgfbg", "hek", "fsaw", "l;fg", "" };
            var temp2 = new[] { "aba", "zzz", "aac", "aaa ", "aca", "abb", "aab", "abc", "acb", "acc", "jhgj"};


            var testObject = new ObservableSortedCollection<string>(temp, Comparer<string>.Default.Compare);
            foreach (var item in temp2)
            {
                testObject.Add(item);
            }
        }
    }
}