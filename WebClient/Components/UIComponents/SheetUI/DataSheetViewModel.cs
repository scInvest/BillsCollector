using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.DataStructures.Geometry;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.SheetUI
{
    public class DataSheetViewModel : ViewModelBase
    {

        public DataSheetViewModel(Func<ComponentBase> getComponent, UniverSheet.UniverDataSheet univerSheet)
            : base(getComponent)
        {
            UniverSheet = univerSheet;
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
                    value.Init(this, UniverSheet);
                    OnPropertyChanged();
                    Refresh();
                    this.EndUpdate();
                }
            }
        }

        public UniverSheet.UniverDataSheet UniverSheet { get; }
    }
}
