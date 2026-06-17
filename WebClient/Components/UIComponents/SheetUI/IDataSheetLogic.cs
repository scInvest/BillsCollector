using BlazorDatasheet;
using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Formula.Core;
using CostAnalizerApp;
using CostAnalizerApp.Interfaces;
using System;
using System.Linq;
using System.Globalization;
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

        public Task Init(DataSheetViewModel vm, UniverDataSheet sheet);

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
            "Kwota łącznie", "Kwota", "Zniżka", "Przed znizka", "Ilość",
            "Kategoria1", "Kategoria2", "Kategoria3", "Tagi", "ID", "Metadane",
        };

        public async Task Init(DataSheetViewModel vm, UniverDataSheet univerDataSheet)
        {
            var updater = univerDataSheet.BatchSheetUpdater;
            var data = this.application.Data.GetData(this.spendingFileType);

            // Build ordered list of nodes: pre-order traversal preserving the order of roots from 'data'
            var ordered = new List<(ISpendingCase Node, int Depth)>();

            void Traverse(ISpendingCase node, int depth)
            {
                ordered.Add((node, depth));
                var childs = node.Node.Childs;
                if (childs != null && childs.Count > 0)
                {
                    foreach (var c in childs)
                    {
                        Traverse(c, depth + 1);
                    }
                }
            }

            foreach (var item in data)
            {
                var root = item.Value;
                Traverse(root, 0);
            }
            updater.BeginUpdate();

            // write headers in the first row (row 0)
            if (this.Headers != null)
            {
                for (int c = 0; c < this.Headers.Length; c++)
                {
                    updater.SetCell(c, 0, this.Headers[c] ?? string.Empty);
                }
            }

            // Ensure sheet has enough rows to contain all nodes plus buffer
            var requiredRows = ordered.Count + 50;
            if (univerDataSheet.SheetWidth < requiredRows)
            {
                // use batch updater to set total row count instead of inserting
                updater.univerSetRowCount(requiredRows);
            }

            var requiredCols = this.Headers?.Length ?? 0;
            if (univerDataSheet.SheetHeight < requiredCols)
            {
                // use batch updater to set total column count instead of inserting
                updater.univerSetColumnCount(requiredCols);
            }

            // Start batch update


            // Fill cells according to headers mapping
            // Column mapping:
            // 0: Rodzaj (depth)
            // 1: Data
            // 2: Nazwa(oryginalna)
            // 3: Nazwa
            // 4: Kwota łącznie
            // 5: Kwota
            // 6: Zniżka
            // 7: Przed znizka
            // 8: Ilość
            // 9-11: Kategoria1..3
            // 12: Tagi
            // 13: ID
            // 14: Metadane

            for (int row = 2; row < ordered.Count + 2; row++)
            {
                var (node, depth) = ordered[row - 2];

                // Rodzaj = depth
                updater.SetCell(0, row, depth.ToString(CultureInfo.InvariantCulture));

                // Data
                updater.SetCell(1, row, node.Date.ToString("yyyy-MM-dd HH:mm"));

                // Names
                updater.SetCell(2, row, node.Name ?? string.Empty);
                updater.SetCell(3, row, node.UserFriendlyName ?? string.Empty);

                var summary = node.Summary;

                if (node.Node.Childs != null && node.Node.Childs.Count > 0)
                {
                    // parent node: put aggregated total only (rounded)
                    var total = Math.Round((decimal)(summary?.Total ?? 0.0), 2);
                    updater.SetCell(4, row, total.ToString(CultureInfo.InvariantCulture));
                    updater.SetCellNumberFormat(4, row, "0.00");

                    // leave individual fields empty
                    updater.SetCell(5, row, string.Empty);
                    updater.SetCell(6, row, string.Empty);
                    updater.SetCell(7, row, string.Empty);

                    // lightly shade parent row
                    try
                    {
                        var cols = this.Headers?.Length ?? univerDataSheet.SheetWidth;
                        for (int c = 0; c < cols; c++)
                        {
                            updater.SetCellColor(c, row, "#f2f2f2"); // light gray
                        }
                    }
                    catch
                    {
                        // ignore formatting errors
                    }
                }
                else
                {
                    // leaf/child: put individual amounts (raw values); formatting via cell type
                    updater.SetCell(4, row, string.Empty);

                    var cost = Math.Round((decimal)(summary?.Cost ?? 0.0), 2);
                    updater.SetCell(5, row, cost.ToString(CultureInfo.InvariantCulture));
                    updater.SetCellNumberFormat(5, row, "0.00");

                    var discount = Math.Round((decimal)(summary?.Discount ?? 0.0), 2);
                    updater.SetCell(6, row, discount.ToString(CultureInfo.InvariantCulture));
                    updater.SetCellNumberFormat(6, row, "0.00");

                    var totalLeaf = Math.Round((decimal)(summary?.Total ?? 0.0), 2);
                    updater.SetCell(7, row, totalLeaf.ToString(CultureInfo.InvariantCulture));
                    updater.SetCellNumberFormat(7, row, "0.00");
                }

                // Quantity and unit (use batch updater)
                try
                {
                    var qty = summary?.Quantity;
                    if (qty != null)
                    {
                        var amt = Math.Round((decimal)qty.Amount, 2).ToString(CultureInfo.InvariantCulture);
                        updater.SetCell(8, row, amt);
                        updater.SetCellNumberFormat(8, row, "0.00");
                    }
                    else
                    {
                        updater.SetCell(8, row, string.Empty);
                    }
                }
                catch
                {
                    updater.SetCell(8, row, string.Empty);
                    updater.SetCell(9, row, string.Empty);
                }

                // Categories - unavailable, leave empty (use batch updater)
                updater.SetCell(9, row, string.Empty);
                updater.SetCell(10, row, string.Empty);
                updater.SetCell(11, row, string.Empty);

                // Tags
                var tags = node.Tags?.Tags;
                var tagsText = tags != null && tags.Count > 0 ? string.Join(", ", tags) : string.Empty;
                updater.SetCell(12, row, tagsText);

                // ID
                updater.SetCell(13, row, node.Id?.ToString() ?? string.Empty);

                // Metadane - try decorations ToString
                updater.SetCell(14, row, node.Decorations?.ToString() ?? string.Empty);
            }

                // Put summary formulas in the pinned second row (row 1)
            if (ordered.Count > 0)
            {
                string ColIndexToLetter(int col)
                {
                    const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    var result = string.Empty;
                    var value = col + 1; // 1-based
                    while (value > 0)
                    {
                        var rem = (value - 1) % 26;
                        result = letters[rem] + result;
                        value = (value - 1) / 26;
                    }
                    return result;
                }

                // Data rows start at sheet row index 2, which corresponds to Excel row 3
                int firstDataExcelRow = 3;
                int lastDataExcelRow = ordered.Count + 2;

                int[] numericCols = new[] { 4, 5, 6, 7, 8 };
                foreach (var col in numericCols)
                {
                    var colLetter = ColIndexToLetter(col);
                    // use ROUND around SUM so the formula result is rounded to 2 decimal places
                    var formula = $"=SUM({colLetter}{firstDataExcelRow}:{colLetter}{lastDataExcelRow})";
                    updater.SetCellNumberFormat(col, 1, "0.00");
                    updater.SetCellFormula(col, 1, formula);
                }

                // Label the summary cell on row 1
                updater.SetCell(0, 1, "SUMA");
            }

            // Freeze header + summary rows via batch updater
            updater.univerFreezeRows(2);

            // Adjust column widths to more realistic values for better display.

            // sensible defaults per header index
            var widths = new Dictionary<int, int>
            {
                [0] = 36,   // Rodzaj (type/depth) - small
                [1] = 140,  // Data - include time
                [2] = 200,  // Nazwa(oryginalna) - long
                [3] = 240,  // Nazwa - long
                [4] = 100,  // Kwota łącznie
                [5] = 100,  // Kwota
                [6] = 80,  // Zniżka
                [7] = 80,  // Przed znizka
                [8] = 80,  // Ilość
                [9] = 140, // Kategoria1
                [10] = 140, // Kategoria2
                [11] = 140, // Kategoria3
                [12] = 220, // Tagi
                [13] = 260, // ID
                [14] = 380, // Metadane - can be large
            };

            // apply column widths via batch updater
            foreach (var kv in widths)
            {
                updater.univerSetColumnWidth(kv.Key, kv.Value);
            }

            // Finish batch update and push to JS
            await updater.EndUpdateAsync();
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
