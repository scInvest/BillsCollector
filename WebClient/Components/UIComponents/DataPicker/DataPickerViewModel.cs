using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SharedCode;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.DataPicker
{
    public class DataPickerViewModel : ViewModelBase
    {
        // Business logic reference (to be injected)
        public object BusinessLogic { get; set; }

        public event Action<SpendingFileType>? UserInputBeforeDataAdded;
        public event Action<IEnumerable<KeyValuePair<string, string>>, SpendingFileType>? UserInputAfterDataAdded;

        public DataPickerViewModel(Func<ComponentBase> getComponent)
            : base(getComponent)
        {
        }

        public async Task<SharedCode.Result> Handle_UserInput_DataAdded(IAsyncEnumerable<(Stream stream, string fileName)> streams, SpendingFileType fileType)
        {
            UserInputBeforeDataAdded?.Invoke(fileType);
            ConcurrentBag<string> errors = new ConcurrentBag<string>();
            ConcurrentDictionary<string,string> contents = new ConcurrentDictionary<string,string>();

            await foreach (var (stream, fileName) in streams)
            {
                try
                {
                    using var reader = new System.IO.StreamReader(stream);
                    var content = await reader.ReadToEndAsync();
                    contents.TryAdd(fileName, content);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
                finally
                {
                    stream.Dispose();
                }
            }

            if (errors.Any())
            {
                var message = string.Join(Environment.NewLine, errors);
                return SharedCode.Result.Failure(message);
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
