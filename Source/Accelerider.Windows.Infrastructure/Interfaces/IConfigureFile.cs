using System;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public class ValueChangedEventArgs : EventArgs
    {
        public string KeyName { get; }

        public ValueChangedEventArgs(string keyName) => KeyName = keyName;
    }

    public interface IConfigureFile
    {
        event EventHandler<ValueChangedEventArgs> ValueChanged;

        bool Contains(string key);

        T GetValue<T>(string key);

        IConfigureFile SetValue<T>(string key, T value);

        IConfigureFile Load(string filePath = null);

        IConfigureFile Clear();

        void Delete();
    }
}
