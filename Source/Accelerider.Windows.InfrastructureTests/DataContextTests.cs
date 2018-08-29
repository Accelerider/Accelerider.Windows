using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mvvm;

namespace Accelerider.Windows.InfrastructureTests
{
    [TestClass()]
    public class DataContextTests : BindableBase
    {
        public static readonly DataContext DataContext = new DataContext();

        public string[] TestStrings =
        {
            "",
            "`1234567890-=~!@#$%^&*()_+",
            @"qwertyuiop[]\asdfghjkl;'zxcvbnm,./",
            null
        };

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ExportTest()
        {
            var dataContext = new DataContext();
            var viewModelExport = new ViewModelExport(dataContext);
            var viewModelExport2 = new ViewModelExport(dataContext);
        }

        [TestMethod()]
        public void ImportTest()
        {
            var dataContext = new DataContext();
            var viewModelImport = new ViewModelImport(dataContext);
            var viewModelExport = new ViewModelExport(dataContext);
            List<ViewModelImport> viewModelImports = new List<ViewModelImport>
            {
                viewModelImport,
                new ViewModelImport(dataContext),
                new ViewModelImport(dataContext),
                new ViewModelImport(dataContext),
                new ViewModelImport(dataContext),
                new ViewModelImport(dataContext),
            };

            var count = 0;

            viewModelImports.ForEach(item => item.PropertyChanged += (sender, args) =>
            {
                Assert.AreEqual(nameof(ViewModelImport.Name), args.PropertyName);
                count++;
            });


            for (int i = 0; i < TestStrings.Length; i++)
            {
                viewModelExport.Name = TestStrings[i];

                Assert.AreEqual((i + 1) * viewModelImports.Count, count);
                foreach (var import in viewModelImports)
                {
                    Assert.AreEqual(TestStrings[i], import.Name);
                }
            }
        }
    }

    public class ViewModelExport : BindableBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ViewModelExport(DataContext dataContext)
        {
            dataContext.Export(() => Name);
        }
    }

    public class ViewModelImport : BindableBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ViewModelImport(DataContext dataContext)
        {
            dataContext.Import(() => Name);
        }
    }
}