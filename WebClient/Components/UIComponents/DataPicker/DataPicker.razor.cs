using BlazorDatasheet.Core.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.Components.UIComponents.Dialogs;

namespace WebClient.Components.UIComponents.DataPicker
{
    public partial class DataPicker : ComponentBase
    {
        List<string> files = new List<string>();
        private string? content;

        [Parameter]
        public EventCallback<Sheet> OnNewSheet { get; set; }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles();

            foreach (var file in files)
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                content = await reader.ReadToEndAsync();
            }
        }

        public TextInputModal    Dialog { get; set; }
    }
}

