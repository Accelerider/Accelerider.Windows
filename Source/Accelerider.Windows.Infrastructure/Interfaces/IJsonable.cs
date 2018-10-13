namespace Accelerider.Windows.Infrastructure
{
    public interface IJsonable<out T>
    {
        string ToJson();

        T FromJson(string json);
    }
}
