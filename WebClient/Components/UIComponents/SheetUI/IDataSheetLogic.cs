namespace WebClient.Components.UIComponents.SheetUI
{
    /// <summary>
    /// Current grid frame work is weak, and in version v0.1xxx for multiple years. 
    /// It will be required to replace the grid framework with a more robust one, so all mid level -> low lever operation should be done via interface.
    /// </summary>
    public interface IDataSheetLogic
    {
        public void BillSummaryTable_CreateEmpty();
        public string[] Headers { get; set; }
        public string[] BillSummaryTable_Headers { get; }
    }
}
