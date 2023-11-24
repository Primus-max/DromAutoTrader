namespace DromAutoTrader.ViewModels
{
    public class ChannelAdInfoBuilder
    {
        private AdPublishingInfo? _adPublishingInfo = null;
        private readonly Channel? _channel = null;
        private readonly FormattedPrice? _price = null;
        private readonly string? _pricePath = null;

        public ChannelAdInfoBuilder(FormattedPrice price, Channel channel, string pricePath)
        {
            _price = price;
            _channel = channel;
            _adPublishingInfo = new();
            _pricePath = pricePath;
        }

        public async Task<AdPublishingInfo> Build()
        {
            if (_channel == null || _price == null || _pricePath == null)
                return new AdPublishingInfo();

            // Проверяем, что цена в прайсе не меньше чем в таблице накрутки цен
            decimal minTo = (decimal)(_channel?.PriceIncreases.Min(inc => (decimal)inc.From));

            if (_price.PriceBuy < minTo)
                return null;

            List<string> imagesPaths = new List<string>();
            string? namePrice = Path.GetFileName(_pricePath);

            // Создаю калькулятор, считаю цену с накруткой
            CalcPrice calcPrice = new();
            decimal calculatedPrice = calcPrice.Calculate(_price.PriceBuy, _channel?.PriceIncreases);

            // Проверяю, может такое объявление уже есть
            using var context = new AppContext();
            AdPublishingInfo existingAd = context.AdPublishingInfo.FirstOrDefault(ad => ad.Artikul == _price.Artikul);
            if (existingAd.DromeId == null) return null;

            if (existingAd.InputPrice != _price.PriceBuy || existingAd.OutputPrice != calculatedPrice)
                existingAd.PriceBuy = "2";

            if (existingAd != null)
            {
                // Если объявление уже существует, обновляем его данные
                existingAd.Brand = _price?.Brand;
                existingAd.Artikul = RemoveNonAlphanumeric(_price?.Artikul);
                existingAd.Description = _channel.Description;
                existingAd.KatalogName = _price?.KatalogName;
                existingAd.InputPrice = _price.PriceBuy;
                existingAd.OutputPrice = (decimal)Math.Round(calculatedPrice, 0);
                existingAd.AdDescription = _channel.Name;
                existingAd.Count = _price.Count;
                existingAd.DatePublished = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss");

                // Обновляем пути изображений
                SelectionImagesPathsService imagesPathsservice = new SelectionImagesPathsService();
                imagesPaths = await imagesPathsservice.SelectPaths(_price?.Brand, _price?.Artikul);

                existingAd.ImagesPaths = imagesPaths;

                existingAd.ImagesPath = string.Empty;
                existingAd.ImagesPath = string.Join(";", imagesPaths);

                // Сохраняем изменения в базе данных
                context.SaveChanges();

                return existingAd;
            }
            else
            {
                // Если объявление не существует, создаем новое
                AdPublishingInfo newAd = new AdPublishingInfo
                {
                    PriceName = namePrice,
                    Brand = _price?.Brand,
                    Artikul = RemoveNonAlphanumeric(_price?.Artikul),
                    Description = _channel.Description,
                    KatalogName = _price?.KatalogName,
                    InputPrice = _price.PriceBuy,
                    OutputPrice = (decimal)Math.Round(calculatedPrice, 0),
                    AdDescription = _channel.Name,
                    Count = _price.Count,
                    DatePublished = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Получаем пути изображений
                SelectionImagesPathsService imagesPathsservice = new SelectionImagesPathsService();
                imagesPaths = await imagesPathsservice.SelectPaths(_price?.Brand, _price?.Artikul);
                newAd.ImagesPaths = imagesPaths;
                newAd.ImagesPath = string.Join(";", imagesPaths);

                // Добавляем новое объявление в базу данных
                context.AdPublishingInfo.Add(newAd);
                context.SaveChanges();

                return newAd;
            }
        }


        // Убираю лишние символы из арткула
        public static string RemoveNonAlphanumeric(string input)
        {
            // Создаем регулярное выражение, которое соответствует всем символам, кроме букв и цифр
            string pattern = @"[^\p{L}\p{N}]";
            // Заменяем найденные символы пустой строкой
            string result = Regex.Replace(input, pattern, "");
            return result;
        }
    }
}
