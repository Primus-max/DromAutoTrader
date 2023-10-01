﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DromAutoTrader.Prices
{
    /// <summary>
    /// Класс для получения индекса поля в файле Excel по его имени.
    /// </summary>
    public class PriceFieldMapper
    {
        private readonly Dictionary<string, List<string>> fieldMappings;

        public PriceFieldMapper()
        {
            // Инициализируем словарь синонимов полей
            fieldMappings = new Dictionary<string, List<string>>
            {
                { PriceField.Brand.ToString(), new List<string> { "Brand", "Бренд" } },
                { PriceField.Artikul.ToString(), new List<string> { "Artikul", "Артикул" } },
                { PriceField.Description.ToString(), new List<string> { "Description", "Описание" } },
                { PriceField.PriceBuy.ToString(), new List<string> { "PriceBuy", "Цена закупки", "Цена"} },
                { PriceField.Count.ToString(), new List<string> { "Count", "Количество","Наличие" } },
                { PriceField.KatalogName.ToString(), new List<string> { "KatalogName", "Название каталога" } }
            };
        }

        /// <summary>
        /// Метод для сопоставления имени поля с индексом столбца.
        /// </summary>
        /// <param name="fieldName">Имя поля или его синоним.</param>
        /// <returns>Индекс столбца или -1, если сопоставление не найдено.</returns>
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
            if (Enum.TryParse(columnName, true, out PriceField field))
            {
                // Если удалось преобразовать имя поля в значение перечисления, 
                // то можно получить индекс, используя целочисленное значение перечисления.
                return (int)field;
            }

            // Если индекс не найден, можно вернуть -1 или другое значение по умолчанию
            return -1;
        }
    }
}
