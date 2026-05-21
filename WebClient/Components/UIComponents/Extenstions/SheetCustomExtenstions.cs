using BlazorDatasheet.Core.Data;
using BlazorDatasheet.DataStructures.Geometry;

namespace WebClient.Components.UIComponents.Extenstions
{
    public static class SheetCustomExtenstions
    {

        public static void BatchUpdateRegion(this Sheet sheet, IRegion? region, Action<SheetCell> updateAction)
        {
            if (region == null)
                return;

            sheet.BatchUpdates();
            for (int i = 0; i < region.Width; i++)
            {
                for (int j = 0; j < region.Height; j++)
                {
                    var x = region.Left + i;
                    var y = region.Top + j;
                    var cell = sheet.Cells[y, x];
                    updateAction(cell);
                    Console.WriteLine(  x +"  " + y);
                }
            }
            sheet.EndBatchUpdates();
        }
    }
}
