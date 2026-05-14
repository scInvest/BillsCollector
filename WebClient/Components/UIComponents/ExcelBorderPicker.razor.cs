using Microsoft.AspNetCore.Components;

namespace WebClient.Components.UIComponents
{
    public partial class ExcelBorderPicker: ComponentBase, IFocusableObject
    {
        private bool IsOpen;

        protected override void OnInitialized()
        {
            this.SheetFocusManger.Register(this);
        }

        [Parameter]
        public BorderOption SelectedBorder { get; set; }

        [Parameter]
        public EventCallback<BorderOption> SelectedBorderChanged { get; set; }

        private void ToggleDropdown()
        {
            if (!IsOpen)
            {
                Focus?.Invoke(this, EventArgs.Empty);
            }
            IsOpen = !IsOpen;
        }

        public event EventHandler Focus;
        public void RemoveFocus()
        {
            this.IsOpen = false;
            this.StateHasChanged();
        }
        private async Task SelectBorder(BorderOption option)
        {
            SelectedBorder = option;
            IsOpen = false;

            await SelectedBorderChanged.InvokeAsync(option);
        }

        private string GetLabel(BorderOption option)
        {
            return option switch
            {
                BorderOption.None => "None",
                BorderOption.AllBorders => "All Borders",
                BorderOption.OutsideBorders => "Outside Borders",
                BorderOption.ThickOutsideBorders => "Thick Outside Borders",
                BorderOption.BottomBorder => "Bottom Border",
                BorderOption.TopBorder => "Top Border",
                BorderOption.LeftBorder => "Left Border",
                BorderOption.RightBorder => "Right Border",
                BorderOption.NoBorder => "No Border",
                _ => option.ToString()
            };
        }

        private string GetIcon(BorderOption option)
        {
            return option switch
            {
                BorderOption.AllBorders => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='black' stroke-width='1.5'/>
                    <line x1='2' y1='7' x2='16' y2='7' stroke='black'/>
                    <line x1='2' y1='11' x2='16' y2='11' stroke='black'/>
                    <line x1='7' y1='2' x2='7' y2='16' stroke='black'/>
                    <line x1='11' y1='2' x2='11' y2='16' stroke='black'/>
                </svg>",

                BorderOption.OutsideBorders => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='black' stroke-width='2'/>
                </svg>",

                BorderOption.ThickOutsideBorders => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='black' stroke-width='3'/>
                </svg>",

                BorderOption.BottomBorder => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='#bbb'/>
                    <line x1='2' y1='16' x2='16' y2='16' stroke='black' stroke-width='2'/>
                </svg>",

                BorderOption.TopBorder => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='#bbb'/>
                    <line x1='2' y1='2' x2='16' y2='2' stroke='black' stroke-width='2'/>
                </svg>",

                BorderOption.LeftBorder => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='#bbb'/>
                    <line x1='2' y1='2' x2='2' y2='16' stroke='black' stroke-width='2'/>
                </svg>",

                BorderOption.RightBorder => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='#bbb'/>
                    <line x1='16' y1='2' x2='16' y2='16' stroke='black' stroke-width='2'/>
                </svg>",

                BorderOption.NoBorder => @"
                <svg width='18' height='18' viewBox='0 0 18 18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='#bbb'/>
                    <line x1='3' y1='15' x2='15' y2='3' stroke='red' stroke-width='2'/>
                </svg>",

                _ => @"
                <svg width='18' height='18'>
                    <rect x='2' y='2' width='14' height='14' fill='none' stroke='gray'/>
                </svg>"
            };
        }
    }
}
