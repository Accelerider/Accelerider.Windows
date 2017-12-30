namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IConfigureFile
    {
        string this[string key] { get; set; }

        bool Contains(string key);

        T GetValue<T>(string key);

        void Delete();

        void Load(string filePath = null);

        void Save();
    }
}
