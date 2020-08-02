using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

using FlowText.TagsCreator;

namespace FlowText
{
    public class FlowTextScrollViewer : FlowDocumentScrollViewer
    {
        private string _text;
        private TextAlignment _textAlignment = TextAlignment.Left;
        private List<ICreatorClosingTags> _customClosingTags = new List<ICreatorClosingTags>();
        private List<ICreatorOneTags> _customOneTags = new List<ICreatorOneTags>();

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
                if(value != null)
                    _text = value;

                Document = ParseText.ParseTextToXaml(_text, _customOneTags, _customClosingTags, FontSize, TextAlignment);
            }
        }
        /// <summary>
        /// Возвращает и устанавливает выравание текста.
        /// </summary>
        public TextAlignment TextAlignment {get => _textAlignment; set => _textAlignment = value; }
        /// <summary>
        /// Возвращает и устанавливает кастомные закрывающиеся теги.
        /// </summary>
        public List<ICreatorClosingTags> CustomClosingTags { get => _customClosingTags; set => _customClosingTags = value; }
        /// <summary>
        /// Возвращает и устанавливает кастомные одиночные теги.
        /// </summary>
        public List<ICreatorOneTags> CustomOneTags { get => _customOneTags; set => _customOneTags = value; }

    }
}
