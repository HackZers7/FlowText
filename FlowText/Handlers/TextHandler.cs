using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using FlowText.TagsCreator;

namespace FlowText
{
    // Структура выполняющая хранение тега и текста который в ней находиться
    public class TextHandler
    {
        public List<TagHandler> Tags = new List<TagHandler>();

        public string Text;

        public double BaseFontSize;

        public bool FlagBreak = false;

        /// <summary>
        /// Метод собирает текст на осневе данных, которые ему уже были предоставлены.
        /// </summary>
        /// <returns>Возвращаяет уже собранный текст.</returns>
        public string Parse(List<ICreatorOneTags> oneTags, List<ICreatorClosingTags> closingTags)
        {
            if (Text == null)
                return "";
            try
            {
                Tags = ReViwTags();

                string run = @"<Run ";
                FlagBreak = false;

                foreach (TagHandler el in Tags) // Запуск поиска просмотра тегов
                {
                    foreach (var oTags in oneTags)
                        if (el.TagName.Replace("&OneTagsSplitTag&", "").ToLower().Trim() == oTags.TagName)
                        {
                            FlagBreak = true;
                            run = oTags.ParseText(run, el, this);
                            return run;
                        }

                    foreach (var cTags in closingTags)
                        if (el.TagName.ToLower().Trim() == cTags.TagName)
                            run = cTags.ParseText(run, el, this);

                    run += " ";
                    if (FlagBreak) return run;
                }

                return run.Remove(run.Length - 1, 1) + @">" + Text + @"</Run> ";
            }
            catch (Exception e) { return $"<Run>Ошибка: Проверьте правельность написания тегов.  {e}</Run>"; }
        }
        /// <summary>
        /// Производит просмотр и удаление похожих заданных тегов. Предпочтение отдается тегам в конце списка.
        /// </summary>
        /// <returns></returns>
        public List<TagHandler> ReViwTags()
        {
            if (Tags.Count < 2) return Tags;

            List<TagHandler> tempTags = new List<TagHandler>();

            for (int i = Tags.Count - 1; i >= 0; i--)
            {
                bool flag = true;

                foreach (var el in tempTags)
                {
                    if (Tags[i].TagName.ToLower() == el.TagName.ToLower())
                    {
                        flag = false;

                        foreach (var varEl in Tags[i].VariantsTag)
                        {
                            bool flag2 = true;

                            for (int j = 0; j < el.VariantsTag.Count; j++)
                            {
                                string tempVarName1 = varEl.Variant.ToLower();
                                string tempVarName2 = el.VariantsTag[j].Variant.ToLower();

                                tempVarName1 = DelNew(tempVarName1);
                                tempVarName2 = DelNew(tempVarName2);

                                if (tempVarName1 == tempVarName2)
                                    flag2 = false;
                            }

                            if (flag2)
                                el.VariantsTag.Add(varEl);
                        }
                    }
                }

                if (flag)
                {
                    TagHandler tempHandler = new TagHandler();
                    tempHandler.TagName = Tags[i].TagName;
                    tempHandler.TagEnd = Tags[i].TagEnd;
                    tempHandler.AddVariantsTag(Tags[i].VariantsTag);

                    tempTags.Add(tempHandler);
                }
            }

            return tempTags;
        }

        private string DelNew(string s)
        {
            if (s.Contains("new"))
            {
                int pos = s.IndexOf("new") + 3;
                s = s.Remove(0, pos);
            }

            return s;
        }
    }
}
