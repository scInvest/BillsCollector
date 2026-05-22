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
    public partial class DataSheetComponent : ComponentBase, IFocusableObject
    {
        private const string POCHighlightColor = "#FFF2CC";

        class DataSheetLogic : IDataSheetLogic
        {
            private readonly Datasheet datasheet;
            private readonly Sheet _sheet;
            private readonly DataSheetComponent dataSheetComponent;
            private IRegion? _headerRegion;

            public DataSheetLogic(Datasheet datasheet, Sheet sheet, DataSheetComponent dataSheetComponent)
            {
                this.datasheet = datasheet;
                _sheet = sheet;
                this.dataSheetComponent = dataSheetComponent;
            }

            public void BillSummaryTable_CreateEmpty()
            {
                for (int col = 0; col < BillSummaryTable_Headers.Length; col++)
                {
                    _sheet.Cells.SetValue(0, col, BillSummaryTable_Headers[col]);
                }
                _sheet.Cells.SetValue(1, 0, "Suma");

                _headerRegion = new Region(0, 1, 0, BillSummaryTable_Headers.Length);
                var headerRegionRow1 = new Region(0, 0, 0, BillSummaryTable_Headers.Length);
                dataSheetComponent.SetBorderChanged(ExcelBorderPicker.BorderOption.AllBorders, _headerRegion);
                dataSheetComponent.SetBorderChanged(ExcelBorderPicker.BorderOption.ThickOutsideBorders, _headerRegion);
                dataSheetComponent.SetColorChanged("#DDDDDD", headerRegionRow1);
                var bodyRegion = new Region(2, 110, 0, BillSummaryTable_Headers.Length);
                dataSheetComponent.SetBorderChanged(ExcelBorderPicker.BorderOption.AllBorders, bodyRegion);
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
            var headers = this.Logic.BillSummaryTable_Headers;
            RenderFragment<HeadingContext> @default = this.datasheet.ColumnHeaderTemplate;
            var x = @default(new HeadingContext(1, "sdf"));
            this.datasheet.ColumnHeaderTemplate = (original) =>
            {
                var index = Math.Max(original.Index, 0);
                index = Math.Min(index, headers.Length);
                return @default(new HeadingContext(original.Index, headers[index]));
            };
            base.OnAfterRender(firstRender);
        }

        [Parameter]
        public string ID { get; set; }
        internal IDataSheetLogic Logic { get; private set; }

        private RenderFragment<Sheet> BuildDefaultMenuItems => currentSheet => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "datasheet-menu-poc");

            builder.OpenElement(2, "button");
            builder.AddAttribute(3, "type", "button");
            builder.AddAttribute(4, "class", "datasheet-menu-poc-item");
            builder.AddAttribute(5, "onclick", EventCallback.Factory.Create(this, () => HandleMenuItemInvoked(currentSheet, "highlight-selection")));
            builder.AddContent(6, "POC: Highlight selection");
            builder.CloseElement();

            builder.OpenElement(7, "button");
            builder.AddAttribute(8, "type", "button");
            builder.AddAttribute(9, "class", "datasheet-menu-poc-item");
            builder.AddAttribute(10, "onclick", EventCallback.Factory.Create(this, () => HandleMenuItemInvoked(currentSheet, "notify-host")));
            builder.AddContent(11, "POC: Notify host");
            builder.CloseElement();

            builder.CloseElement();
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
            sheet.BatchUpdateRegion(region, cell =>
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
            sheet.BatchUpdateRegion(region, cell =>
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
            sheet.BatchUpdateRegion(region, cell =>
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
                        // approximate outside borders by setting all sides for each cell
                        formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        formatCopy.BorderRight = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = selectedBorderThickness };
                        break;
                    case ExcelBorderPicker.BorderOption.ThickOutsideBorders:
                        formatCopy.BorderTop = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
                        formatCopy.BorderBottom = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
                        formatCopy.BorderLeft = new BlazorDatasheet.Core.Formats.Border() { Color = "black", Width = Math.Max(1, selectedBorderThickness + 1) };
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
