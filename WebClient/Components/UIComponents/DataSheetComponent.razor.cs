using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Core.Events.Selection;
using BlazorDatasheet.Core.Formats;
using BlazorDatasheet.DataStructures.Geometry;
using BlazorDatasheet.Render.Headings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using WebClient.Components.UIComponents.Extenstions;
using WebClient.Components.UIServices;

namespace WebClient.Components.UIComponents
{

    /// <summary>
    /// Current grid frame work is weak, and in version v0.1xxx for multiple years. 
    /// It will be required to replace the grid framework with a more robust one, so all mid level -> low lever operation should be done via interface.
    /// </summary>
    public interface IDataSheetComponent : IFocusableObject
    {
        public string ID { get; set; }

        public string[] Headers { get; set; }

    }
    /// <summary>
    /// Current grid frame work is weak, and in version v0.1xxx for multiple years. 
    /// It will be required to replace the grid framework with a more robust one, so all mid level -> low lever operation should be done via interface.
    /// </summary>
    public interface IDataSheetLogic
    {
        public void BillSummaryTable_CreateEmpty();
        public string[] BillSummaryTable_Headers { get; }



    }
    public partial class DataSheetComponent : ComponentBase, IFocusableObject, IDataSheetComponent
    {
        private const string POCHighlightColor = "#FFF2CC";

        string[] headers;
        public string[] Headers { get => headers; set { headers = value; StateHasChanged(); } }

        class DataSheetLogic : IDataSheetLogic
        {
            private readonly Datasheet datasheet;
            private readonly Sheet _sheet;
            private readonly DataSheetComponent parent;
            private IRegion? _headerRegion;

            public DataSheetLogic(Datasheet datasheet, Sheet sheet, DataSheetComponent dataSheetComponent)
            {
                this.datasheet = datasheet;
                _sheet = sheet;
                this.parent = dataSheetComponent;
            }

            public void BillSummaryTable_CreateEmpty()
            {
                this.parent.Headers = BillSummaryTable_Headers;
            }

            public string[] BillSummaryTable_Headers => new string[]
            {
                "",  "Rodzaj", "Źródło", "Data", "Nazwa(oryginalna)", "Nazwa",
                "Kwota łącznie", "Kwota", "Zniżka", "Przed znizka", "Ilość", "Jednostka",
                "Kategoria", "Kategoria", "Kategoria", "Tagi", "ID", "Metadane",
            };

        }
    }
    public partial class DataSheetComponent : ComponentBase, IFocusableObject
    {
        private ExcelBorderPicker? borderPicker;
        private ExcelFontColorPicker? colorPickerFont;
        private ExcelFontColorPicker? colorPickerFill;
        private Datasheet datasheet;
        private Sheet sheet;
        private int selectedBorderThickness = 2;
        private string selectedBorderPosition = "all";

        public event EventHandler Focus;


        [Parameter]
        public RenderFragment<Sheet>? ContextMenuItems { get; set; }

        [Parameter]
        public EventCallback<string> MenuItemInvoked { get; set; }

        protected override void OnInitialized()
        {
            //  this.datasheet.StickyHeaders = true;

            sheet = new Sheet(120, 18);
            sheet.Selection.CellsSelected += Selection_CellsSelected;
            this.SheetFocusManger.Register(this);
            this.sheet.SheetDirty += Sheet_SheetDirty;
            this.Logic = new DataSheetLogic(datasheet, sheet, this);
            this.Logic.BillSummaryTable_CreateEmpty();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            var headers = this.Headers;
            RenderFragment<HeadingContext> @default = this.datasheet.ColumnHeaderTemplate;

            this.datasheet.ColumnHeaderTemplate = (original) =>
            {
                if (original.Index < 0 || original.Index >= headers.Length)
                {
                    return @default(original);
                }
                else
                {
                    return @default(new HeadingContext(original.Index, headers[original.Index]));

                }
            };
            base.OnAfterRender(firstRender);
        }

        [Parameter]
        public string ID { get; set; }
        internal IDataSheetLogic Logic { get; private set; }

        private RenderFragment<Sheet> BuildDefaultMenuItems => currentSheet => builder =>
        {
            builder.OpenComponent<DataSheetContextMenu>(0);
            builder.AddAttribute(1, nameof(DataSheetContextMenu.CurrentSheet), currentSheet);
            builder.CloseComponent();
        };

        private async Task HandleMenuItemInvoked(Sheet currentSheet, string action)
        {
            if (action == "highlight-selection")
            {
                SetColorChanged(POCHighlightColor, currentSheet.Selection.ActiveRegion);
            }

            await MenuItemInvoked.InvokeAsync(action);
        }

        private void Sheet_SheetDirty(object? sender, BlazorDatasheet.Core.Events.Visual.DirtySheetEventArgs e)
        {
        }

        public void RemoveFocus()
        {
            // no action needed.
        }
        private void Selection_CellsSelected(object? sender, CellsSelectedEventArgs e)
        {
            this.Focus?.Invoke(this, EventArgs.Empty);
        }

        private void HandleColorChanged(string color)
        {
            SetColorChanged(color, sheet.Selection.ActiveRegion);
        }

        public void SetColorChanged(string color, IRegion? region)
        {
            sheet.BatchUpdateRegion(region, (cell, x, y) =>
            {
                var formatCopy = cell.Format.Clone();
                formatCopy.BackgroundColor = color;
                cell.Format = formatCopy;
            });
        }

        private void HandleForeGroundColorChange(string color)
        {
            SetForeGroundColorChange(color, sheet.Selection.ActiveRegion);
        }

        public void SetForeGroundColorChange(string color, IRegion? region)
        {
            sheet.BatchUpdateRegion(region, (cell, x, y) =>
            {
                var formatCopy = cell.Format.Clone();
                formatCopy.ForegroundColor = color;
                cell.Format = formatCopy;
            });
        }
        private void HandleBorderChanged(ExcelBorderPicker.BorderOption borderType)
        {
            SetBorderChanged(borderType, sheet.Selection.ActiveRegion);
        }
        public void SetBorderChanged(ExcelBorderPicker.BorderOption args, IRegion? region)
        {
            if (region == null)
                return;

            int left = region.Left;
            int top = region.Top;
            int right = region.Left + region.Width - 1;
            int bottom = region.Top + region.Height - 1;

            sheet.BatchUpdateRegion(region, (cell, x, y) =>
            {
                var formatCopy = cell.Format.Clone();

                switch (args)
                {
                    case ExcelBorderPicker.BorderOption.None:
                        // reset all borders to the default format
                        formatCopy.BorderTop = null;
                        formatCopy.BorderBottom = null;
                        formatCopy.BorderLeft = null;
                        formatCopy.BorderRight = null;
                        break;
                    case ExcelBorderPicker.BorderOption.AllBorders:
                        formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        formatCopy.BorderRight = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.OutsideBorders:
                        // set borders only on the outer edges of the region
                        if (y == top)
                            formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        if (y == bottom)
                            formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        if (x == left)
                            formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        if (x == right)
                            formatCopy.BorderRight = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.ThickOutsideBorders:
                        if (y == top)
                            formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
                        if (y == bottom)
                            formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
                        if (x == left)
                            formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
                        if (x == right)
                            formatCopy.BorderRight = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
                        break;
                    case ExcelBorderPicker.BorderOption.BottomBorder:
                        formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.TopBorder:
                        formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.LeftBorder:
                        formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.RightBorder:
                        formatCopy.BorderRight = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.NoBorder:
                        formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "transparent", Width = 0 };
                        formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "transparent", Width = 0 };
                        formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "transparent", Width = 0 };
                        formatCopy.BorderRight = new BlazorDatasheet.Core.Formats.Border() { Color = "transparent", Width = 0 };
                        break;
                    default:
                        break;
                }
                cell.Format = formatCopy;
            });
        }


    }
}
