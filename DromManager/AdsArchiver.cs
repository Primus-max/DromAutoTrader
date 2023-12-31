﻿using DromAutoTrader.Data;

namespace DromAutoTrader.DromManager
{
    /// <summary>
    /// Класс для проверки актуальности объявлений
    /// </summary>
    public class AdsArchiver
    {
        public AdsArchiver()
        {
            
        }

        /// <summary>
        /// Метод проверяет объявления по прайсу, если в прайсе нет такого объявления, убирает в архив       
        /// </summary>
        public void CompareAndArchiveAds()
        {
            var todayAds = GetAdsForToday();
            var outdatedAds = GetOutdatedAds();

            ArchiveOutdatedAds(todayAds, outdatedAds);
        }

        private List<AdPublishingInfo> GetAdsForToday()
        {
            var currentDate = DateTime.Now.Date;
            using var context = new AppContext();
            return context.AdPublishingInfo
                .ToList()
                .Where(a => DateTime.Parse(a.DatePublished).Date == currentDate)
                .ToList();
        }

        private List<AdPublishingInfo> GetOutdatedAds()
        {
            var currentDate = DateTime.Now.Date;
            using var context = new AppContext();
            return context.AdPublishingInfo
                .ToList()
                .Where(a => DateTime.Parse(a.DatePublished).Date != currentDate)
                .ToList();
        }

        // Метод проверки объявлений для перемещения в архив
        private void ArchiveOutdatedAds(List<AdPublishingInfo> currentAds, List<AdPublishingInfo> outdatedAds)
        {
            foreach (var outdatedAd in outdatedAds)
            {
                bool adExistsInCurrent = currentAds.Any(a =>
                    a.Brand == outdatedAd.Brand &&
                    a.Artikul == outdatedAd.Artikul &&
                    a.KatalogName == outdatedAd.KatalogName);

                if (adExistsInCurrent)
                {
                    // Объявление отсутствует в текущих объявлениях, устанавливаем флаг IsArchived
                    outdatedAd.IsArchived = true;
                    // Сохраняем изменения в базе данных
                    try
                    {
                        using var context = new AppContext();
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        string messageError = $"Ошибка при записи IsArchived в объекте публикации, в методе ArchiveOutdatedAds {ex.Message}";
                    }
                }
            }
        }
    }
}
