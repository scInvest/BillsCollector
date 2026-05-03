namespace CostAnalizerApp.Interfaces
{
    public interface ISpendingId
    {

        /// <summary>
        /// Numer elementu w kolejnosci. Glownie do obslugi gdy mamy kilka "identycznych", wydatków jeden po drugim.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Unikalne ID, wramach systemu.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Dodatkowe ID, takie jak numer tranzakcji, numer paragonu, numer konta itp. 
        /// </summary>
        public IReadOnlyDictionary<string, string> AllIds { get; }
    }
}
