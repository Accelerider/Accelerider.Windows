using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Core;
using Accelerider.Windows.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using Accelerider.Windows.Converters;

namespace Accelerider.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FolderLocationBar.xaml
    /// </summary>
    public partial class FolderLocationBar : INotifyPropertyChanged
    {
        private ICommand _locateFolderCommand;
        private ICommand _locateRootCommand;
        private ObservableCollection<ITreeNodeAsync<INetDiskFile>> _folderChain;
        private ObservableCollection<ITreeNodeAsync<INetDiskFile>> _foldedFolders;
        private IList<double> _listBoxActualWidthStorage = new List<double>();

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty CurrentFolderProperty = DependencyProperty.Register(
                "CurrentFolder",
                typeof(ITreeNodeAsync<INetDiskFile>),
                typeof(FolderLocationBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CurrentFolderChanged));

        public static readonly DependencyProperty DeltaWidthProperty = DependencyProperty.Register(
            "DeltaWidth",
            typeof(double),
            typeof(FolderLocationBar),
            new PropertyMetadata(0.0, DeltaWidthChanged));

        public ITreeNodeAsync<INetDiskFile> CurrentFolder
        {
            get => (ITreeNodeAsync<INetDiskFile>)GetValue(CurrentFolderProperty);
            set => SetValue(CurrentFolderProperty, value);
        }
        public double DeltaWidth
        {
            get => (double)GetValue(DeltaWidthProperty);
            set => SetValue(DeltaWidthProperty, value);
        }

        public ObservableCollection<ITreeNodeAsync<INetDiskFile>> FolderChain
        {
            get => _folderChain;
            set => SetProperty(ref _folderChain, value);
        }
        public ObservableCollection<ITreeNodeAsync<INetDiskFile>> FoldedFolders
        {
            get => _foldedFolders;
            set => SetProperty(ref _foldedFolders, value);
        }

        public ICommand LocateFolderCommand
        {
            get => _locateFolderCommand;
            set => SetProperty(ref _locateFolderCommand, value);
        }
        public ICommand LocateRootCommand
        {
            get => _locateRootCommand;
            set => SetProperty(ref _locateRootCommand, value);
        }


        public FolderLocationBar()
        {
            LocateFolderCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(
                fileNode => CurrentFolder = fileNode,
                fileNode => CurrentFolder != fileNode);
            LocateRootCommand = new RelayCommand(
                () => CurrentFolder = CurrentFolder.Root,
                () => CurrentFolder != CurrentFolder?.Root);

            InitializeComponent();

            var multiBinding = new MultiBinding
            {
                Bindings =
                {
                    new Binding
                    {
                        Path = new PropertyPath("ActualWidth"),
                        Source = PART_ListBox
                    },
                    new Binding
                    {
                        Path = new PropertyPath("ActualWidth"),
                        Source = this
                    }
                },
                Converter = new MinusConverter(),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, DeltaWidthProperty, multiBinding);
        }


        private static void CurrentFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FolderLocationBar)d;
            var folderChain = new List<ITreeNodeAsync<INetDiskFile>>();
            var temp = control.CurrentFolder;
            do
            {
                folderChain.Add(temp);
            } while ((temp = temp.Parent) != null);
            folderChain.Reverse();
            folderChain.RemoveAt(0);
            control.FolderChain = new ObservableCollection<ITreeNodeAsync<INetDiskFile>>(folderChain);
        }

        private static void DeltaWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FolderLocationBar)d;
            // 1. 检查control.PART_ListBox.ActualWidth有无变化，变化则记录新值（添加至_listBoxActualWidthStorage末尾）
            var deltaWidth = control.PART_ListBox.ActualWidth - control.ActualWidth;
            if (deltaWidth <= 0) return;
            // 2. 找到control.PART_ListBox.ActualWidth - _listBoxActualWidthStorage[i] <= control.ActualWidth
            // 3. Moves i item(s) from FolderChain to FoldedFolders

            //Debug.WriteLine($"{control.PART_ListBox.ActualWidth} == {control.FolderChain?.Aggregate(0, (sum, item) => sum + item.Content.FilePath.FileName.Length)}");
            //if (!control.FolderChain.Any()) return;
            //control.FoldedFolders?.Insert(0, control.FolderChain[0]);
            //control.FolderChain.RemoveAt(0);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
