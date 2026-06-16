using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;

namespace WebClient.Components.UIComponents.UniverSheet
{
    /// <summary>
    /// Prosty batch updater dla sheetu. Zbiera operacje lokalnie i przy EndUpdate wysyła jedno wywołanie JS.
    /// Na razie EndUpdateAsync wywołuje tylko alert z liczbą operacji.
    /// </summary>
    public class BatchSheetUpdater
    {
        private readonly Func<IJSRuntime> _rundtimeFucntion;
       
        private readonly IJSRuntime _jsRuntime;

        public IJSRuntime JSRuntime => _jsRuntime ?? _rundtimeFucntion();
        private bool _inUpdate;
        private List<UpdateData> _buffer = new();
        public BatchSheetUpdater(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public BatchSheetUpdater(Func<IJSRuntime> rundtimeFucntion)
        {
            _rundtimeFucntion = rundtimeFucntion;
        }

        public void BeginUpdate()
        {
            _buffer.Clear();
            _inUpdate = true;
        }

        public void SetCell(int x, int y, string value)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new CellTextUpdate(x, y, value));
        }
        public void SetCellColor(int x, int y, string value)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new CellColorUpdate(x, y, value));
        }

        public void SetCellNumberFormat(int x, int y, string format)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new CellNumberFormatUpdate(x, y, format));
        }
        public void SetCellNumberFormat(int x, int y, int  decimalPlaces)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }
            string format = $"0.{new string('0', decimalPlaces)}";
            _buffer.Add(new CellNumberFormatUpdate(x, y, format));
        }
        
        // Ustawienie formuły w komórce — przyjmuje x, y i formułę jako tekst
        public void SetCellFormula(int x, int y, string formula)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new CellFormulaUpdate(x, y, formula));
        }
        
        // Zamrożenie kolumn — przyjmuje liczbę kolumn do zamrożenia
        public void univerFreezeColumns(int columns)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new FreezeColumnsUpdate(columns));
        }

        // Zamrożenie wierszy — przyjmuje liczbę wierszy do zamrożenia
        public void univerFreezeRows(int rows)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new FreezeRowsUpdate(rows));
        }

        // Ustawienie liczby kolumn zaczynając od pozycji x,y — przyjmuje x, y i count
        public void univerSetColumnCount(int count)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new SetColumnCountUpdate( count));
        }
        
        // Ustawienie liczby wierszy — przyjmuje count
        public void univerSetRowCount(int count)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new SetRowCountUpdate(count));
        }
        //univerSetNumberFormat
        // Ustawienie szerokości kolumny — przyjmuje index kolumny i szerokość
        public void univerSetColumnWidth(int columnIndex, double width)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new SetColumnWidthUpdate(columnIndex, width));
        }

        // Ustawienie wysokości wiersza — przyjmuje index wiersza i wysokość
        public void univerSetRowHeight(int rowIndex, double height)
        {
            if (!_inUpdate)
            {
                throw new InvalidOperationException("BeginUpdate must be called before setting cells.");
            }

            _buffer.Add(new SetRowHeightUpdate(rowIndex, height));
        }
        public async Task EndUpdateAsync()
        {
            if (!_inUpdate)
            {
                return;
            }
            var x = _buffer.Select(x => x.NameAndArgs().ToArray());
            await JSRuntime.InvokeVoidAsync("batchInvoker_generic", x);
            _inUpdate = false;

            _buffer.Clear();
        }

        private record CellTextUpdate(int X, int Y, string Value) : UpdateData
        {
            public string Name { get; set; } = "univerSetCell";
            public object[] ToArray()
            {
                return new object[] { X, Y, Value };
            }
        }

        private record CellColorUpdate(int X, int Y, string color) : UpdateData
        {
            public string Name { get; set; } = "univerSetCellColor";
            public object[] ToArray()
            {
                return new object[] { X, Y, color };
            }
        }

        private record CellNumberFormatUpdate(int X, int Y, string Format) : UpdateData
        {
            public string Name { get; set; } = "univerSetNumberFormat";
            public object[] ToArray()
            {
                return new object[] { X, Y, Format };
            }
        }

        private record CellFormulaUpdate(int X, int Y, string Formula) : UpdateData
        {
            public string Name { get; set; } = "univerSetFormula";
            public object[] ToArray()
            {
                return new object[] { X, Y, Formula };
            }
        }

        private record FreezeColumnsUpdate(int Columns) : UpdateData
        {
            public string Name { get; set; } = "univerFreezeColumns";
            public object[] ToArray()
            {
                return new object[] { Columns };
            }
        }

        private record FreezeRowsUpdate(int Rows) : UpdateData
        {
            public string Name { get; set; } = "univerFreezeRows";
            public object[] ToArray()
            {
                return new object[] { Rows };
            }
        }

        private record SetColumnCountUpdate(int Count) : UpdateData
        {
            public string Name { get; set; } = "univerSetColumnCount";
            public object[] ToArray()
            {
                return new object[] {  Count };
            }
        }


        private record SetRowCountUpdate(int Count) : UpdateData
        {
            public string Name { get; set; } = "univerSetRowCount";
            public object[] ToArray()
            {
                return new object[] { Count };
            }
        }

        private record SetColumnWidthUpdate(int ColumnIndex, double Width) : UpdateData
        {
            public string Name { get; set; } = "univerSetColumnWidth";
            public object[] ToArray()
            {
                return new object[] { ColumnIndex, Width };
            }
        }

        private record SetRowHeightUpdate(int RowIndex, double Height) : UpdateData
        {
            public string Name { get; set; } = "univerSetRowHeight";
            public object[] ToArray()
            {
                return new object[] { RowIndex, Height };
            }
        }


        private interface UpdateData
        {
            string Name { get; }
            object[] ToArray();
            public object[] NameAndArgs()
            {
                return new object[] { Name }.Concat(ToArray()).ToArray();
            }
        }
    }
}
