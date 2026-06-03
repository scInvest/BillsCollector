using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.DataStructures.Geometry;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.SheetUI
{
    public class DataSheetLogicViewModel : ViewModelBase, IDataSheetLogic
    {
        private readonly Datasheet datasheet;
        private readonly Sheet _sheet;
        public DataSheetLogicViewModel(Func<ComponentBase> getComponent, Datasheet datasheet, Sheet sheet)
            : base(getComponent)
        {   
            this.datasheet = datasheet;
            _sheet = sheet;
        }
    
        private string[] _headers;
        public string[] Headers
        {
            get => _headers;
            set
            {
                if (_headers != value)
                {
                    _headers = value;
                    OnPropertyChanged();
                    Refresh();
                }
            }
        }

        public void BillSummaryTable_CreateEmpty()
        {
            this.Headers = BillSummaryTable_Headers;
        }

        public string[] BillSummaryTable_Headers => new string[]
        {
            "",  "Rodzaj", "Źródło", "Data", "Nazwa(oryginalna)", "Nazwa",
            "Kwota łącznie", "Kwota", "Zniżka", "Przed znizka", "Ilość", "Jednostka",
            "Kategoria", "Kategoria", "Kategoria", "Tagi", "ID", "Metadane",
        };
    }
}
