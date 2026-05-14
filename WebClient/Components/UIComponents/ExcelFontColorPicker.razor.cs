using Microsoft.AspNetCore.Components;

namespace WebClient.Components.UIComponents
{
    public partial class ExcelFontColorPicker : ComponentBase, IFocusableObject
    {
        private List<FontColorOption> Colors = new()
        {
            new() { Name = "Black", Color = "#000000" },
            new() { Name = "Red", Color = "#C00000" },
            new() { Name = "Blue", Color = "#4472C4" },
            new() { Name = "Green", Color = "#70AD47" },
            new() { Name = "Orange", Color = "#ED7D31" },
            new() { Name = "Purple", Color = "#7030A0" },
            new() { Name = "Gray", Color = "#7F7F7F" },
            new() { Name = "Yellow", Color = "#FFC000" }
        };

        protected override void OnInitialized()
        {
            this.SheetFocusManger.Register(this);
        }
        [Parameter]
        public RenderFragment<string> MainIcon { get; set; }

        private bool IsOpen;

        public event EventHandler Focus;

        [Parameter]
        public string SelectedColor { get; set; } = "#000000";

        [Parameter]
        public EventCallback<string> SelectedColorChanged { get; set; }

        private void ToggleDropdown()
        {
            if (!IsOpen)
            {
                Focus?.Invoke(this, EventArgs.Empty);
            }

            IsOpen = !IsOpen;
        }

        private async Task SelectColor(string color)
        {
            SelectedColor = color;
            IsOpen = false;

            await SelectedColorChanged.InvokeAsync(color);
        }

        public void RemoveFocus()
        {
            this.IsOpen = false;
            this.StateHasChanged();
        }

        public class FontColorOption
        {
            public string Name { get; set; } = string.Empty;
            public string Color { get; set; } = string.Empty;
        }

    }
}
