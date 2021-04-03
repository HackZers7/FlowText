using FlowText.DeafaultTags.ClosingTags;
using FlowText.DeafaultTags.OneTags;
using FlowText.TagsCreator;
using System.Collections.Generic;

namespace FlowText.DeafaultTags
{
    public static class CreateDefaultTags
    {
        public static List<ITagsCreator> CDefaultTags()
        {
            return new List<ITagsCreator>()
            {
                // Закрывающиеся теги
                new Bold(),
                new Italic(),
                new BackGroundText(),
                new Font(),
                new HyperLink(),
                // Одиночные теги
                new BreakLine()
            };
        }
    }
}
