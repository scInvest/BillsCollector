using CostAnalizerApp;
using SharedCode;
using WebClient.Components.UIComponents.ChatAgent;
using WebClient.Components.Pages;
using WebClient.Components.UIComponents.DataPicker;
using WebClient.Components.UIComponents.SheetUI;

namespace WebClient.ViewModels
{
    class ExistingSheetsCreated : ExistingSheets
    {
        public DataSheetComponent DataSheet { get; set; }

        // Konstruktor przyjmujący instancję bazową (template) i opcjonalnie przypisujący komponent DataSheet.
        public ExistingSheetsCreated(ExistingSheets template, DataSheetComponent dataSheet)
        {
            if (template != null)
            {
                this.SpendingFileType = template.SpendingFileType;
                this.Title = template.Title;
                this.Id = template.Id;
                this.DataSheet = dataSheet;
                this.Logic = template.Logic;
            }

            this.DataSheet = dataSheet;
        }
    }

    class ExistingSheets
    {
        public SharedCode.SpendingFileType? SpendingFileType { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public Counter.SheetGroup Group { get; set; }
        public IDataSheetLogic Logic { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        private Dictionary<string, ExistingSheets> sheetPendingToCreate = new();
        private List<ExistingSheetsCreated> sheets = new List<ExistingSheetsCreated>();
        private Func<CostAnalizerApplication> costAnalizerApp;

        public ChatAgentViewModel ChatAgentViewModel { get; set; }
        public DataPickerViewModel DataPickerViewModel { get; set; }
        public CostAnalizerApplication CostAnalizerApp => costAnalizerApp();
        public Counter MainPage => (base.Component as Counter)!;

        public MainViewModel(
            System.Func<Microsoft.AspNetCore.Components.ComponentBase> getComponent,
            System.Func<Microsoft.AspNetCore.Components.ComponentBase> getDatPicker,
            System.Func<CostAnalizerApplication> mainApp)
            : base(getComponent)
        {
            ChatAgentViewModel = new ChatAgentViewModel(getComponent);
            DataPickerViewModel = new DataPickerViewModel(getDatPicker, mainApp);
            DataPickerViewModel.UserInputBeforeDataAdded += DataPickerViewModel_UserInputBeforeDataAdded;
            DataPickerViewModel.UserInputAfterDataAdded += DataPickerViewModel_UserInputAfterDataAdded;
            costAnalizerApp = mainApp;
            this.MainPage.BeforeSheetAdded += MainPage_BeforeSheetAdded;
            this.MainPage.SheetAdded += MainPage_SheetAdded; ;
        }

        private void DataPickerViewModel_UserInputAfterDataAdded(SharedCode.SpendingFileType type)
        {
            if (this.sheets.Count == 0 && this.sheetPendingToCreate.Count == 0)
            {
                var titleAnalitic = "Wszystko";
                var sheetAnaliticId = Guid.NewGuid().ToString();
                var sheetAnaliticKey = new ExistingSheets
                {
                    Id = sheetAnaliticId,
                    SpendingFileType = null,
                    Title = titleAnalitic,
                    Group = Counter.SheetGroup.Analitical,
                    Logic = new ShopProductsDataSheetLogic(type, this.CostAnalizerApp)
                };
                sheetPendingToCreate.Add(sheetAnaliticId, sheetAnaliticKey);
                MainPage.AddSheet(Counter.SheetGroup.Analitical, titleAnalitic, sheetAnaliticId);
            }

            var id = Guid.NewGuid().ToString();
            var title = type.ToString();
            var sheetKey = new ExistingSheets
            {
                Id = id,
                SpendingFileType = type,
                Title = title,
                Group = Counter.SheetGroup.Data,
                                    Logic = new ShopProductsDataSheetLogic(type, this.CostAnalizerApp)
            };
            sheetPendingToCreate.Add(id, sheetKey);

            RemoveExisting(type);
            MainPage.AddSheet(Counter.SheetGroup.Data, title, id);

            base.Refresh();
        }

        private void RemoveExisting(SpendingFileType type)
        {
            if (this.sheets.Any(X => X.SpendingFileType == type))
            {
                var items = this.sheets.Where(X => X.SpendingFileType == type);
                foreach (var item in items)
                {
                    this.MainPage.RemoveSheet(item.Id);
                }
            }
        }

        private void MainPage_SheetAdded(string id, DataSheetComponent sheetComponent)
        {
            if (sheetPendingToCreate.ContainsKey(id))
            {
                var toBeAdded = sheetPendingToCreate[id];
                sheetPendingToCreate.Remove(id);

                var sheet = new ExistingSheetsCreated(toBeAdded, sheetComponent);
                sheetComponent.ViewModel.DataSheetLogic = toBeAdded.Logic;
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

        public Result UserInput_SheetDeleted((string Key, string Title) sheetData)
        {
            var fileType = sheets.Find(x => x.Id == sheetData.Key).SpendingFileType;

            sheets.RemoveAll(x => x.Id == sheetData.Key);
            if (fileType != null)
            {
                this.CostAnalizerApp.RemoveData((SpendingFileType)fileType);
            }
            this.Refresh();
            return Result.Success();
        }
    }
}