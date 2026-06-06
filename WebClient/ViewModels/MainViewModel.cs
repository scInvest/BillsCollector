using CostAnalizerApp;
using System.Diagnostics.Metrics;
using WebClient.Components.Pages;
using WebClient.Components.UIComponents.DataPicker;
using WebClient.ViewModels;

namespace WebClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public DataPickerViewModel DataPickerViewModel { get; set; }
        public Func<CostAnalizerApplication> CostAnalizerApp { get; }
        public Counter MainPagelo => (base.Component as Counter)!;

        public MainViewModel(
            System.Func<Microsoft.AspNetCore.Components.ComponentBase> getComponent,
            System.Func<Microsoft.AspNetCore.Components.ComponentBase> getDatPicker,
            System.Func<CostAnalizerApplication> mainApp)
            : base(getComponent)
        {
            DataPickerViewModel = new DataPickerViewModel(getDatPicker, mainApp);
            CostAnalizerApp = mainApp;
        }
    }
}