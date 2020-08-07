using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.ClosingTags
{
    public class Bold : ICreatorClosingTags
    {
        public string TagName { get; } = "b";

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler)
        {
            return runCode + @"FontWeight='Bold' ";
        }
    }
}
