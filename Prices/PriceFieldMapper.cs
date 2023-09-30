using System;
using System.Collections.Generic;
using System.Linq;

namespace DromAutoTrader.Prices
{
    public class PriceFieldMapper
    {
        private readonly Dictionary<string, List<string>> fieldMappings;

        public PriceFieldMapper()
        {
            // Инициализируем словарь синонимов полей
            fieldMappings = new Dictionary<string, List<string>>
            {
                { "Brand", new List<string> { "Brand", "Бренд" } },
                { "Artikul", new List<string> { "Artikul", "Артикул" } },
                { "Description", new List<string> { "Description", "Описание" } },
                { "PriceBuy", new List<string> { "PriceBuy", "Цена закупки" } },
                { "Count", new List<string> { "Count", "Количество" } },
                { "KatalogName", new List<string> { "KatalogName", "Название каталога" } }
            };
        }

        public int MapField(string fieldName)
        {
            // Поиск синонимов для заданного поля, учитывая регистр символов
            var mappedField = fieldMappings.FirstOrDefault(entry => entry.Value.Any(value => value.Equals(fieldName, StringComparison.OrdinalIgnoreCase)));

            // Если синонимы найдены, возвращаем индекс столбца
            // В противном случае возвращаем -1 или другое значение по умолчанию
            return mappedField.Key != null ? GetColumnIndexByName(mappedField.Key) : -1;
        }


        private int GetColumnIndexByName(string columnName)
        {
            // Здесь нужно получить индекс столбца по его имени.
            // Например, можно использовать словарь, который сопоставляет имена полей и индексы столбцов.

            // Пример словаря:
            var columnIndices = new Dictionary<string, int>
            {
                { "Brand", 0 },
                { "Artikul", 1 },
                { "Description", 2 },
                { "PriceBuy", 3 },
                { "Count", 4 },
                { "KatalogName", 5 },
            };

            // Попытка получить индекс по имени поля
            if (columnIndices.ContainsKey(columnName))
            {
                return columnIndices[columnName];
            }

            // Если индекс не найден, можно вернуть -1 или другое значение по умолчанию
            return -1;
        }
    }
}
