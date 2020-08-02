namespace FlowText
{
    /// <summary>
    /// Хранит в себе разделенный подвариант.
    /// </summary>
    public struct VariantHandler
    {
        private string _variant;
        private string _value;
        /// <summary>
        /// Возвращяет имя подварианта.
        /// </summary>
        public string Variant { get => _variant; }
        /// <summary>
        /// Возвращает значение подварианта.
        /// </summary>
        public string Value { get => _value; }
        /// <summary>
        /// Задает подвариант.
        /// </summary>
        /// <param name="var"> Подвариант со значением, через знак "=".</param>
        public void SetVariantTag(string var)
        {
            _value = "";

            // Обязательные преобразование, если тег должен содержать атрибуты  
            string[] count = var.Split((new char[] { '=' }));

            if (count.Length >= 2) // Если в значении тега есть мнимые пробелы заменить их
                _value = count[1].Trim().Replace("_", " ");

            _variant = count[0].Trim();
        }
        public void SetVariantTag(VariantHandler variantHandler)
        {
            _variant = variantHandler._variant;
            _value = variantHandler._value;
        }
    }
}
