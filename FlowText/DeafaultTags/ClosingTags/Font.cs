using FlowText.TagsCreator;
using System.Text.RegularExpressions;

namespace FlowText.DeafaultTags.ClosingTags
{
    class Font : ICreatorClosingTags
    {
        public string TagName { get; } = "font";

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler)
        {
            foreach (var var in tag.VariantsTag)
                switch (var.Variant.ToLower().Trim())
                {
                    case "textdecor":
                        string val = "none";
                        switch (var.Value.ToLower().Trim())
                        {
                            case "strike":
                                val = "Strikethrough";
                                break;
                            case "oline":
                                val = "Overline";
                                break;
                            case "uline":
                                val = "Underline";
                                break;
                        }
                        if (val == "none")
                            break;
                        runCode += "TextDecorations ='" + val + "' ";
                        break;

                    case "newsize":
                    case "size":
                        bool flag = double.TryParse(var.Value, out double value);

                        if (!flag) break;

                        switch (var.Variant.ToLower().Trim())
                        {
                            case "newsize":
                                value += textHandler.BaseFontSize;
                                value = value <= 0 ? 1 : value;
                                runCode += "FontSize='" + value + "' "; // Размер
                                break;

                            case "size":
                                value = value <= 0 ? 1 : value;
                                runCode += "FontSize='" + value + "' "; // Размер
                                break;
                        }
                        break;

                    case "family":

                        if (var.Value == null)
                            break;
                        if (var.Value.Trim() == "")
                            break;

                        runCode += "FontFamily='" + var.Value + "' "; // Шрифт
                        break;

                    case "color":
                        Regex regex = new Regex(@"#\w{6}");
                        var m = regex.Matches(var.Value);

                        if (m.Count == 1)
                            runCode += "Foreground='" + m[0].Value + "' "; // Цвет 
                        break;
                }

            return runCode;
        }
    }
}
