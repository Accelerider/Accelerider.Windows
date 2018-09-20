namespace Accelerider.Windows.Infrastructure.ViewModels
{
    public interface IAwareViewLoadedAndUnloaded
    {
        void OnLoaded();

        void OnUnloaded();
    }

    public interface IAwareViewLoadedAndUnloaded<in TView>
    {
        void OnLoaded(TView view);

        void OnUnloaded(TView view);
    }
}
