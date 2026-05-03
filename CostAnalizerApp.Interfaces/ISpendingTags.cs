namespace CostAnalizerApp.Interfaces
{
    public interface ISpendingTags
    {
        /// <summary>
        /// Stadardowe tagi, alvkoho slodycze zywnosc
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Dodatkowe teagi ulatwiajece gruowanie
        /// </summary>
        public string[] GroupingTags { get; set; }
    }
}
