using FlowText.TagsCreator;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FlowText
{
    public class FlowTextScrollViewer : FlowDocumentScrollViewer
    {
        private string _text;
        private TextAlignment _textAlignment = TextAlignment.Left;
        private List<ITagsCreator> _customTags = new List<ITagsCreator>();

        /// <summary>
        /// Возвращает и устанавливает текст.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = "";
                if (value != null)
                    _text = value;

                var parse = new ParseText();
                parse.CustomTags = _customTags;
                parse.FontSize = FontSize;
                parse.TextAlignment = TextAlignment;

                Document = parse.ParseTextToXaml(_text);
            }
        }
        /// <summary>
        /// Возвращает и устанавливает выравание текста.
        /// </summary>
        public TextAlignment TextAlignment { get => _textAlignment; set => _textAlignment = value; }
        /// <summary>
        /// Возвращает и устанавливает пользовательские теги.
        /// </summary>
        public List<ITagsCreator> CustomTags { get => _customTags; set => _customTags = value; }

    }
}
