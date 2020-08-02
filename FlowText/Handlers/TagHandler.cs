using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowText
{
    // Хранит в себе имя тега и все его варианты, если они присутствуют
    /// <summary>
    /// Хранит имя тега, подварианты и позицию, на которой он закрывается.
    /// </summary>
    public class TagHandler
    {
        private List<VariantHandler> _variantsTag = new List<VariantHandler>();
        /// <summary>
        /// Устанавливает и возвращает имя тега.
        /// </summary>
        public string TagName;
        /// <summary>
        /// Возвращает все установленные варианты тега.
        /// </summary>
        public List<VariantHandler> VariantsTag { get => _variantsTag; }
        /// <summary>
        /// Позиция закрытия тега.
        /// </summary>
        public int TagEnd;

        /// <summary>
        /// Устанавливает подварианты тега. Резделителями для разных подтегов являются запятые.
        /// </summary>
        /// <param name="variants">Все подварианты тега строкой.</param>
        public void AddVariantsTag(string variants)
        {
            string rr = "&rr++++&";
            string rl = "&++++rl&";

            if (variants != null)
                if (variants != "") // Разделение всех вариантов тегов
                {
                    List<string> tagVar = variants.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    foreach(var el in tagVar)
                    {
                        variants = el;

                        if (!variants.Contains("="))
                            variants += "=";

                        // Добавление меток знака "="
                        variants = variants.Replace("=", rr + "=");
                        variants = variants.Replace("=", "=" + rl);

                        List<string> varVol = variants.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        // Проходит по всем элементам и соеденяет подтег и значение для держателя 
                        for (int i = 0; i < varVol.Count; i++)
                            if (varVol[i].Contains(rr))
                                for (int j = i + 1; j < varVol.Count; j++)
                                {
                                    VariantHandler tempHandler = new VariantHandler();

                                    if (varVol[j].Contains(rr))
                                    {
                                        string tag = varVol[j].Replace(rr, "=").Trim();

                                        tempHandler.SetVariantTag(tag);
                                        _variantsTag.Add(tempHandler);

                                        break;
                                    }
                                    if (varVol[j].Contains(rl))
                                    {
                                        string tag = varVol[i].Replace(rr, "").Trim();
                                        string value = varVol[j].Replace(rl, "").Trim();
                                        tag += "=" + value;

                                        tempHandler.SetVariantTag(tag);
                                        _variantsTag.Add(tempHandler);

                                        break;
                                    }

                                }
                    }
                }
        }
        /// <summary>
        /// Добавляет в конец разбитые подварианты.
        /// </summary>
        /// <param name="variantHandlers">Другие разбитые подварианты.</param>
        public void AddVariantsTag(List<VariantHandler> variantHandlers)
        {
            _variantsTag.AddRange(variantHandlers);
        }
    }
}
