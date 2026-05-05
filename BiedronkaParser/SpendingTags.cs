using CostAnalizerApp.Interfaces;

namespace BiedronkaParser
{
    class SpendingTags : ISpendingTags
    {
        public SpendingTags(ISpendingCase parent)
        {
            if (parent != null)
            {
                if (parent.Node.IsLeaf)
                {
                    Tags.Add(TagsConsts.Leaf);
                }
                if (parent.Node.IsRoot)
                {
                    Tags.Add(TagsConsts.TopLevel);
                }
            }
        }

        public List<string> Tags { get; } = new List<string>();
        public List<string> GroupingTags { get; } = new List<string>();

        IReadOnlyList<string> ISpendingTags.Tags => Tags;
        IReadOnlyList<string> ISpendingTags.GroupingTags => GroupingTags;
    }
}
