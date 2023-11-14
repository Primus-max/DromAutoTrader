namespace DromAutoTrader.DromManager
{
    public class PriceFilter
    {
        public PriceFilter()
        {

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
                        string message = $"ОШибка в FilterAndSaveByPrice, не удалось добавить {adInfo.Artikul} || {adInfo.Brand} {ex.Message}";
                        Console.WriteLine(message);
                    }
                }
                else
                {
                    foreach (var existingAd in existingAds)
                    {
                        // Проверяю цену, если у нового объявления цена лучше чем у старого, сохраняем новый объект
                        if (adInfo.InputPrice < existingAd.InputPrice)
                        {
                            // Обновляю объект в базе
                            existingAd.InputPrice = adInfo.InputPrice;
                            existingAd.OutputPrice = adInfo.OutputPrice;
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
                            existingAd.PriceBuy = "2"; // Ставлю флаг что это изменённая цена, значит буду это учитывать при публикации объявленй
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
                    string message = $"ОШибка в FilterAndSaveByPrice {ex.Message}";
                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                string message = $"ОШибка в FilterAndSaveByPrice {ex.Message}";
                Console.WriteLine(message);
            }
        }
    }

}
