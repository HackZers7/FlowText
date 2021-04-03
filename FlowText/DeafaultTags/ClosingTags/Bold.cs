using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.ClosingTags
{
    public class Bold : ITagsCreator
    {
        public string TagName { get; } = "b";
        public TypesTag TypeTag { get; } = TypesTag.ClosingTag;

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler, ParseText owner)
        {
            return runCode + @"FontWeight='Bold' ";
        }
    }
}
