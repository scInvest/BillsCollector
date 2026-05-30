using WebClient.ViewModels;
using WebClient.Components.UIComponents.DataPicker;

namespace WebClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public DataPickerViewModel DataPickerViewModel { get; set; }

        public MainViewModel(Microsoft.AspNetCore.Components.ComponentBase component) : base(component)
        {
            DataPickerViewModel = new DataPickerViewModel(component);
        }
    }
}