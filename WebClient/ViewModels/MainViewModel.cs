using CostAnalizerApp;
using System.Diagnostics.Metrics;
using WebClient.Components.Pages;
using WebClient.Components.UIComponents.DataPicker;
using WebClient.Components.UIComponents.SheetUI;
using WebClient.ViewModels;

namespace WebClient.ViewModels
{
    class ExistingSheetsCreated : ExistingSheets
    {
        public DataSheetComponent DataSheet { get; set; }

        // Konstruktor przyjmujący instancję bazową (template) i opcjonalnie przypisujący komponent DataSheet.
        public ExistingSheetsCreated(ExistingSheets template, DataSheetComponent dataSheet = null)
        {
            if (template != null)
            {
                this.spendingFileType = template.spendingFileType;
                this.Title = template.Title;
                this.Id = template.Id;
            }

            this.DataSheet = dataSheet;
        }
    }

    class ExistingSheets
    {
        public SharedCode.SpendingFileType spendingFileType { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        private Dictionary<string, ExistingSheets> sheetPendingToCreate = new();
        private List<ExistingSheetsCreated> sheets = new List<ExistingSheetsCreated>();

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
            DataPickerViewModel.UserInputAfterDataAdded += DataPickerViewModel_UserInputAfterDataAdded;
            CostAnalizerApp = mainApp;
            this.MainPage.BeforeSheetAdded += MainPage_BeforeSheetAdded;
            this.MainPage.SheetAdded += MainPage_SheetAdded; ;
        }

        private void DataPickerViewModel_UserInputAfterDataAdded(SharedCode.SpendingFileType type)
        {
            var id = Guid.NewGuid().ToString();
            var title = type.ToString();
            var sheetKey = new ExistingSheets { Id = id, spendingFileType = type, Title = title };
            sheetPendingToCreate.Add(id, sheetKey);

            if (this.sheets.Any(X => X.spendingFileType == type))
            {
                var items = this.sheets.Where(X => X.spendingFileType == type);
                foreach (var item in items)
                {
                    this.MainPage.RemoveSheet(item.Id);
                }
            }

            MainPage.AddSheet(Counter.SheetGroup.Data, title, id);

            base.Refresh();
        }

        private void MainPage_SheetAdded(string id, DataSheetComponent sheetComponent)
        {
            if (sheetPendingToCreate.ContainsKey(id))
            {
                var toBeAdded = sheetPendingToCreate[id];
                sheetPendingToCreate.Remove(id);
                var sheet = new ExistingSheetsCreated(toBeAdded, sheetComponent);
                this.sheets.Add(sheet);
            }

        }

        private void MainPage_BeforeSheetAdded(string id)
        {

        }

        private void DataPickerViewModel_UserInputBeforeDataAdded(SharedCode.SpendingFileType type)
        {

        }

        public void UserInput_AddAnalyticAdded()
        {

        }

        public void UserInput_AddDataClicked()
        {
        }
    }
}