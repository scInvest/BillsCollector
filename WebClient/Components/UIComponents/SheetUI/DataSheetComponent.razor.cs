using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Core.Events.Selection;
using BlazorDatasheet.Core.Formats;
using BlazorDatasheet.DataStructures.Geometry;
using BlazorDatasheet.Render.Headings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using WebClient.Components.UIComponents.Extenstions;
using WebClient.Components.UIComponents.UniverSheet;
using WebClient.Components.UIServices;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.SheetUI
{

    /// <summary>
    /// Current grid frame work is weak, and in version v0.1xxx for multiple years. 
    /// It will be required to replace the grid framework with a more robust one, so all mid level -> low lever operation should be done via interface.
    /// </summary>
public partial class DataSheetComponent : ComponentBase, IFocusableObject, IRefreshableComponent
    {

        public event EventHandler Focus;
        private UniverDataSheet? _univerSheet;
        public UniverDataSheet? UniverSheet
        {
            get => _univerSheet;
            set
            {
                if (!object.Equals(_univerSheet, value))
                {
                    _univerSheet = value;
                    this.ViewModel.BlazorLifeCycle_UniverSheetReady(() => value);

                }
            }
        }

        protected override void OnInitialized()
        {
            this.SheetFocusManger.Register(this);
            this.ViewModel = new DataSheetViewModel(() => this);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        [Parameter]
        public string ID { get; set; }


        public DataSheetViewModel ViewModel { get; private set; } = null!;


        public void RemoveFocus()
        {
            // no action needed.
        }

        public void Refresh()
        {
            this.StateHasChanged();
        }
    }
}
