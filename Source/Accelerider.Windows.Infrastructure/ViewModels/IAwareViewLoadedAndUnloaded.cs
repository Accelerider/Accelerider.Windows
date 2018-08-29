namespace Nutstore.Client.Wpf.Infrastructure.ViewModels
{
    public interface IAwareViewLoadedAndUnloaded
    {
        void OnLoaded(object view);

        void OnUnloaded(object view);
    }
}
