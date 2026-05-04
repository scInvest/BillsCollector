namespace CostAnalizerApp.Interfaces
{
    public interface ISpendingTags
    {
        /// <summary>
        /// Stadardowe tagi, alvkoho slodycze zywnosc
        /// </summary>
        public  IReadOnlyList<string> Tags { get;  }

        /// <summary>
        /// Dodatkowe teagi ulatwiajece gruowanie
        /// </summary>
        public IReadOnlyList<string> GroupingTags { get;  }
    }
}
