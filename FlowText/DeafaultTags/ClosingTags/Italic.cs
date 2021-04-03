using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.ClosingTags
{
    class Italic : ITagsCreator
    {
        public string TagName { get; } = "i";
        public TypesTag TypeTag { get; } = TypesTag.ClosingTag;

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler, ParseText owner)
        {
            return runCode + @"FontStyle='Italic' ";
        }
    }
}
