using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace WebClient.Components.UIComponents.UniverSheet
{
    /// <summary>
    /// Prosty batch updater dla sheetu. Zbiera operacje lokalnie i przy EndUpdate wysyła jedno wywołanie JS.
    /// Na razie EndUpdateAsync wywołuje tylko alert z liczbą operacji.
    /// </summary>
    public class BatchSheetUpdater
    {
        private readonly IJSRuntime _jsRuntime;

        private bool _inUpdate;
        private List<UpdateData> _buffer = new();

        public BatchSheetUpdater(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
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
        //univerSetNumberFormat
        public async Task EndUpdateAsync()
        {
            if (!_inUpdate)
            {
                return;
            }

            _inUpdate = false;

            var count = _buffer.Count;
            var groups = _buffer.GroupBy(x => x.Name);
            // Na razie wywołanie JS: alert z liczbą operacji.
            foreach (var item in groups)
            {
                var args = item.Select(x => x.ToArray()).ToArray(); ;
                await _jsRuntime.InvokeVoidAsync("batchInvoker", item.Key, args);
            }

            // W przyszłości tutaj można wywołać jednorazowo funkcję JS która przyjmie _buffer
            // i wykona aktualizacje w kliencie.

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


        private interface UpdateData
        {
            string Name { get; }
            object[] ToArray();
        }
    }
}
