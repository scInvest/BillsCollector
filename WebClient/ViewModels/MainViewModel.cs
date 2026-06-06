using CostAnalizerApp;
using System.Diagnostics.Metrics;
using WebClient.Components.Pages;
using WebClient.Components.UIComponents.DataPicker;
using WebClient.ViewModels;

namespace WebClient.ViewModels
{
    record ExistingSheets
    {
        public SharedCode.SpendingFileType spendingFileType { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        private HashSet<ExistingSheets> sheetPendingToCreate = new();
        private List<ExistingSheets> sheets = new List<ExistingSheets>();

        public DataPickerViewModel DataPickerViewModel { get; set; }
        public Func<CostAnalizerApplication> CostAnalizerApp { get; }
        public Counter MainPage => (base.Component as Counter)!;

        public MainViewModel(
            System.Func<Microsoft.AspNetCore.Components.ComponentBase> getComponent,
            System.Func<Microsoft.AspNetCore.Components.ComponentBase> getDatPicker,
            System.Func<CostAnalizerApplication> mainApp)
            : base(getComponent)
        {
            DataPickerViewModel = new DataPickerViewModel(getDatPicker, mainApp);
            DataPickerViewModel.UserInputBeforeDataAdded += DataPickerViewModel_UserInputBeforeDataAdded;
            CostAnalizerApp = mainApp;
            this.MainPage.BeforeSheetAdded += MainPage_BeforeSheetAdded;
            this.MainPage.SheetAdded += MainPage_SheetAdded; ;
        }

        private void MainPage_SheetAdded(string obj)
        {

        }

        private void MainPage_BeforeSheetAdded(string id)
        {
        }

        private void DataPickerViewModel_UserInputBeforeDataAdded(SharedCode.SpendingFileType type)
        {
            var id = Guid.NewGuid().ToString();
            var title = type.ToString();
            sheetPendingToCreate.Add(new ExistingSheets { Id = id, spendingFileType = type, Title = title });

            MainPage.AddSheet(Counter.SheetGroup.Data, title, id);

            base.Refresh();
        }

        public void UserInput_AddAnalyticAdded()
        {

        }

        public void UserInput_AddDataClicked()
        {
        }
    }
}