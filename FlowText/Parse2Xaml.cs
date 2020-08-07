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
        public static FlowDocument ParseTextToXaml(string s, double baseFontSize = 14, TextAlignment textAlignment = TextAlignment.Left)
        {
            return ParseTextToXaml(s, new List<ICreatorOneTags>(), new List<ICreatorClosingTags>(), baseFontSize, textAlignment);
        }

        /// <summary>
        /// Преобразует текст из формата "При[Font size=3 family=Font_A]вет [/b]мир[/b][/Font]!"
        /// в формат пригодный для записи в потоковые документы.
        /// Для обозначения пробелов внутри значения тега использовать _.
        /// Внимание закрывайте все теги перед br - переносом на следующую строчку.
        /// </summary>
        /// <param name="s">Строка для сборки</param>
        /// <param name="baseFontSize">Базовый размер шрифта</param>
        /// <param name="textAlignment">Выравнивание всего текста</param>
        public static FlowDocument ParseTextToXaml(string s, List<ICreatorOneTags> oneTags, List<ICreatorClosingTags> closingTags, double baseFontSize = 14, TextAlignment textAlignment = TextAlignment.Left)
        {
            var tags = CDefaultTags();
            var onetags = CDefaultOneTags();

            tags.AddRange(closingTags);
            onetags.AddRange(oneTags);

            try
            {
                // Задание значений для открывающегося и закрывающегося тега
                string startBracket = "[";
                string endBracket = "]";

                s = s.Replace(@"\" + startBracket, "$rb");
                s = s.Replace(@"\" + endBracket, "$lb");

                string doneText = string.Format("<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
                " xmlns:x = 'http://schemas.microsoft.com/winfx/2006/xaml'" +
                " xmlns:d = 'http://schemas.microsoft.com/expression/blend/2008'" +
                " xmlns:mc = 'http://schemas.openxmlformats.org/markup-compatibility/2006'" +
                $" FontSize='{baseFontSize}'" +
                " xml:space='preserve'> ");

                List<string> textParce = TagBR(SplitText(s, startBracket, endBracket), startBracket, endBracket); // Разделение текста по тегу br

                foreach (string el in textParce)
                {
                    List<string> tempText = OneTagsParse(SplitText(el, startBracket, endBracket), onetags, startBracket, endBracket);

                    if (tempText.Count > 1)
                        for (int i = 0; i < tempText.Count;)
                        {
                            if (tempText[i].Trim() == "" && i + 1 != tempText.Count)
                            {
                                int pos = 0;

                                for (int j = i + 1; j < tempText.Count; j++)
                                    if (!tempText[j].Contains(startBracket) && !tempText[j].Contains(endBracket))
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

                    doneText += $"<Paragraph TextAlignment='{TextAligmentSwith(textAlignment)}'>" +
                        TwoTagsParse(tempText, baseFontSize, startBracket, endBracket, onetags, tags) +  // Преобразование в код Xaml на основе двух тегов
                        "</Paragraph>";
                }

                doneText += "</FlowDocument>";

                doneText = doneText.Replace(@"$rb", startBracket);
                doneText = doneText.Replace(@"$lb", endBracket);

                //MessageBox.Show(doneText);

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
            catch (Exception e)
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
        private static List<string> TagBR(List<string> s, string startBracket, string endBracket)
        {
            string tag = "br"; // Тег, который будет распозноваться как перенос на следующую строчку
            try
            {
                List<int> tagBr = new List<int>();

                // Добавление тега br в начало и конец строкового массива
                List<string> temp = new List<string> { $"{startBracket}{tag}{endBracket}" };
                temp.AddRange(s);
                s.Clear();
                s.AddRange(temp);
                s.Add($"{startBracket}{tag}{endBracket}");

                for (int i = 0; i < s.Count; i++) // Поиск тегов br и разделение по ним
                    if (s[i].ToLower() == $"{startBracket}{tag}{endBracket}".ToLower())
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

                    string tempStr = "";
                    foreach (string el in str) // Перемещение строк в переменную, для передачи значения
                        tempStr += el;

                    returnString.Add(tempStr);
                }

                return returnString;
            }
            catch { return new List<string> { $"Ошибка: Произошла ошибка при разделении на строки, пожалуйста проверьте все теги {tag}." }; }
        }

        private static List<string> OneTagsParse(List<string> s, List<ICreatorOneTags> oneTags, string startBracket, string endBracket)
        {
            try
            {
                for (int i = 0; i < s.Count; i++)
                {
                    if (s[i].Contains(startBracket) && s[i].Contains(endBracket))
                    {
                        if (s[i].StartsWith($"{startBracket}!--") && s[i].EndsWith($"--{endBracket}"))
                        {
                            s[i] = "";
                            continue;
                        }

                        s[i] = s[i].Trim();

                        TagSplitter(s[i], out string tagName, out string tagVarl, startBracket, endBracket);

                        string[] tagVar = new string[0];

                        if (tagVarl != "" && tagVarl != null) // Разделение всех вариантов тега
                            tagVar = tagVarl.Split(new char[] { ' ' });

                        foreach (var el in oneTags)
                            if (el.TagName == tagName.ToLower().Trim())
                            {
                                s[i] = "[&OneTagsSplitTag&" + tagName + " " + tagVarl + "]";
                                s.Insert(i + 1, "Текст заполнения внутренностей тега, будет удален.");
                                s.Insert(i + 2, "[/&OneTagsSplitTag&" + tagName + "]");
                            }

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

        private static string TwoTagsParse(List<string> s, double baseFontSize, string startBracket, string endBracket,
            List<ICreatorOneTags> oneTags, List<ICreatorClosingTags> tags)
        {
            // Произвоиться запись тега вместе с текстом, для последующего преобразования. Текст записывается по порядку следования
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
                    if (s[i].Contains(startBracket) && s[i].Contains(endBracket))
                        pos1 = i;
                    else // Если не содержит, то текст записывается в держатель вместе со всеми открытыми тегами
                    {
                        TextHandler tempTextHandler = new TextHandler();
                        tempTextHandler.Text = s[i];
                        foreach (TagHandler el in openTags)
                            tempTextHandler.Tags.Add(el);

                        tempTextHandler.BaseFontSize = baseFontSize;

                        textHandler.Add(tempTextHandler);

                        continue;
                    }

                    // Проверка на закрывающийся тег
                    if (s[i].Replace(" ", "").StartsWith("[/"))
                        continue;


                    // Резделение тега и его вариантов, раделителями являются пробелы
                    TagSplitter(s[pos1], out string tagName, out string tagVar, startBracket, endBracket);

                    int pos2 = -1;

                    int countSkip = 0;

                    // Поиск закрывающегося тега
                    for (int j = pos1 + 1; j < s.Count; j++)
                    {
                        TagSplitter(s[j], out string tempName, out string none, startBracket, endBracket);

                        if ($"{startBracket}{tagName}{endBracket}".ToLower() == $"{startBracket}{tempName}{endBracket}".ToLower())
                            countSkip++;

                        if ($"{startBracket}/{tagName}{endBracket}".ToLower() == s[j].ToLower().Replace(" ", ""))
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
                doneText += el.Parse(oneTags, tags);

            return doneText;
        }

        private static void TagSplitter(string s, out string tagName, out string tagVar, string startBracket, string endBracket)
        {
            tagName = s;
            tagVar = "";

            if (!tagName.Contains("[") && !tagName.Contains("]"))
                return;

            tagName = tagName.Remove(0, startBracket.Length);
            tagName = tagName.Remove(tagName.Length - endBracket.Length, endBracket.Length);

            tagName = tagName.Trim();

            // Проверка на нахождение вариантов
            if (tagName.Contains(" "))
            {
                int posp = tagName.IndexOf(" ");
                tagVar = tagName.Substring(posp + 1, tagName.Length - posp - 1);
                tagName = tagName.Substring(0, posp);
            }
        }

        private static List<string> SplitText(string s, string startBracket, string endBracket)
        {
            string rb = "&pR+++&";
            string lb = "&pL+++&";
            string[] stringSeparators = { startBracket, endBracket };

            s = s.Replace(startBracket, startBracket + rb);
            s = s.Replace(endBracket, lb + endBracket);

            List<string> textParse = s.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < textParse.Count; i++)
            {
                textParse[i] = textParse[i].Replace(rb, startBracket);
                textParse[i] = textParse[i].Replace(lb, endBracket);

                if (!textParse[i].Contains(startBracket) || !textParse[i].Contains(endBracket))
                {
                    textParse[i] = textParse[i].Replace(startBracket, "");
                    textParse[i] = textParse[i].Replace(endBracket, "");
                }
            }

            return textParse;
        }
        private static string TextAligmentSwith(TextAlignment textAlignment)
        {
            switch (textAlignment)
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
