using System;
using System.Windows.Threading;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows
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
