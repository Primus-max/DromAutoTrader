using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

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
                var worksheet = package.Workbook.Worksheets[0]; // Предположим, что данные находятся в первом листе.

                // Здесь вы можете использовать EPPlus для извлечения данных из Excel и применить сопоставление полей.
                int rowCount = worksheet.Dimension.End.Row;

                // Предположим, что у вас есть сопоставление синонимов полей.
                var fieldMapper = new PriceFieldMapper();

                for (int row = 2; row <= rowCount; row++) // Предполагаем, что первая строка - заголовки.
                {
                    var priceItem = new FormattedPrice();

                    // Используем fieldMapper для сопоставления синонимов полей с номерами столбцов.
                    var brandColumn = fieldMapper.MapField("brand");
                    var artikulColumn = fieldMapper.MapField("artikul");
                    var descriptionColumn = fieldMapper.MapField("description");
                    var priceBuyColumn = fieldMapper.MapField("price_buy");
                    var countColumn = fieldMapper.MapField("count");
                    var katalogNameColumn = fieldMapper.MapField("katalog_name");

                    string brandCellValue = worksheet.Cells[row, brandColumn].Text;
                    if (!string.IsNullOrEmpty(brandCellValue))
                    {
                        priceItem.Brand = brandCellValue;
                    }
                    else
                    {
                        // Обработка ошибки или установка значения по умолчанию для Brand.
                    }

                    string artikulCellValue = worksheet.Cells[row, artikulColumn].Text;
                    if (!string.IsNullOrEmpty(artikulCellValue))
                    {
                        priceItem.Artikul = artikulCellValue;
                    }
                    else
                    {
                        // Обработка ошибки или установка значения по умолчанию для Artikul.
                    }

                    string descriptionCellValue = worksheet.Cells[row, descriptionColumn].Text;
                    if (!string.IsNullOrEmpty(descriptionCellValue))
                    {
                        priceItem.Description = descriptionCellValue;
                    }
                    else
                    {
                        // Обработка ошибки или установка значения по умолчанию для Description.
                    }

                    // Преобразование цены из строки в decimal
                    if (decimal.TryParse(worksheet.Cells[row, priceBuyColumn].Text, out decimal priceBuyValue))
                    {
                        priceItem.PriceBuy = priceBuyValue;
                    }
                    else
                    {
                        // Обработка ошибки при парсинге цены.
                        // Можете установить значение по умолчанию или выполнить другие действия для PriceBuy.
                    }

                    // Преобразование количества из строки в int
                    if (int.TryParse(worksheet.Cells[row, countColumn].Text, out int countValue))
                    {
                        priceItem.Count = countValue;
                    }
                    else
                    {
                        // Обработка ошибки при парсинге количества.
                        // Можете установить значение по умолчанию или выполнить другие действия для Count.
                    }

                    string katalogNameCellValue = worksheet.Cells[row, katalogNameColumn].Text;
                    if (!string.IsNullOrEmpty(katalogNameCellValue))
                    {
                        priceItem.KatalogName = katalogNameCellValue;
                    }
                    else
                    {
                        // Обработка ошибки или установка значения по умолчанию для KatalogName.
                    }

                    // Добавляем обработанный элемент в коллекцию.
                    priceList.Add(priceItem);
                }

            }

            return priceList;
        }

    }
}
