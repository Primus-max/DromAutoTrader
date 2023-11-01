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
            // Ваша логика по фильтрации и сохранению данных в базе
            // (без публикации)

            // Проверка на существующие объявления
            var existingAds = _db.AdPublishingInfo
                .Where(a =>
                    a.Brand == adInfo.Brand &&
                    a.Artikul == adInfo.Artikul &&
                    a.DatePublished == DateTime.Now.Date.ToString()
                )
                .ToList();

            if (existingAds.Count == 0)
            {
                // Если нет существующих объявлений, сохраняем новое объявление
                SaveAd(adInfo);
            }
            else
            {
                // Если есть существующие объявления, выполним сравнение и обновление данных
                foreach (var existingAd in existingAds)
                {
                    if (adInfo.InputPrice >= existingAd.InputPrice)
                    {
                        // Выполняем обновление существующего объявления или другую необходимую логику
                        existingAd.OutputPrice = adInfo.OutputPrice;
                        // Другие обновления
                        _db.SaveChanges();
                    }
                }
            }
        }

        private void SaveAd(AdPublishingInfo adInfo)
        {
            // Сохраняем новое объявление в базу
            _db.AdPublishingInfo.Add(adInfo);
            _db.SaveChanges();
        }

        // Метод инициализации базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД и включаем связанные данные (PriceIncreases и Brands)
                _db.Channels
                    .Load();
                _db.Brands
                    .Include(b => b.ImageServices)
                    .Load();
                _db.BrandImageServiceMappings
                    // Загрузка связанных ImageService
                    .Load();
                _db.BrandImageServiceMappings.Load();
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
