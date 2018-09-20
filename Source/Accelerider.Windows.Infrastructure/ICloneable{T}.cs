// ReSharper disable once CheckNamespace
namespace System
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
