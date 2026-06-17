using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.DataStructures.Geometry;
using Integrations.Biedronka.BiedronkaImport.Dto;
using Microsoft.AspNetCore.Components;
using System;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.SheetUI
{
    public class DataSheetViewModel : ViewModelBase
    {

        private Func<UniverSheet.UniverDataSheet?> _univerSheetFactory;
        private bool logicInitialized = false;
        public DataSheetViewModel(Func<ComponentBase> getComponent)
            : base(getComponent)
        {

        }

        public async Task BlazorLifeCycle_InitDataSheetLogicIfNeeded()
        {
            if (IsInitNeeded())
            {
                this.BeginUpdate();
                if (UniverSheet != null)
                {
                    await DataSheetLogic.Init(this, UniverSheet);
                }
                logicInitialized = true;
                this.EndUpdate(); 
            }
        }

        private bool IsInitNeeded()
        {
            if(this.DataSheetLogic != null && UniverSheet != null && !logicInitialized)
            {
                return true;
            }
            return false;
        }

        public async Task BlazorLifeCycle_UniverSheetReady(Func<UniverSheet.UniverDataSheet?> univerSheet)
        {
            _univerSheetFactory = univerSheet;
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
                    logicInitialized = false;
                    //value.Init(this, UniverSheet);
                    OnPropertyChanged();
                    Refresh();
                    this.EndUpdate();
                }
            }
        }

        public UniverSheet.UniverDataSheet? UniverSheet => _univerSheetFactory();
    }
}
