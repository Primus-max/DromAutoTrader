using DromAutoTrader.Data;

namespace DromAutoTrader.DromManager
{
    public class PriceFilter
    {
        private AppContext _db = null!;
        public PriceFilter()
        {
            InitializeDatabase();
        }

        public void FilterAndSaveByPrice(AdPublishingInfo adInfo)
        {
            // Проверка на существующие объявления
            var existingAds = _db.AdPublishingInfo
                .Where(a =>
                        a.Brand == adInfo.Brand &&
                        a.Artikul == adInfo.Artikul &&
                        a.KatalogName == adInfo.KatalogName
                        )
                .ToList();
                //.Where(a => DateTime.Parse(a.DatePublished).Date == DateTime.Now.Date)
                //.ToList();


            if (existingAds.Count == 0)
            {
                // Если нет существующих объявлений, сохраняем новое объявление
                SaveAd(adInfo);
            }
            else
            {
                foreach (var existingAd in existingAds)
                {
                    // Проверяю цену, если у нового объявления цена лучше чем у старого, сохраняем новый объект
                    if (adInfo.InputPrice < existingAd.InputPrice)
                    {    
                        // Обновляю обэект в базе
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

                        // Сохраняю
                        try
                        {
                            _db.SaveChanges();
                        }
                        catch (Exception )
                        {
                            // TODO сделат логирование
                           // Console.WriteLine($"Не удалось сохранить новый объект публикации {ex.Message}");
                        }
                    }
                }
            }
        }

        private void SaveAd(AdPublishingInfo adInfo)
        {
            // Сохраняем новое объявление в базу
            try
            {
                _db.AdPublishingInfo.Add(adInfo);
                _db.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        // Метод инициализации базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                
                // Загружаю таблицу
                _db.AdPublishingInfo.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                // TODO сделать запись логов
                //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }
    }

}
