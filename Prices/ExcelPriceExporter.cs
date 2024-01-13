using OfficeOpenXml;
using System.Linq;


namespace DromAutoTrader.Prices
{
    /// <summary>
    /// Класс формирования прайса из размещенных объявлений с выходными ценами
    /// </summary>
    public class ExcelPriceExporter
    {
        private int cellCount = 0;

        /// <summary>
        /// Метод экспорта прайса с выходными ценами
        /// </summary>
        /// <param name="price">Объект объявления</param>
        /// <returns>Путь к прайсу</returns>
        public async Task<string> ExportPriceToExcel(AdPublishingInfo price)
        {
            if (price == null )
                return null;

            // Получаем или создаем файл Excel
            string filePath = GetFilePath();
            FileInfo excelFile = new FileInfo(filePath);

            try
            {
                using (var package = new ExcelPackage(excelFile))
                {
                    // Получаем лист, если он существует, или создаем новый
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Prices");

                    // Заголовки столбцов (если лист был создан только что)
                    if (worksheet.Dimension == null)
                    {
                        worksheet.Cells[1, 1].Value = "Бренд";
                        worksheet.Cells[1, 2].Value = "Артикул";
                        worksheet.Cells[1, 3].Value = "Описание";
                        worksheet.Cells[1, 4].Value = "Цена";
                        worksheet.Cells[1, 5].Value = "Состояние";
                        worksheet.Cells[1, 6].Value = "Наличие";
                        worksheet.Cells[1, 7].Value = "Фото";
                    }

                    // Заполняем данные
                    var imageLocalPath = price?.ImagesPath?.Split(";");
                    List<string> images = new List<string>();
                    string uploadedImagePathUrl;
                    ImageBanUploaderService uploaderService = new ();
                    foreach (var image in imageLocalPath)
                    {
                        uploadedImagePathUrl = await uploaderService.UploadImageFromFile(image);
                        images.Add(uploadedImagePathUrl);

                        await Task.Delay(200);
                    }
                    

                    worksheet.Cells[cellCount + 2, 1].Value = price.Brand;
                    worksheet.Cells[cellCount + 2, 2].Value = price.Artikul;
                    worksheet.Cells[cellCount + 2, 3].Value = price.KatalogName;
                    worksheet.Cells[cellCount + 2, 4].Value = price.OutputPrice;
                    worksheet.Cells[cellCount + 2, 5].Value = "Новый";
                    worksheet.Cells[cellCount + 2, 6].Value = "В наличии";
                    worksheet.Cells[cellCount + 2, 7].Value = string.Join(",", images);

                    cellCount++;

                    // Сохраняем изменения в существующем файле
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось добавить запись в прайс: {ex.Message}");
            }

            return filePath;
        }

        private string GetFilePath()
        {
            // Генерируем или получаем путь к файлу
            var currentDate = DateTime.Now;
            var fileName = $"{currentDate:yyyyMMdd}_Prices.xlsx";
            var filePath = Path.Combine("ourprices", fileName);

            // Если папки "ourprices" не существует, создаем ее
            if (!Directory.Exists("ourprices"))
            {
                Directory.CreateDirectory("ourprices");
            }

            return filePath;
        }
    }
}
