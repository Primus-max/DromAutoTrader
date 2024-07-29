using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel;

using System.Globalization;

namespace DromAutoTrader.Prices
{
    /// <summary>
    /// Класс для обработки прайс-листов из Excel-файлов.
    /// </summary>
    public class PriceProcessor
    {
        private PriceFieldMapper fieldMapper;
        private readonly Logger _logger;

        public PriceProcessor()
        {
            fieldMapper = new PriceFieldMapper();
            _logger = new LoggingService().ConfigureLogger();
        }

        /// <summary>
        /// Обработка прайс-листа из Excel-файла.
        /// </summary>
        /// <param name="filePath">Путь к Excel-файлу.</param>
        /// <returns>Коллекция обработанных прайс-листов.</returns>
        public PriceList ProcessExcelPrice(string filePath)
        {
            var priceList = new PriceList(); // Создаем коллекцию для хранения обработанных прайс-листов.

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {                
                var worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Предполагаем, что данные находятся в первом листе.

                if (worksheet == null)
                {
                    _logger.Error("Не удалось получить листы при парсинге прайса");
                    return new PriceList();
                }

                int rowCount = worksheet.Dimension.End.Row;

                // Получаем индексы колонок из файла
                var columnIndexes = GetColumnIndexes(worksheet);

                for (int row = 2; row <= rowCount; row++) // Предполагаем, что первая строка - заголовки и начинаем с 2-й строки.
                {
                    var priceItem = new FormattedPrice();

                    foreach (var columnIndexInFile in columnIndexes.Values) // Используем значения (индексы колонок из файла Excel)
                    {
                        if (columnIndexInFile != -1)
                        {
                            string cellValue = worksheet.Cells[row, columnIndexInFile].Text;
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                // Определяем, куда ложить значение на основе индекса колонки из файла Excel
                                foreach (var field in columnIndexes.Keys)
                                {
                                    if (columnIndexes[field] == columnIndexInFile)
                                    {
                                        SetPropertyByFieldName(priceItem, field, cellValue);
                                        break; // Прерываем цикл, так как поле уже найдено и установлено
                                    }
                                }
                            }
                            else
                            {
                                _logger.Error($"Не удалось получить значение в ячейке прайса при парсинге {filePath}");
                            }
                        }
                    }


                    // Добавляем обработанный элемент в коллекцию.
                    priceList.Add(priceItem);
                }
            }

            return priceList;
        }


        // Метод для получения индексов колонок из файла при ассоциации колонок из нашего прайса
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
                    value = value.Replace(",", ".");
                    if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal priceBuyValue))
                    {
                        priceItem.PriceBuy = priceBuyValue;
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
