namespace Accelerider.Windows.Infrastructure
{
    public interface IJsonable
    {
        string ToJson();

        void FromJson(string json);
    }
}
