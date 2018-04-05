using System;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public delegate void ValueChangedEventHandler(IConfigureFile sender, string keyName);

    public interface IConfigureFile
    {
        event ValueChangedEventHandler ValueChanged;

        bool Contains(string key);

        T GetValue<T>(string key);

        IConfigureFile SetValue<T>(string key, T value);

        IConfigureFile Load(string filePath = null);

        IConfigureFile Clear();

        void Delete();
    }
}
