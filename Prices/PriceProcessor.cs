using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DromAutoTrader.Prices
{
    public class PriceProcessor
    {
        private PriceFieldMapper fieldMapper;
        private PriceParser parser;
        private PriceList priceList;

        public PriceProcessor()
        {
            fieldMapper = new PriceFieldMapper();
            parser = new PriceParser();
            priceList = new PriceList();
        }

        public PriceList ProcessExcelPrice(string filePath)
        {
            var priceList = new PriceList(); // Создаем коллекцию для хранения обработанных прайс-листов.

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Предполагаем, что данные находятся в первом листе.

                int rowCount = worksheet.Dimension.End.Row;
                int colCount = worksheet.Dimension.End.Column;

                // Получаем индексы колонок из файла
                var columnIndexes = GetColumnIndexes(worksheet);

                for (int row = 2; row <= rowCount; row++) // Предполагаем, что первая строка - заголовки и начинаем с 2-й строки.
                {
                    var priceItem = new FormattedPrice();

                    foreach (var field in columnIndexes.Keys)
                    {
                        int columnIndex = columnIndexes[field];
                        if (columnIndex != -1)
                        {
                            string cellValue = worksheet.Cells[row, columnIndex].Text;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                SetPropertyByFieldName(priceItem, field, cellValue);
                            }
                            else
                            {
                                // Обработка ошибки или установка значения по умолчанию.
                            }
                        }
                    }

                    // Добавляем обработанный элемент в коллекцию.
                    priceList.Add(priceItem);
                }
            }

            return priceList;
        }

        
        // Метод для получения индексов колонок из файла
        private Dictionary<PriceField, int> GetColumnIndexes(ExcelWorksheet worksheet)
        {
            var columnIndexes = new Dictionary<PriceField, int>();
            int colCount = worksheet.Dimension.End.Column;

            for (int col = 1; col <= colCount; col++)
            {
                string columnName = worksheet.Cells[1, col].Text;
                int columnIndex = fieldMapper.MapField(columnName);

                if (columnIndex != -1)
                {
                    columnIndexes[(PriceField)columnIndex] = col;
                }
            }

            return columnIndexes;
        }


        // Метод для установки свойства priceItem на основе имени поля
        private void SetPropertyByFieldName(FormattedPrice priceItem, PriceField field, string value)
        {
            switch (field)
            {
                case PriceField.Brand:
                    priceItem.Brand = value;
                    break;
                case PriceField.Artikul:
                    priceItem.Artikul = value;
                    break;
                case PriceField.Description:
                    priceItem.Description = value;
                    break;
                case PriceField.PriceBuy:
                    if (decimal.TryParse(value, out decimal priceBuyValue))
                    {
                        priceItem.PriceBuy = priceBuyValue;
                    }
                    else
                    {
                        // Обработка ошибки при парсинге цены.
                    }
                    break;
                case PriceField.Count:
                    if (int.TryParse(value, out int countValue))
                    {
                        priceItem.Count = countValue;
                    }
                    else
                    {
                        // Обработка ошибки при парсинге количества.
                    }
                    break;
                case PriceField.KatalogName:
                    priceItem.KatalogName = value;
                    break;
                default:
                    // Неизвестное поле, обработка ошибки.
                    break;
            }
        }

    }
}
