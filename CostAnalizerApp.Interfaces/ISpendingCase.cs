using System;
using System.Collections.Generic;

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
        public IEnumerable<ISpendingCase> AllLeafs()
        {
            // If this node itself is a leaf, return its associated spending case (if any)
            if (IsLeaf)
            {
                if (Parent is not null)
                    yield return Parent;
                yield break;
            }

            var stack = new Stack<ISpendingCase>(Childs ?? Array.Empty<ISpendingCase>());
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Node.IsLeaf)
                {
                    yield return current;
                }
                else
                {
                    var childs = current.Node.Childs;
                    if (childs != null && childs.Count > 0)
                    {
                        for (int i = childs.Count - 1; i >= 0; i--)
                        {
                            stack.Push(childs[i]);
                        }
                    }
                }
            }
        }

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
