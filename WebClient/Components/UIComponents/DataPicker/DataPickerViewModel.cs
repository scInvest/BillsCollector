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
        public event Action<SpendingFileType>? UserInputAfterDataAdded;

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
            ConcurrentBag<bool> conter = new ConcurrentBag<bool>();

            try
            {
                UpdateOptions updateOptions = new UpdateOptions()
                {
                    UpdateType = UpdateType.Replace,
                    SpendingFileType = fileType
                };
                Result<SpendingDataBatchUpdate> batchUpdate = CostAnalizerApplication.BeginUpdate(updateOptions);
                
                if (batchUpdate.IsFailed)
                {
                    return batchUpdate.ToOtherError<string>();
                }

                await foreach ((Stream? stream, string? fileName) in streams)
                {
                    try
                    {
                        using var reader = new System.IO.StreamReader(stream);
                        var content = await reader.ReadToEndAsync();
                        conter.Add(true);

                        ISpendingCase file = Intergrations.ReadBiedronkaJson(content);
                        batchUpdate.Value.AddData(fileName, file);
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

                CostAnalizerApplication.EndUpdate(batchUpdate.Value);

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

            if (!conter.Any())
            {
                return SharedCode.Result<string>.Failure("Brak danych do dodania.");
            }

            UserInputAfterDataAdded?.Invoke(fileType);
            return SharedCode.Result<string>.Success("Pomylślnie wczytano " + conter.Count + " plików");
        }
    }
}
