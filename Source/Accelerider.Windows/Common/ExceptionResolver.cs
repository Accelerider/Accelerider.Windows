using Microsoft.Practices.Unity;
using System;
using System.Windows.Threading;

namespace Accelerider.Windows.Common
{
    public class ExceptionResolver
    {
        private IUnityContainer _container;


        public ExceptionResolver(IUnityContainer container)
        {
            _container = container;
        }

        public void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            
        }

        public void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            
        }
    }
}
