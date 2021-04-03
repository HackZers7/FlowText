using FlowText.TagsCreator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using static FlowText.DeafaultTags.CreateDefaultTags;

namespace FlowText
{
    public partial class ParseText
    {
        private double _fontSize = 14;
        private TextAlignment _textAlignment = TextAlignment.Left;
        private string _lastContent = "";
        private string _startBracket = "[";
        private string _endBracket = "]";
        private List<ITagsCreator> _cusromTags = new List<ITagsCreator>();
        private FlowDocument _lastFlowDocument = new FlowDocument();
        private object _otherCustomProperties = null;

        #region Properties
        /// <summary>
        /// Возвращает и устанавливает базовы размер шрифта.
        /// По умолчанию - 14.
        /// </summary>
        public double FontSize { get => _fontSize; set => _fontSize = value > 1 ? value : 1; }

        /// <summary>
        /// Возвращает и устанавливает базовое выравнивание.
        /// По умолчанию - по левой стороне.
        /// </summary>
        public TextAlignment TextAlignment { get => _textAlignment; set => _textAlignment = value; }

        /// <summary>
        /// Возвращает последний преобразованный текст.
        /// </summary>
        public string LastContent { get => _lastContent; }

        /// <summary>
        /// Возвращает и устанавливает стартовую каретку тега.
        /// </summary>
        public string StartBracket { get => _startBracket; set => _startBracket = string.IsNullOrEmpty(value) ? "[" : value; }

        /// <summary>
        /// Возвращает и устанавливает конечную каретку тега.
        /// </summary>
        public string EndBracket { get => _endBracket; set => _endBracket = string.IsNullOrEmpty(value) ? "]" : value; }

        /// <summary>
        /// Возвращает и устанавливает пользовательсие теги.
        /// </summary>
        public List<ITagsCreator> CustomTags { get => _cusromTags; set => _cusromTags = value; }

        /// <summary>
        /// Возвращает последний преобразованныей Потоковый документ.
        /// </summary>
        public FlowDocument LastFlowDocument { get => _lastFlowDocument; }

        /// <summary>
        /// Возвращает и устанавливает объект (ссылки) на класс с доп. параметрами для паршеров пользовательских тегов.
        /// </summary>
        public object CustomProperties { get => _otherCustomProperties; set => _otherCustomProperties = value; }
        #endregion

        #region Иницилизаторы
        /// <summary>
        /// Заного преобразует последний текст из формата "При[Font size=3 family=Font_A]вет [/b]мир[/b][/Font]!"
        /// в формат пригодный для записи в потоковые документы.
        /// Для обозначения пробелов внутри значения тега использовать _.
        /// Внимание закрывайте все теги перед br - переносом на следующую строчку.
        /// </summary>
        public FlowDocument ParseTextToXaml()
        {
            return ParseTextToXaml(_lastContent);
        }

        /// <summary>
        /// Заного преобразует последний текст из формата "При[Font size=3 family=Font_A]вет [/b]мир[/b][/Font]!"
        /// в формат пригодный для записи в потоковые документы.
        /// Для обозначения пробелов внутри значения тега использовать _.
        /// Внимание закрывайте все теги перед br - переносом на следующую строчку.
        /// </summary>
        /// <param name="customTags">Пользовательские теги</param>
        public FlowDocument ParseTextToXaml(List<ITagsCreator> customTags)
        {
            _cusromTags = customTags;

            return ParseTextToXaml(_lastContent);
        }

        /// <summary>
        /// Преобразует текст из формата "При[Font size=3 family=Font_A]вет [/b]мир[/b][/Font]!"
        /// в формат пригодный для записи в потоковые документы.
        /// Для обозначения пробелов внутри значения тега использовать _.
        /// Внимание закрывайте все теги перед br - переносом на следующую строчку.
        /// </summary>
        /// <param name="s">Строка для преобразования</param>
        /// <param name="customTags">Пользовательские теги</param>
        public FlowDocument ParseTextToXaml(string s, List<ITagsCreator> customTags)
        {
            _cusromTags = customTags;

            return ParseTextToXaml(s);
        }
        #endregion

        /// <summary>
        /// Преобразует текст из формата "При[Font size=3 family=Font_A]вет [/b]мир[/b][/Font]!"
        /// в формат пригодный для записи в потоковые документы.
        /// Для обозначения пробелов внутри значения тега использовать _.
        /// Внимание закрывайте все теги перед br - переносом на следующую строчку.
        /// </summary>
        /// <param name="s">Строка для преобразования</param>
        public FlowDocument ParseTextToXaml(string s)
        {
            _lastContent = s;
            // Создание тегов по умолчанию
            var tags = CDefaultTags();
            // Добавление пользовательских тегов к остальным
            tags.AddRange(_cusromTags);

            try
            {
                // Замена экранированных кореток
                s = s.Replace(@"\" + _startBracket, "$rb");
                s = s.Replace(@"\" + _endBracket, "$lb");
                // Подготовка изначального кода Потокового документа
                string doneText = string.Format("<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
                " xmlns:x = 'http://schemas.microsoft.com/winfx/2006/xaml'" +
                " xmlns:d = 'http://schemas.microsoft.com/expression/blend/2008'" +
                " xmlns:mc = 'http://schemas.openxmlformats.org/markup-compatibility/2006'" +
                $" FontSize='{_fontSize}'" +
                " xml:space='preserve'> ");

                List<string> textParce = TagBR(SplitText(s)); // Разделение текста по тегу br

                foreach (string el in textParce)
                {
                    // Паршинг одиночных тегов
                    List<string> tempText = OneTagsParse(SplitText(el), tags);

                    // Перенос нескольких пробелов из "пустых" строчек для правильной работы табуляции
                    if (tempText.Count > 1)
                        for (int i = 0; i < tempText.Count;)
                        {
                            if (tempText[i].Trim() == "" && i + 1 != tempText.Count)
                            {
                                int pos = 0;

                                for (int j = i + 1; j < tempText.Count; j++)
                                    if (!tempText[j].Contains(_startBracket) && !tempText[j].Contains(_endBracket))
                                    {
                                        pos = j;
                                        break;
                                    }

                                string temp = tempText[i];

                                tempText.RemoveAt(i);

                                string temp2 = tempText[pos - 1];

                                tempText.RemoveAt(pos - 1);

                                tempText.Insert(pos - 1, temp + temp2);
                            }
                            else i++;
                        }

                    doneText += $"<Paragraph TextAlignment='{TextAligmentSwith()}'>" +
                        TwoTagsParse(tempText, tags) +  // Преобразование в код Xaml на основе двух тегов
                        "</Paragraph>";
                }

                doneText += "</FlowDocument>";
                // Возвращем экранированные корретки на место
                doneText = doneText.Replace(@"$rb", _startBracket);
                doneText = doneText.Replace(@"$lb", _endBracket);
                // Интерпритируеи готовый код Потокового документа
                using (var xmlReader = new XmlTextReader(new MemoryStream(new UTF8Encoding().GetBytes(doneText))) { WhitespaceHandling = WhitespaceHandling.None })
                {
                    var document = (FlowDocument)XamlReader.Load(xmlReader);

                    document.IsOptimalParagraphEnabled = true;
                    document.IsHyphenationEnabled = true;

                    Style style = new Style(typeof(Paragraph));
                    style.Setters.Add(new Setter(Block.MarginProperty, new Thickness(5)));
                    document.Resources.Add(typeof(Paragraph), style);

                    return document;
                }
            }
            catch (Exception e) // Реагируем на ошибку компиляции
            {
                string exaption = string.Format("<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
                " xmlns:x = 'http://schemas.microsoft.com/winfx/2006/xaml'" +
                " xmlns:d = 'http://schemas.microsoft.com/expression/blend/2008'" +
                " xmlns:mc = 'http://schemas.openxmlformats.org/markup-compatibility/2006' > " +
                "<Paragraph>" +
                $"<Run>Ошибка: Ошибка при компиляции, пожалуйста, проверте, чтобы значения атрибутов тегов не были пустыми. {e}</Run>" +
                "</Paragraph>" +
                "</FlowDocument>");

                var exaptionBytes = new UTF8Encoding().GetBytes(exaption);
                var flowDocument = (FlowDocument)XamlReader.Load(new MemoryStream(exaptionBytes));

                flowDocument.IsOptimalParagraphEnabled = true;
                flowDocument.IsHyphenationEnabled = true;

                return flowDocument;
            }
        }
        private List<string> TagBR(List<string> s)
        {
            string tag = "br"; // Тег, который будет распозноваться как перенос на следующую строчку
            try
            {
                List<int> tagBr = new List<int>();

                //Добавление тега br в начало и конец строкового массива
                s.Insert(0, $"{_startBracket}{tag}{_endBracket}");
                s.Add($"{_startBracket}{tag}{_endBracket}");

                for (int i = 0; i < s.Count; i++) // Поиск тегов br и разделение по ним
                    if (s[i].ToLower() == $"{_startBracket}{tag}{_endBracket}".ToLower())
                        tagBr.Add(i);

                List<string> returnString = new List<string>(); // Здесь храняться уже разделенные строки

                for (int tagNumber = 0; tagNumber < tagBr.Count - 1; tagNumber++)
                {
                    List<string> str = new List<string>();

                    for (int i = tagBr[tagNumber] + 1; i < s.Count; i++) // Разделение строк
                    {
                        if (i == tagBr[tagNumber + 1]) break;

                        str.Add(s[i]);
                    }

                    returnString.Add(string.Join("", str));
                }

                return returnString;
            }
            catch { return new List<string> { $"Ошибка: Произошла ошибка при разделении на строки, пожалуйста проверьте все теги {tag}." }; }
        }
        private List<string> OneTagsParse(List<string> s, List<ITagsCreator> oneTags)
        {
            try
            {
                for (int i = 0; i < s.Count; i++)
                {
                    if (s[i].Contains(_startBracket) && s[i].Contains(_endBracket))
                    {
                        if (s[i].StartsWith($"{_startBracket}!--") && s[i].EndsWith($"--{_endBracket}"))
                        {
                            s[i] = "";
                            continue;
                        }

                        s[i] = s[i].Trim();

                        TagSplitter(s[i], out string tagName, out string tagVarl);

                        string[] tagVar = new string[0];

                        if (tagVarl != "" && tagVarl != null) // Разделение всех вариантов тега
                            tagVar = tagVarl.Split(new char[] { ' ' });

                        foreach (var el in oneTags)
                            if (el.TypeTag == TypesTag.OneTag && el.TagName == tagName.ToLower().Trim())
                            {
                                s[i] = "[&OneTagsSplitTag&" + tagName + " " + tagVarl + "]";
                                s.Insert(i + 1, "Текст заполнения внутренностей тега, будет удален.");
                                s.Insert(i + 2, "[/&OneTagsSplitTag&" + tagName + "]");
                            }

                        // Обработка табуляции
                        switch (tagName.ToLower())
                        {
                            case "tab":
                            case "t":
                                s[i] = string.Format("    ");
                                break;
                        }
                    }
                }

                return s;
            }
            catch (Exception e) { MessageBox.Show("" + e); return new List<string> { $"Ошибка: Проверьте одиночные теги." }; }
        }
        private string TwoTagsParse(List<string> s, List<ITagsCreator> tags)
        {
            // Производиться запись тега вместе с текстом, для последующего преобразования. Текст записывается по порядку следования
            List<TextHandler> textHandler = new List<TextHandler>();

            // Записываются все найденные открытые теги по пути, если найден закрывающийся тег такого же типа, то открытый тег закрывается
            List<TagHandler> openTags = new List<TagHandler>();

            for (int i = 0; i < s.Count; i++)
            {
                try
                {
                    for (int j = 0; j < openTags.Count; j++)
                        if (openTags[j].TagEnd == i)
                            openTags.RemoveAt(j);

                    int pos1 = -1;
                    // Смотрит содержит ли предоставленный элемент текста открывающийся тег
                    if (s[i].Contains(_startBracket) && s[i].Contains(_endBracket))
                        pos1 = i;
                    else // Если не содержит, то текст записывается в держатель вместе со всеми открытыми тегами
                    {
                        TextHandler tempTextHandler = new TextHandler();
                        tempTextHandler.Text = s[i];
                        foreach (TagHandler el in openTags)
                            tempTextHandler.Tags.Add(el);

                        tempTextHandler.BaseFontSize = _fontSize;

                        textHandler.Add(tempTextHandler);

                        continue;
                    }

                    // Проверка на закрывающийся тег
                    if (s[i].Replace(" ", "").StartsWith("[/"))
                        continue;


                    // Резделение тега и его вариантов, раделителями являются пробелы
                    TagSplitter(s[pos1], out string tagName, out string tagVar);

                    int pos2 = -1;

                    // Счетчик, указывающий, сколько закрывающихся тегов необзодимо пропустить
                    int countSkip = 0;

                    // Поиск закрывающегося тега
                    for (int j = pos1 + 1; j < s.Count; j++)
                    {
                        TagSplitter(s[j], out string tempName, out string none);

                        // Если найден открытый тег такого же типа, пропустить следующий закрытый
                        if ($"{_startBracket}{tagName}{_endBracket}".ToLower() == $"{_startBracket}{tempName}{_endBracket}".ToLower())
                            countSkip++;

                        if ($"{_startBracket}/{tagName}{_endBracket}".ToLower() == s[j].ToLower().Replace(" ", ""))
                        {
                            if (countSkip == 0)
                            {
                                pos2 = j;
                                break;
                            }
                            countSkip--;
                        }
                    }

                    // Реагирование на не нахождение закрывающегося тега
                    //if (pos2 == -1)
                    //  return @"<Run>Ошибка: Не найден закрывающийся тег для: " + startBracket + tagName + " " + tagVar + endBracket + " </Run>";

                    TagHandler tempHandler = new TagHandler();
                    tempHandler.TagName = tagName;
                    tempHandler.TagEnd = pos2;
                    tempHandler.AddVariantsTag(tagVar);

                    // Добавление тега в хранилище для последующей передачи
                    openTags.Add(tempHandler);
                }
                catch (Exception e) { return @"<Run>Ошибка: " + e + " </Run>"; }
            }

            string doneText = "";
            // Сборка текста
            foreach (TextHandler el in textHandler)
                doneText += el.Parse(tags, this);

            return doneText;
        }
        private void TagSplitter(string s, out string tagName, out string tagVar)
        {
            tagName = s;
            tagVar = "";

            if (!tagName.Contains("[") && !tagName.Contains("]"))
                return;

            tagName = tagName.Remove(0, _startBracket.Length);
            tagName = tagName.Remove(tagName.Length - _endBracket.Length, _endBracket.Length);

            tagName = tagName.Trim();

            // Проверка на нахождение вариантов
            if (tagName.Contains(" "))
            {
                int posp = tagName.IndexOf(" ");
                tagVar = tagName.Substring(posp + 1, tagName.Length - posp - 1);
                tagName = tagName.Substring(0, posp);
            }
        }
        private List<string> SplitText(string s)
        {
            string rb = "&pR+++&";
            string lb = "&pL+++&";
            string[] stringSeparators = { _startBracket, _endBracket };

            s = s.Replace(_startBracket, _startBracket + rb);
            s = s.Replace(_endBracket, lb + _endBracket);

            List<string> textParse = s.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < textParse.Count; i++)
            {
                textParse[i] = textParse[i].Replace(rb, _startBracket);
                textParse[i] = textParse[i].Replace(lb, _endBracket);

                if (!textParse[i].Contains(_startBracket) || !textParse[i].Contains(_endBracket))
                {
                    textParse[i] = textParse[i].Replace(_startBracket, "");
                    textParse[i] = textParse[i].Replace(_endBracket, "");
                }
            }

            return textParse;
        }
        private string TextAligmentSwith()
        {
            switch (_textAlignment)
            {
                case TextAlignment.Right:
                    return "Right";
                case TextAlignment.Center:
                    return "Center";
                case TextAlignment.Justify:
                    return "Justify";
            }

            return "Left";
        }
    }
}
