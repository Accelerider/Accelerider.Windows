namespace Accelerider.Windows.Infrastructure.Mvvm
{
    public interface IViewLoadedAndUnloadedAware
    {
        void OnLoaded();

        void OnUnloaded();
    }

    public interface IViewLoadedAndUnloadedAware<in TView>
    {
        void OnLoaded(TView view);

        void OnUnloaded(TView view);
    }
}
