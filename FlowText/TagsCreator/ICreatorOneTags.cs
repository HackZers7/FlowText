using System.Collections.Generic;

namespace FlowText.TagsCreator
{
    public interface ICreatorOneTags
    {
        string TagName { get; }

        string ParseText(string runCode, TagHandler tag, TextHandler textHandler);
    }
}
