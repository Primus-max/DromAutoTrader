using DromAutoTrader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromAutoTrader.DromManager
{
    public class AdsComparisonExistsFilter
    {
        private AppContext _db = null!;

        public AdsComparisonExistsFilter()
        {
            InitializeDatabase();
        }

        public void CompareAndArchiveAds()
        {
            var todayAds = GetAdsForToday();
            var outdatedAds = GetOutdatedAds();

            ArchiveOutdatedAds(todayAds, outdatedAds);
        }

        private List<AdPublishingInfo> GetAdsForToday()
        {
            var currentDate = DateTime.Now.Date;

            return _db.AdPublishingInfo
                .Where(a => DateTime.Parse(a.DatePublished).Date == currentDate)
                .ToList();
        }

        private List<AdPublishingInfo> GetOutdatedAds()
        {
            var currentDate = DateTime.Now.Date;

            return _db.AdPublishingInfo
                .Where(a => DateTime.Parse(a.DatePublished).Date != currentDate)
                .ToList();
        }

        private void ArchiveOutdatedAds(List<AdPublishingInfo> currentAds, List<AdPublishingInfo> outdatedAds)
        {
            foreach (var outdatedAd in outdatedAds)
            {
                bool adExistsInCurrent = currentAds.Any(a =>
                    a.Brand == outdatedAd.Brand &&
                    a.Artikul == outdatedAd.Artikul &&
                    a.KatalogName == outdatedAd.KatalogName);

                if (!adExistsInCurrent)
                {
                    // Объявление отсутствует в текущих объявлениях, устанавливаем флаг IsArchived
                    outdatedAd.IsArchived = true;
                    // Сохраняем изменения в базе данных
                    _db.SaveChanges();
                }
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
