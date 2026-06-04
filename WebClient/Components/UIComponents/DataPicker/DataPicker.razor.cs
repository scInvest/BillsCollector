using BlazorDatasheet.Core.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedCode;
using System.Runtime.CompilerServices;
using System.Threading;
using WebClient.Components.UIComponents.Dialogs;
using WebClient.Components.UIServices;

namespace WebClient.Components.UIComponents.DataPicker
{
    public partial class DataPicker : ComponentBase
    {

        [Parameter]
        public Func<DataPickerViewModel> ViewModelCallBack { get; set; }

        public DataPickerViewModel ViewModel => ViewModelCallBack();

        private async Task HandleFileSelected_Biedronka(InputFileChangeEventArgs e)
        {
            await FileSelected(e, SpendingFileType.BiedronkaJson);
        }

        // New handlers for each FileType so pickers are routed correctly
        private async Task HandleFileSelected_MBank(InputFileChangeEventArgs e)
        {
            await FileSelected(e, SpendingFileType.MBankCsv);
        }

        private async Task HandleFileSelected_Allegro(InputFileChangeEventArgs e)
        {
            await FileSelected(e, SpendingFileType.Allegro);
        }

        private async Task HandleFileSelected_Kaufland(InputFileChangeEventArgs e)
        {
            await FileSelected(e, SpendingFileType.Kaufland);
        }

        private async Task HandleFileSelected_CsvCustom(InputFileChangeEventArgs e)
        {
            await FileSelected(e, SpendingFileType.CsvCustom);
        }

        private async Task HandleFileSelected_Zabka(InputFileChangeEventArgs e)
        {
            await FileSelected(e, SpendingFileType.Zabka);
        }

        private async Task FileSelected(InputFileChangeEventArgs e, SpendingFileType fileType)
        {
            try
            {
                var files = e.GetMultipleFiles();
                var filesToProcess = ReadFilesLazy(files);
                var result = await ViewModel.Handle_UserInput_DataAdded(filesToProcess, fileType);
                if (result != null && !result.IsSuccess)
                {
                    await DialogService.ShowAlert(result.Error ?? "Błąd nieznany", "Błąd importu");
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlert(ex.Message ?? "Błąd nieznany", "Błąd importu");
            }
        }

        private async IAsyncEnumerable<(Stream stream, string fileName)> ReadFilesLazy(
            IReadOnlyList<IBrowserFile> files)
        {
            foreach (var file in files)
            {
                Stream? stream = null;
                stream = file.OpenReadStream();

                yield return (stream, file.Name);
            }
        }

        public TextInputModal Dialog { get; set; }

    }
}

