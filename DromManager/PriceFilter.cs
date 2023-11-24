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
                        if (adInfo.InputPrice != existingAd.InputPrice || adInfo.OutputPrice != existingAd.OutputPrice)
                        {
                            // Обновляем объект в базе только в тех случаях, где произошли изменения
                            if (adInfo.InputPrice != existingAd.InputPrice)
                            {
                                existingAd.InputPrice = adInfo.InputPrice;
                            }

                            if (adInfo.OutputPrice != existingAd.OutputPrice)
                            {
                                existingAd.OutputPrice = adInfo.OutputPrice;
                            }

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
                            existingAd.PriceBuy = "2"; // Ставлю флаг, что это изменённая цена / или изменилась накрутка, значит буду это учитывать при публикации объявлений
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
