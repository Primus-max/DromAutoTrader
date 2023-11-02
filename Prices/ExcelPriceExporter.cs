using OfficeOpenXml;
using System.IO;


namespace DromAutoTrader.Prices
{
    /// <summary>
    /// Класс формирования прайса из размещенных объявлений с выходными ценами
    /// </summary>
    public class ExcelPriceExporter
    {
        /// <summary>
        /// Метод экспорта прайса с выходными ценами
        /// </summary>
        /// <param name="prices"></param>
        /// <returns>Путь к прайсу</returns>
        public string ExportPricesToExcel(List<AdPublishingInfo> prices)
        {
            string channelName = string.Empty;

            // Создаем новый пакет Excel
            using (var package = new ExcelPackage())
            {
                // Добавляем новый лист в пакет
                var worksheet = package.Workbook.Worksheets.Add("Prices");

                // Заголовки столбцов
                worksheet.Cells[1, 1].Value = "Канал";
                worksheet.Cells[1, 2].Value = "Бренд";
                worksheet.Cells[1, 3].Value = "Артикул";
                worksheet.Cells[1, 4].Value = "Описание";
                worksheet.Cells[1, 5].Value = "Цена";

                bool isOnce = false;
                // Заполняем данные из списка
                for (int i = 0; i < prices.Count; i++)
                {
                    var price = prices[i];

                    if (price.IsArchived == true) continue; // Если объявление в архиве
                    if(price.PriceBuy != 1) continue; // Если не было размещено

                    worksheet.Cells[i + 2, 1].Value = price.AdDescription;
                    worksheet.Cells[i + 2, 2].Value = price.Brand;
                    worksheet.Cells[i + 2, 3].Value = price.Artikul;
                    worksheet.Cells[i + 2, 4].Value = price.KatalogName;
                    worksheet.Cells[i + 2, 5].Value = price.OutputPrice;

                    if (!isOnce)
                    {
                        channelName = price?.AdDescription;
                        isOnce = true;
                    }
                }

                // Сохраняем файл Excel
                var currentDate = DateTime.Now;
                var fileName = $"{currentDate:yyyyMMddHHmmss}_{channelName}.xlsx";
                var filePath = Path.Combine("ourprices", fileName);

                // Если папки "ourprices" не существует, создаем ее
                if (!Directory.Exists("ourprices"))
                {
                    Directory.CreateDirectory("ourprices");
                }

                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);

                return filePath;
            }
        }
    }

}
