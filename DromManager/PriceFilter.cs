namespace DromAutoTrader.DromManager
{
    public class PriceFilter
    {
        private readonly Logger _logger;
        public PriceFilter()
        {
            _logger = new LoggingService().ConfigureLogger();
        }

        public void FilterAndSaveByPrice(AdPublishingInfo adInfo)
        {
            using var context = new AppContext();
            try
            {
                // Проверка на существующие объявления
                var existingAds = context.AdPublishingInfo
                    .Where(a =>
                        a.Brand.ToLower() == adInfo.Brand.ToLower() &&
                        a.Artikul == adInfo.Artikul &&
                        a.KatalogName == adInfo.KatalogName)
                    .ToList();

                if (existingAds.Count == 0)
                {
                    // Если нет существующих объявлений, сохраняем новое объявление
                    try
                    {
                        context.AdPublishingInfo.Add(adInfo);
                    }
                    catch (Exception ex)
                    {
                        string message = $"Ошибка в FilterAndSaveByPrice, не удалось добавить {adInfo.Artikul} || {adInfo.Brand} {ex.Message}";
                        _logger.Error(message);
                    }
                }
                else
                {
                    foreach (var existingAd in existingAds)
                    {
                        bool inputPriceChanged = adInfo.InputPrice < existingAd.InputPrice;
                        bool outputPriceChanged = adInfo.OutputPrice != existingAd.OutputPrice;

                        if (inputPriceChanged || (outputPriceChanged && adInfo.OutputPrice < existingAd.OutputPrice))
                        {
                            // Обновляем объект в базе только если изменилась InputPrice или OutputPrice стала меньше
                            if (inputPriceChanged)
                            {
                                existingAd.InputPrice = adInfo.InputPrice;
                            }

                            if (outputPriceChanged && adInfo.OutputPrice < existingAd.OutputPrice)
                            {
                                existingAd.OutputPrice = adInfo.OutputPrice;
                            }

                            // Устанавливаем флаг изменения цены или накрутки
                            existingAd.PriceBuy = "2";

                            // Общие обновления, которые применяются только при изменении цены или накрутки
                            existingAd.Brand = adInfo.Brand;
                            existingAd.Artikul = adInfo.Artikul;
                            existingAd.DatePublished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            existingAd.AdDescription = adInfo.AdDescription;
                            existingAd.Count = adInfo.Count;
                            existingAd.ImagesPath = adInfo.ImagesPath;
                            existingAd.IsArchived = adInfo.IsArchived;
                            existingAd.KatalogName = adInfo.KatalogName;
                            existingAd.PriceName = adInfo.PriceName;
                            existingAd.Description = adInfo.Description;
                        }
                    }
                }

                // Сохраняю изменения
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    string message = $"Ошибка в FilterAndSaveByPrice {ex.Message}";
                    _logger.Error(message);
                }
            }
            catch (Exception ex)
            {
                string message = $"Ошибка в FilterAndSaveByPrice {ex.Message}";
                _logger.Error(message);
            }
        }
    }

}
