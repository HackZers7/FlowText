using FlowText.DeafaultTags.ClosingTags;
using FlowText.DeafaultTags.OneTags;
using FlowText.TagsCreator;
using System.Collections.Generic;

namespace FlowText.DeafaultTags
{
    public static class CreateDefaultTags
    {
        public static List<ICreatorClosingTags> CDefaultTags()
        {
            return new List<ICreatorClosingTags>()
            {
                new Bold() as ICreatorClosingTags,
                new Italic() as ICreatorClosingTags,
                new BackGroundText() as ICreatorClosingTags,
                new Font() as ICreatorClosingTags,
                new HyperLink() as ICreatorClosingTags
            };
        }

        public static List<ICreatorOneTags> CDefaultOneTags()
        {
            return new List<ICreatorOneTags>()
            {
                new BreakLine() as ICreatorOneTags
            };
        }
    }
}
