namespace Accelerider.Windows.Infrastructure.Mvvm
{
    public interface ITabItemSelectionChangedAware
    {
        void OnSelected();

        void OnUnselected();
    }
}
