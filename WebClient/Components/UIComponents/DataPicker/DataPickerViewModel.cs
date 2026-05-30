using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.DataPicker
{
    public class DataPickerViewModel : ViewModelBase
    {
        // Business logic reference (to be injected)
        public object BusinessLogic { get; set; }

        public event Action<FileType>? UserInputBeforeDataAdded;
        public event Action<IEnumerable<string>, FileType>? UserInputAfterDataAdded;

        public DataPickerViewModel(ComponentBase component) : base(component)
        {
        }

        public async Task<SharedCode.Result> Handle_UserInput_DataAdded(IAsyncEnumerable<(System.IO.Stream stream, string fileName)> streams, FileType fileType)
        {
            UserInputBeforeDataAdded?.Invoke( fileType);

            var contents = new List<string>();
            try
            {
                await foreach (var (stream,  fileName) in streams)
                {
                    using var reader = new System.IO.StreamReader(stream);
                    var content = await reader.ReadToEndAsync();
                    contents.Add(content);
                }
            }
            catch (Exception ex)
            {
                return SharedCode.Result.Failure($"Błąd podczas przetwarzania plików: {ex.Message}");
            }

            if (!contents.Any())
            {
                return SharedCode.Result.Failure("Brak danych do dodania.");
            }

            UserInputAfterDataAdded?.Invoke(contents, fileType);
            return SharedCode.Result.Success();
        }
    }
}
