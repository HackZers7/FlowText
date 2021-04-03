using FlowText.TagsCreator;
using System.Text.RegularExpressions;

namespace FlowText.DeafaultTags.ClosingTags
{
    class BackGroundText : ITagsCreator
    {
        public string TagName { get; } = "background";
        public TypesTag TypeTag { get; } = TypesTag.ClosingTag;

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler, ParseText owner)
        {
            foreach (var var in tag.VariantsTag)
                switch (var.Variant.ToLower().Trim())
                {
                    case "color":
                        Regex regex = new Regex(@"#\w{6}");
                        var m = regex.Matches(var.Value);

                        if (m.Count == 1)
                            return runCode += "Background='" + m[0].Value + "' "; // Цвет 
                        break;
                }

            return runCode;
        }
    }
}
