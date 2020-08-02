using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.ClosingTags
{
    class Italic : ICreatorClosingTags
    {
        public string TagName { get; } = "i";

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler)
        {
            return runCode + @"FontStyle='Italic' ";
        }
    }
}
