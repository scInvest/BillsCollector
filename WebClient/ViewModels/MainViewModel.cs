using WebClient.ViewModels;
using WebClient.Components.UIComponents.DataPicker;

namespace WebClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public DataPickerViewModel DataPickerViewModel { get; set; }

        public MainViewModel(System.Func<Microsoft.AspNetCore.Components.ComponentBase> getComponent)
            : base(getComponent)
        {
            DataPickerViewModel = new DataPickerViewModel(getComponent);
        }
    }
}