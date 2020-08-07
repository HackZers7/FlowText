using FlowText.TagsCreator;
using System;
using System.Collections.Generic;

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
                Tags = ReViwVariants();
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

                BaseFontSize = BaseFontSize <= 0 ? 1 : BaseFontSize;

                run += "FontSize='" + BaseFontSize + "' ";
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
                                string tempVarName1 = varEl.Variant.ToLower().Trim();
                                string tempVarName2 = el.VariantsTag[j].Variant.ToLower().Trim();

                                tempVarName1 = DelNew(tempVarName1);
                                tempVarName2 = DelNew(tempVarName2);

                                if (tempVarName1 == tempVarName2)
                                    flag2 = false;
                            }

                            if (flag2)
                            {
                                if (varEl.Variant.ToLower().Trim() == "size")
                                    el.VariantsTag.Insert(0, varEl);
                                else el.VariantsTag.Add(varEl);
                            }
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

        public List<TagHandler> ReViwVariants()
        {
            List<TagHandler> tempTags = new List<TagHandler>();

            foreach (var el in Tags)
            {
                List<VariantHandler> variantHandlers = new List<VariantHandler>();

                foreach (var variant in el.VariantsTag)
                {
                    string tempVarName1 = DelNew(variant.Variant.ToLower().Trim());
                    bool flag = true;

                    foreach (var nonVariants in variantHandlers)
                    {
                        string tempVarName2 = DelNew(nonVariants.Variant.ToLower().Trim());

                        if (tempVarName1 == tempVarName2)
                            flag = false;
                    }

                    if (flag)
                        variantHandlers.Add(variant);
                }

                TagHandler tempHandler = new TagHandler();
                tempHandler.TagName = el.TagName;
                tempHandler.TagEnd = el.TagEnd;
                tempHandler.AddVariantsTag(variantHandlers);

                tempTags.Add(tempHandler);
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
