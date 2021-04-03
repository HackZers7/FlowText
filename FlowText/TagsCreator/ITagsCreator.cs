namespace FlowText.TagsCreator
{
    public interface ITagsCreator
    {
        string TagName { get; }
        TypesTag TypeTag { get; }

        string ParseText(string runCode, TagHandler tag, TextHandler textHandler, ParseText owner);
    }
}
