
using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.ClosingTags
{
    class HyperLink : ITagsCreator
    {
        public string TagName { get; } = "hyperlink";
        public TypesTag TypeTag { get; } = TypesTag.ClosingTag;

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler, ParseText owner)
        {
            textHandler.FlagBreak = true;

            runCode = "<Hyperlink ";

            foreach (var el in tag.VariantsTag)
                switch (el.Variant.ToLower().Trim())
                {
                    case "url":
                        if (el.Value == "")
                            break;

                        runCode += "NavigateUri='" + el.Value + "' ";
                        break;
                    case "bold":
                        runCode += @"FontWeight='Bold' ";
                        break;
                }

            return runCode + ">" + textHandler.Text + " </Hyperlink>";
        }
    }
}
