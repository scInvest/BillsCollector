using CostAnalizerApp.Interfaces;

namespace Integrations.Biedronka
{
    class SpendingCaseNodeDummy : ISpendingCaseNode
    {
        public bool IsRoot => Parent == null;

        public bool IsLeaf => Childs?.Any() != true;

        public ISpendingCase? Parent { get; set; }

        public IReadOnlyList<ISpendingCase> Childs { get; set; }


    }
}
