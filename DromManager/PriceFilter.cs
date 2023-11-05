using DromAutoTrader.Data;

namespace DromAutoTrader.DromManager
{
    public class PriceFilter
    {
        private AppContext _db = null!;
        public PriceFilter()
        {
            
        }

        public void FilterAndSaveByPrice(AdPublishingInfo adInfo)
        {
            using (var context = new AppContext())
            {
                try
                {
                    // Проверка на существующие объявления
                    var existingAds = context.AdPublishingInfo
                        .Where(a =>
                            a.Brand == adInfo.Brand &&
                            a.Artikul == adInfo.Artikul &&
                            a.KatalogName == adInfo.KatalogName)
                        .ToList();

                    if (existingAds.Count == 0)
                    {
                        // Если нет существующих объявлений, сохраняем новое объявление
                        context.AdPublishingInfo.Add(adInfo);
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
                            }
                        }
                    }

                    // Сохраняю изменения
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {

                    }
                }
                catch (Exception)
                {
                    // Обработка ошибки сохранения
                }
            }
        }
    }

}
