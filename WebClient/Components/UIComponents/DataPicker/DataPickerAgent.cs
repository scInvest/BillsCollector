using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using WebClient.ViewModels;

namespace WebClient.Components.UIComponents.DataPicker
{
    public class DataPickerAgent : ViewModelBase
    {
        // Business logic reference (to be injected)
        public object BusinessLogic { get; set; }

        public event Action<IEnumerable<string>, FileType>? UserInputBeforeDataAdded;
        public event Action<IEnumerable<string>, FileType>? UserInputAfterDataAdded;

        public DataPickerAgent(ComponentBase component) : base(component)
        {
        }

        public void Handle_UserInput_DataAdded(IEnumerable<string> data, FileType type)
        {
            UserInputBeforeDataAdded?.Invoke(data, type);
            // TODO: Add logic for handling data addition
            UserInputAfterDataAdded?.Invoke(data, type);
        }
    }
}
