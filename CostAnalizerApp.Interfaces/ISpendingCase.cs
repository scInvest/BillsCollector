namespace CostAnalizerApp.Interfaces
{
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

        public ISpendingDecorations Decorations { get; set; }

    }
}
