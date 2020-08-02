using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
