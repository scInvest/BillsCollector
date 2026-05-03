namespace CostAnalizerApp.Interfaces
{
    public interface ISpendingCase 
    {
        /// <summary>
        /// Data wydatku
        /// </summary>
        public DateTime Date { get; }


        /// <summary>
        /// User friendly name
        /// </summary>
        public string UserFriendlyName { get; }

        public string Name { get; }

        public bool IsRoot { get; }

        public bool IsLeaf { get; }

        public IReadOnlyList<ISpendingCase> Childs { get; }

        public ISpendingTags Tags { get; }

        public ISpendingId Id { get; }

        public ISpendingDecorations Decorations { get; set; }

        public ISpending Summary { get; set; }

    }
}
