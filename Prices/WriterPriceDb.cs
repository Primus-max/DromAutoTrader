using DromAutoTrader.Data;

namespace DromAutoTrader.Prices
{
    class WriterPriceDb
    {

        private AppContext _db = null!;
        public WriterPriceDb()
        {
            // Инициализация базы данных
            InitializeDatabase();
        }

        private void Write(FormattedPrice formattedPrice)
        {
            if (formattedPrice != null) return;
            try
            {
                _db.FormattedPrices.Add(formattedPrice);
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
                // загружаем данные о поставщиках из БД
                _db.FormattedPrices.Load();
            }
            catch (Exception)
            {
                // TODO сделать запись логов
                //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }

    }
}
