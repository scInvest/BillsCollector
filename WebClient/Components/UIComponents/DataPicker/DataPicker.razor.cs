using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using BlazorDatasheet.Core.Data;

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

        private async Task CreateNewSheet()
        {
            // create a default sheet (20 rows x 10 cols)
            var sheet = new Sheet(20, 10);
            // invoke callback so parent can attach or display the sheet
            if (OnNewSheet.HasDelegate)
            {
                await OnNewSheet.InvokeAsync(sheet);
            }
        }
    }
}

