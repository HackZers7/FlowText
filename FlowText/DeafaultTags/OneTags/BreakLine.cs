using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FlowText.TagsCreator;

namespace FlowText.DeafaultTags.OneTags
{
    class BreakLine : ICreatorOneTags
    {
        public string TagName { get; } = "brline";

        public string ParseText(string runCode, TagHandler tag, TextHandler textHandler)
        {
            return "<LineBreak/> ";
        }
    }
}
