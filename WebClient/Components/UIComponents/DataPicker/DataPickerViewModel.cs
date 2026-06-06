using CostAnalizerApp;
using CostAnalizerApp.Interfaces;
using Integrations.API;
using Microsoft.AspNetCore.Components;
using SharedCode;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.ViewModels;
using static MudBlazor.CategoryTypes;

namespace WebClient.Components.UIComponents.DataPicker
{
    public class DataPickerViewModel : ViewModelBase
    {
        private readonly Func<CostAnalizerApplication> mainApp;
        public CostAnalizerApplication CostAnalizerApplication => mainApp();
        // Business logic reference (to be injected)
        public object BusinessLogic { get; set; }

        public event Action<SpendingFileType>? UserInputBeforeDataAdded;
        public event Action<IEnumerable<KeyValuePair<string, string>>, SpendingFileType>? UserInputAfterDataAdded;

        public DataPickerViewModel(
            Func<Microsoft.AspNetCore.Components.ComponentBase> getComponent,
            System.Func<CostAnalizerApplication> mainApp)
            : base(getComponent)
        {
            this.mainApp = mainApp;
        }

        public async Task<SharedCode.Result<string>> Handle_UserInput_DataAdded(
            IAsyncEnumerable<(Stream stream, string fileName)> streams,
            SpendingFileType fileType)
        {
            UserInputBeforeDataAdded?.Invoke(fileType);
            ConcurrentBag<string> errors = new ConcurrentBag<string>();
            ConcurrentDictionary<string, string> contents = new ConcurrentDictionary<string, string>();

            try
            {
                UpdateOptions updateOptions = new UpdateOptions()
                {
                    UpdateType = UpdateType.Replace,
                    SpendingFileType = fileType
                };
                CostAnalizerApplication.BeginUpdate(updateOptions);

                await foreach ((Stream? stream, string? fileName) in streams)
                {
                    try
                    {
                        using var reader = new System.IO.StreamReader(stream);
                        var content = await reader.ReadToEndAsync();
                        contents.TryAdd(fileName, content);

                        ISpendingCase file = Intergrations.ReadBiedronkaJson(content);
                        CostAnalizerApplication.AddData(fileName, file);
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

                CostAnalizerApplication.EndUpdate();

            }
            catch (Exception ex)
            {
                errors.Add("Blad " + ex.Message);
            }

            if (errors.Any())
            {
                var message = string.Join(Environment.NewLine, errors);
                return SharedCode.Result<string>.Failure(message);
            }

            if (!contents.Any())
            {
                return SharedCode.Result<string>.Failure("Brak danych do dodania.");
            }

            UserInputAfterDataAdded?.Invoke(contents, fileType);
            return SharedCode.Result<string>.Success("Pomylślnie wczytano "+ contents.Count + " plików");
        }
    }
}
