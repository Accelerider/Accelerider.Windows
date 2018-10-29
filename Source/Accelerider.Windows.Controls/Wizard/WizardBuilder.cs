using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Accelerider.Windows.Controls.Wizard
{
    public class WizardBuilder
    {
        private readonly List<Func<FrameworkElement>> _creators = new List<Func<FrameworkElement>>();

        public WizardBuilder Next<T>() where T : FrameworkElement, new()
        {
            _creators.Add(() => new T());
            return this;
        }

        public WizardBuilder Next<T>(Func<T> creator) where T : FrameworkElement, new()
        {
            _creators.Add(creator);
            return this;
        }

        public WizardablePanel Build(Style style = null)
        {
            var host = new WizardablePanel();
            foreach (var element in _creators.Select(item => item()))
            {
                if (style != null) element.Style = style;
                host.WizardViews.Add(element);
            }

            return host;
        }
    }
}
