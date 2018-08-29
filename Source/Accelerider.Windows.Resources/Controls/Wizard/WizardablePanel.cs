using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Accelerider.Windows.Resources.Controls.Wizard
{
    [ContentProperty(nameof(WizardViews))]
    public class WizardablePanel : UserControl
    {
        //static WizardablePanel()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(WizardablePanel), new FrameworkPropertyMetadata(typeof(WizardablePanel)));
        //}

        public static readonly DependencyProperty WizardViewsProperty = DependencyProperty.Register(
            "WizardViews", typeof(WizardableViewCollection), typeof(WizardablePanel), new PropertyMetadata(null));

        public WizardablePanel()
        {
            WizardViews = new WizardableViewCollection(this);
            Loaded += (sender, e) => Content = WizardViews?.FirstOrDefault();
        }

        public WizardableViewCollection WizardViews
        {
            get { return (WizardableViewCollection)GetValue(WizardViewsProperty); }
            set { SetValue(WizardViewsProperty, value); }
        }
    }
}
