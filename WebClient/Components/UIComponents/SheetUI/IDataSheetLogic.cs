using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Formula.Core;
using CostAnalizerApp;
using CostAnalizerApp.Interfaces;
using System;
using System.Linq;
using SharedCode;
using BlazorDatasheet.Formula.Core.Interpreter;
using WebClient.Components.UIComponents.UniverSheet;

namespace WebClient.Components.UIComponents.SheetUI
{
    /// <summary>
    /// Current grid frame work is weak, and in version v0.1xxx for multiple years. 
    /// It will be required to replace the grid framework with a more robust one, so all mid level -> low lever operation should be done via interface.
    /// </summary>
    public interface IDataSheetLogic
    {
        public string[] Headers { get; set; }

        public void Init(DataSheetViewModel vm, UniverDataSheet sheet);


        public void OnDataAdded();
        public void OnDataRemoved();
        public void OnSheetDeleted();

    }

    class ShopProductsDataSheetLogic : IDataSheetLogic
    {
        private SpendingFileType spendingFileType;
        private readonly CostAnalizerApplication application;

        public ShopProductsDataSheetLogic(SpendingFileType spendingFileType, CostAnalizerApplication application)
        {
            this.spendingFileType = spendingFileType;
            this.application = application;
        }
        public string[] Headers { get => BillSummaryTable_Headers; set => throw new NotImplementedException(); }

        public string[] BillSummaryTable_Headers => new string[]
        {
            "Rodzaj", "Data", "Nazwa(oryginalna)", "Nazwa",
            "Kwota łącznie", "Kwota", "Zniżka", "Przed znizka", "Ilość", "Jednostka",
            "Kategoria1", "Kategoria2", "Kategoria3", "Tagi", "ID", "Metadane",
        };

        public void Init(DataSheetViewModel vm, UniverDataSheet univerDataSheet)
        {
            var updater = univerDataSheet.BatchSheetUpdater;
            //var data = this.application.Data.GetData(this.spendingFileType);

            //// Build ordered list of nodes: pre-order traversal preserving the order of roots from 'data'
            //var ordered = new List<(ISpendingCase Node, int Depth)>();

            //void Traverse(ISpendingCase node, int depth)
            //{
            //    ordered.Add((node, depth));
            //    var childs = node.Node.Childs;
            //    if (childs != null && childs.Count > 0)
            //    {
            //        foreach (var c in childs)
            //        {
            //            Traverse(c, depth + 1);
            //        }
            //    }
            //}

            //foreach (var item in data)
            //{
            //    var root = item.Value;
            //    Traverse(root, 0);
            //}

            //// Ensure sheet has enough rows to contain all nodes plus buffer
            //try
            //{
            //    var requiredRows = ordered.Count + 50;
            //    if (sheet.NumRows < requiredRows)
            //    {
            //        sheet.Rows.InsertAt(sheet.NumRows, requiredRows - sheet.NumRows);
            //    }

            //    var requiredCols = this.Headers?.Length ?? 0;
            //    if (sheet.NumCols < requiredCols)
            //    {
            //        sheet.Columns.InsertAt(sheet.NumCols, requiredCols - sheet.NumCols);
            //    }
            //}
            //catch
            //{
            //    // swallow - sheet API may throw for invalid ops
            //}

            //// Fill cells according to headers mapping
            //// Column mapping:
            //// 0: Rodzaj (depth)
            //// 1: Data
            //// 2: Nazwa(oryginalna)
            //// 3: Nazwa
            //// 4: Kwota łącznie
            //// 5: Kwota
            //// 6: Zniżka
            //// 7: Przed znizka
            //// 8: Ilość
            //// 9: Jednostka
            //// 10-12: Kategoria1..3
            //// 13: Tagi
            //// 14: ID
            //// 15: Metadane

            //for (int row = 1; row < ordered.Count+1; row++)
            //{
            //    var (node, depth) = ordered[row-1];

            //    // Rodzaj = depth
            //    sheet.Cells.SetValue(row, 0, new CellValue(depth));

            //    // Data
            //    sheet.Cells.SetValue(row, 1, new CellValue(node.Date.ToString("yyyy-MM-dd HH:mm")));

            //    // Names
            //    sheet.Cells.SetValue(row, 2, new CellValue(node.Name ?? string.Empty));
            //    sheet.Cells.SetValue(row, 3, new CellValue(node.UserFriendlyName ?? string.Empty));

            //    var summary = node.Summary;

            //    if (node.Node.Childs != null && node.Node.Childs.Count > 0)
            //    {
            //        // parent node: put aggregated total only (rounded)
            //        sheet.Cells.SetValue(row, 4, new CellValue(Math.Round((decimal)(summary?.Total ?? 0.0), 2)));
            //        sheet.Cells.SetType(row, 4, "C2");

            //        // leave individual fields empty
            //        sheet.Cells.SetValue(row, 5, new CellValue(string.Empty));
            //        sheet.Cells.SetValue(row, 6, new CellValue(string.Empty));
            //        sheet.Cells.SetValue(row, 7, new CellValue(string.Empty));

            //        // lightly shade parent row
            //        try
            //        {
            //            var cols = this.Headers?.Length ?? sheet.NumCols;
            //            for (int c = 0; c < cols; c++)
            //            {
            //                var cell = sheet.Cells[row, c];
            //                var fmt = cell.Format.Clone();
            //                fmt.BackgroundColor = "#f2f2f2"; // light gray
            //                cell.Format = fmt;
            //            }
            //        }
            //        catch
            //        {
            //            // ignore formatting errors
            //        }
            //    }
            //    else
            //    {
            //        // leaf/child: put individual amounts (raw values); formatting via cell type
            //        sheet.Cells.SetValue(row, 4, new CellValue(string.Empty));
            //        sheet.Cells.SetValue(row, 5, new CellValue(Math.Round((decimal)(summary?.Cost ?? 0.0), 2)));
            //        sheet.Cells.SetType(row, 5, "C2");
            //        sheet.Cells.SetValue(row, 6, new CellValue(Math.Round((decimal)(summary?.Discount ?? 0.0), 2)));
            //        sheet.Cells.SetType(row, 6, "C2");
            //        sheet.Cells.SetValue(row, 7, new CellValue(Math.Round((decimal)(summary?.Total ?? 0.0), 2)));
            //        sheet.Cells.SetType(row, 7, "C2");
            //    }

            //    // Quantity and unit
            //    try
            //    {
            //        var qty = summary?.Quantity;
            //            if (qty != null)
            //            {
            //                sheet.Cells.SetValue(row, 8, new CellValue(Math.Round((decimal)qty.Amount, 2)));
            //                sheet.Cells.SetType(row, 8, "C2");
            //                sheet.Cells.SetValue(row, 9, new CellValue(qty.Unit ?? string.Empty));
            //            }
            //        else
            //        {
            //            sheet.Cells.SetValue(row, 8, new CellValue(string.Empty));
            //            sheet.Cells.SetValue(row, 9, new CellValue(string.Empty));
            //        }
            //    }
            //    catch
            //    {
            //        sheet.Cells.SetValue(row, 8, new CellValue(string.Empty));
            //        sheet.Cells.SetValue(row, 9, new CellValue(string.Empty));
            //    }

            //    // Categories - unavailable, leave empty
            //    sheet.Cells.SetValue(row, 10, new CellValue(string.Empty));
            //    sheet.Cells.SetValue(row, 11, new CellValue(string.Empty));
            //    sheet.Cells.SetValue(row, 12, new CellValue(string.Empty));

            //    // Tags
            //    var tags = node.Tags?.Tags;
            //    sheet.Cells.SetValue(row, 13, new CellValue(tags != null && tags.Count > 0 ? string.Join(", ", tags) : string.Empty));

            //    // ID
            //    sheet.Cells.SetValue(row, 14, new CellValue(node.Id?.ToString() ?? string.Empty));

            //    // Metadane - try decorations ToString
            //    sheet.Cells.SetValue(row, 15, new CellValue(node.Decorations?.ToString() ?? string.Empty));
            //}

            //// Put summary formulas in the pinned first row (row 0)
            //if (ordered.Count > 0)
            //{
            //    string ColIndexToLetter(int col)
            //    {
            //        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //        var result = string.Empty;
            //        var value = col + 1; // 1-based
            //        while (value > 0)
            //        {
            //            var rem = (value - 1) % 26;
            //            result = letters[rem] + result;
            //            value = (value - 1) / 26;
            //        }
            //        return result;
            //    }

            //    // Data rows start at sheet row index 1, which corresponds to Excel row 2
            //    int firstDataExcelRow = 2;
            //    int lastDataExcelRow = ordered.Count + 1;

            //    int[] numericCols = new[] { 4, 5, 6, 7, 8 };
            //    foreach (var col in numericCols)
            //    {
            //        var colLetter = ColIndexToLetter(col);
            //        // use ROUND around SUM so the formula result is rounded to 2 decimal places
            //        var formula = $"=SUM({colLetter}{firstDataExcelRow}:{colLetter}{lastDataExcelRow})";
            //        sheet.Cells.SetType(0, col, "C2");
            //        sheet.Cells.SetFormula(0, col, formula);
            //    }

            //    // Label the first cell
            //    sheet.Cells.SetValue(0, 0, new CellValue("SUMA"));
            //}

            //sheet.FreezeTopRows(1);

            //// Adjust column widths to more realistic values for better display.

            //    void TrySetColWidth(int colIndex, int widthPx)
            //    {
            //        sheet.Columns.SetSize(colIndex, widthPx);
            //    }

            //    // sensible defaults per header index
            //    var widths = new Dictionary<int, int>
            //    {
            //        [0] = 36,   // Rodzaj (type/depth) - small
            //        [1] = 140,  // Data - include time
            //        [2] = 200,  // Nazwa(oryginalna) - long
            //        [3] = 240,  // Nazwa - long
            //        [4] = 100,  // Kwota łącznie
            //        [5] = 100,  // Kwota
            //        [6] = 80,  // Zniżka
            //        [7] = 80,  // Przed znizka
            //        [8] = 80,  // Ilość
            //        [9] = 40,   // Jednostka
            //        [10] = 140, // Kategoria1
            //        [11] = 140, // Kategoria2
            //        [12] = 140, // Kategoria3
            //        [13] = 220, // Tagi
            //        [14] = 260, // ID
            //        [15] = 380, // Metadane - can be large
            //    };

            //    foreach (var kv in widths)
            //    {
            //        TrySetColWidth(kv.Key, kv.Value);
            //    }

        }

        public void OnDataAdded()
        {

        }

        public void OnDataRemoved()
        {
 
        }

        public void OnSheetDeleted()
        {

        }
    }
}
