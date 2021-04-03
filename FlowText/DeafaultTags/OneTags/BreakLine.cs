using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.OneTags
{
    class BreakLine : ITagsCreator
    {
        public string TagName { get; } = "brline";
        public TypesTag TypeTag { get; } = TypesTag.OneTag;

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler, ParseText owner)
        {
            return "<LineBreak/> ";
        }
    }
}
