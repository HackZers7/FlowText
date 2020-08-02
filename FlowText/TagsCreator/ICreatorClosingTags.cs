namespace FlowText.TagsCreator
{
    public interface ICreatorClosingTags
    {
        string TagName { get; }

        string ParseText(string runCode, TagHandler tag, TextHandler textHandler);
    }
}
