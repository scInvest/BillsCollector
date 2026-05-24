using BlazorDatasheet.Core.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using WebClient.Components.UIComponents.Dialogs;

namespace WebClient.Components.UIComponents.DataPicker
{
    public partial class DataPicker : ComponentBase
    {
        private List<string> files = new List<string>();
        private string? content;

        public Dictionary<FileData, string> Files { get; set; }

        [Parameter]
        public EventCallback<Sheet> OnNewSheet { get; set; }


        private async Task HandleFileSelected_Biedronka(InputFileChangeEventArgs e)
        {
            await FileSelected(e, FileType.Biedronka);
        }

        // New handlers for each FileType so pickers are routed correctly
        private async Task HandleFileSelected_MBank(InputFileChangeEventArgs e)
        {
            await FileSelected(e, FileType.MBank);
        }

        private async Task HandleFileSelected_Allegro(InputFileChangeEventArgs e)
        {
            await FileSelected(e, FileType.Allegro);
        }

        private async Task HandleFileSelected_Kaufland(InputFileChangeEventArgs e)
        {
            await FileSelected(e, FileType.Kaufland);
        }

        private async Task HandleFileSelected_CsvCustom(InputFileChangeEventArgs e)
        {
            await FileSelected(e, FileType.CsvCustom);
        }

        private async Task HandleFileSelected_Zabka(InputFileChangeEventArgs e)
        {
            await FileSelected(e, FileType.Zabka);
        }

        private async Task FileSelected(InputFileChangeEventArgs e, FileType fileType)
        {
            var files = e.GetMultipleFiles();
            List<string> errors = files.Select(f => f.Name).ToList();
            foreach (var file in files)
            {
                try
                {
                    using var stream = file.OpenReadStream();
                    using var reader = new StreamReader(stream);
                    content = await reader.ReadToEndAsync();
                    var key = new FileData { FileName = file.Name, Type = fileType };
                    Files.Add(key, content);
                    throw new Exception($"File {file.Name} of type {fileType} loaded successfully with content length {content.Length}");
                }
                catch (Exception ex)
                {
                    errors.Add($"Error reading file {file.Name}: {ex.Message}");
                }
            }

           await DialogService.ShowAlert(string.Join("\n", errors));
        }

        public TextInputModal Dialog { get; set; }

    }
}

