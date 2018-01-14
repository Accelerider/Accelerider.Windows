namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IConfigureFile
    {
        object this[string key] { get; set; }

        bool Contains(string key);

        T GetValue<T>(string key);

        IConfigureFile Clear();

        void Delete();

        IConfigureFile Load(string filePath = null);
    }
}
