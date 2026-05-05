namespace CostAnalizerApp.Interfaces
{
    /// <summary>
    /// Represents the hierarchical structure of a spending case node
    /// </summary>
    public interface ISpendingCaseNode
    {
        public bool IsRoot { get; }

        public bool IsLeaf { get; }

        public ISpendingCase? Parent { get; }

        public IReadOnlyList<ISpendingCase> Childs { get; }
    }

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

        public ISpendingCaseNode Node { get; }

        public ISpendingTags Tags { get; }

        public ISpendingId Id { get; }

        public ISpendingDecorations Decorations { get; }

        public ISpending Summary { get;}

    }
}
