using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Core.Events.Selection;
using BlazorDatasheet.Core.Formats;
using Microsoft.AspNetCore.Components;
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

        protected override void OnInitialized()
        {
            sheet = new Sheet(20, 10);
            sheet.Selection.CellsSelected += Selection_CellsSelected;
            sheet.Cells.SetValue(0, 0, "Hello");
            this.SheetFocusManger.Register(this);
            this.sheet.SheetDirty += Sheet_SheetDirty;
        }

        [Parameter]
        public string ID { get; set; }
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
            sheet.BatchUpdateRegion(sheet.Selection.ActiveRegion, cell =>
            {
                var formatCopy = cell.Format.Clone();
                formatCopy.BackgroundColor = color;
                cell.Format = formatCopy;
            });
        }

        private void HandleForeGroundColorChange(string color)
        {
            sheet.BatchUpdateRegion(sheet.Selection.ActiveRegion, cell =>
            {
                var formatCopy = cell.Format.Clone();
                formatCopy.ForegroundColor = color;
                cell.Format = formatCopy;
            });
        }

        private void HandleBorderChanged(ExcelBorderPicker.BorderOption args)
        {
            sheet.BatchUpdateRegion(sheet.Selection.ActiveRegion, cell =>
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
