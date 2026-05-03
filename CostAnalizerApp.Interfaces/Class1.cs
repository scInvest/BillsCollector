namespace CostAnalizerApp.Interfaces
{

    public interface SpendingTags
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

    public interface ISpendingId
    {

        /// <summary>
        /// Any other object that i find sue ful makebe for UI, mabe for debugging. 
        /// </summary>
        public string AdditionalTextInfo { get; }

        /// <summary>
        /// Unikalne ID, wramach systemu.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Dodatkowe ID, takie jak numer tranzakcji, numer paragonu, numer konta itp. 
        /// </summary>
        public IReadOnlyDictionary<string, string> AllIds { get; }
    }

    public interface ISpendingCase : ISpending
    {
        /// <summary>
        /// Data wydatku
        /// </summary>
        public DateTime Date { get; }


        /// <summary>
        /// User friendly name
        /// </summary>
        public string UserFriendlyName { get; }
        public bool IsRoot { get; }

        public bool IsLeaf { get; }

        public IReadOnlyList<ISpendingCase> Childs { get; }

        public SpendingTags Tags { get; }

        public ISpendingId Id { get; }

    }

    /// <summary>
    /// Reprezentuje pojedyczy wydatek. Uproszczona bazowka
    /// </summary>
    public interface ISpending
    {
        public string Name { get; }
        public double Cost { get; }
        public double Discount { get; }
        public double Total { get; }

    }
}
