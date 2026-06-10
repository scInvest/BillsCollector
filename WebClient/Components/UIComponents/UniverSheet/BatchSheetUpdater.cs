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
        private List<PerformUpdate> _buffer = new();

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

            _buffer.Add(new PerformUpdate("univerSetCell", x, y, value));
        }

        public async Task EndUpdateAsync()
        {
            if (!_inUpdate)
            {
                return;
            }

            _inUpdate = false;

            var count = _buffer.Count;
            var groups = _buffer.GroupBy(x => x.name);
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

        private record PerformUpdate(string name, int X, int Y, string Value)
        {
            public object[] ToArray()
            {
                return new object[] { X, Y, Value };
            }
        }
    }
}
