using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Accelerider.Windows
{
    public static class WindowController
    {
        private static readonly Dictionary<Type, Window> _windowDictionary = new Dictionary<Type, Window>();

        public static void Register<T>(T window) where T : Window
        {
            //if (_windowDictionary.ContainsKey(typeof(T))) throw new ArgumentException();

            _windowDictionary[typeof(T)] = window;
        }

        public static void Close<T>() where T : Window
        {
            var type = typeof(T);
            var window = _windowDictionary[typeof(T)];
            _windowDictionary.Remove(type);
            window?.Close();
        }

        public static void Switch<TClose, TShow>() 
            where TClose : Window
            where TShow : Window, new()
        {
            new TShow().Show();
            Close<TClose>();
        }
    }
}
