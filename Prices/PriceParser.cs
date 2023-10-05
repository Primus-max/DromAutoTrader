using OfficeOpenXml;

namespace DromAutoTrader.Prices
{
    public class PriceParser
    {
        public FormattedPrice ParsePriceItem(ExcelWorksheet worksheet)
        {
            // Здесь реализуйте логику парсинга данных из Excel и создания объекта PriceItem
            // Пример:
            int row = 2; // Начнем с второй строки, если заголовки в первой строке
            FormattedPrice priceItem = new FormattedPrice();

            priceItem.Brand = worksheet.Cells[row, 1].Text;
            priceItem.Artikul = worksheet.Cells[row, 2].Text;
            priceItem.Description = worksheet.Cells[row, 3].Text;
            priceItem.PriceBuy = decimal.Parse(worksheet.Cells[row, 4].Text);
            priceItem.Count = int.Parse(worksheet.Cells[row, 5].Text);
            priceItem.KatalogName = worksheet.Cells[row, 6].Text;

            return priceItem;
        }
    }
}
