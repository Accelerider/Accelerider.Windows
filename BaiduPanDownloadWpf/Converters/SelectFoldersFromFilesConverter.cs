using BaiduPanDownloadWpf.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf.Converters
{
    class SelectFoldersFromFilesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var files = value as IEnumerable<NetDiskFileNodeViewModel>;
            if (files == null || !files.Any()) return Binding.DoNothing;
            var temp = new ObservableCollection<NetDiskFileNodeViewModel>(from item in files where item.FileType == FileTypeEnum.FolderType select item);
            return temp.Count == 0 ? null : temp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
