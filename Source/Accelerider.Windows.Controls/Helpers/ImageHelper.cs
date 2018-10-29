using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Accelerider.Windows.Controls
{
    public static class ImageHelper
    {
        public static readonly DependencyProperty ImageSelectorProperty = DependencyProperty.RegisterAttached("ImageSelector", typeof(ImageSelector), typeof(ImageHelper), new PropertyMetadata(null, OnImageSelectorChanged));
        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static ImageSelector GetImageSelector(DependencyObject obj)
        {
            return (ImageSelector)obj.GetValue(ImageSelectorProperty);
        }
        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static void SetImageSelector(DependencyObject obj, ImageSelector value)
        {
            obj.SetValue(ImageSelectorProperty, value);
        }

        private static void OnImageSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageSelector = (ImageSelector)e.NewValue;
            var image = (Image)d;

            imageSelector.SetTargetControl(image);
        }
    }

    [ContentProperty(nameof(ImageSources))]
    public class ImageSelector : DependencyObject
    {
        public static readonly DependencyProperty SelectedTypeProperty = DependencyProperty.Register("SelectedType", typeof(object), typeof(ImageSelector), new PropertyMetadata(null, OnSelectedTypeChanged));
        public static readonly DependencyProperty DefaultImageProperty = DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(ImageSelector), new PropertyMetadata(null));
        public static readonly DependencyProperty ImageSourcesProperty = DependencyProperty.Register("ImageSources", typeof(ImageSourceCollection), typeof(ImageSelector), new PropertyMetadata(null));

        private Image _target;

        public object SelectedType
        {
            get => GetValue(SelectedTypeProperty);
            set => SetValue(SelectedTypeProperty, value);
        }

        public ImageSource DefaultImage
        {
            get => (ImageSource)GetValue(DefaultImageProperty);
            set => SetValue(DefaultImageProperty, value);
        }

        public ImageSourceCollection ImageSources
        {
            get => (ImageSourceCollection)GetValue(ImageSourcesProperty);
            set => SetValue(ImageSourcesProperty, value);
        }


        public ImageSelector()
        {
            ImageSources = new ImageSourceCollection();
        }


        internal void SetTargetControl(Image targetControl) => _target = targetControl;

        private static void OnSelectedTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ImageSelector imageSelector) || imageSelector._target == null) return;

            var imageSource = imageSelector.ImageSources.FirstOrDefault(item => EqualityComparer<object>.Default.Equals(item.Type, e.NewValue));
            imageSelector._target.Source = imageSource?.Source ?? imageSelector.DefaultImage;
        }
    }

    public class ImageSourceCollection : ObservableCollection<ImageSourceItem> { }

    public class ImageSourceItem : DependencyObject
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageSourceItem), new PropertyMetadata(null));

        public object Type { get; set; }

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
    }
}
