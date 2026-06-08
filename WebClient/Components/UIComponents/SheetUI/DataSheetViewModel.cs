using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.DataStructures.Geometry;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.SheetUI
{
    public class DataSheetViewModel : ViewModelBase
    {
        private readonly Datasheet datasheet;
        private readonly Sheet _sheet;

        public Sheet SheetData => _sheet;
        public DataSheetViewModel(Func<ComponentBase> getComponent, Datasheet datasheet, Sheet sheet)
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

        private IDataSheetLogic? _dataSheetLogic;
        public IDataSheetLogic? DataSheetLogic
        {
            get => _dataSheetLogic;
            set
            {
                if (_dataSheetLogic != value)
                {
                    this.BeginUpdate();
                    this.Headers = value!.Headers;
                    _dataSheetLogic = value;
                    value.Init(this, datasheet, _sheet);
                    OnPropertyChanged();
                    Refresh();
                    this.EndUpdate();
                }
            }
        }
    }
}
