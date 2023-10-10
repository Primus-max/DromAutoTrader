using DromAutoTrader.Data;

namespace DromAutoTrader.ViewModels
{
    internal class AddBrandsToChannelaWindowViewModel : BaseViewModel
    {
        #region Приватные свойства
        private AppContext _db = null!;
        private ObservableCollection<Brand> _brands = null!;
        #endregion

        #region Публичные свойства
        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set => Set(ref _brands, value); 
        }
        #endregion

        #region Команды

        #endregion

        public AddBrandsToChannelaWindowViewModel()
        {

            // Получаю базу данных
            InitializeDatabase();

            #region Инициализация источников данных
            Brands = new ObservableCollection<Brand>(_db.Brands.ToList()); 
            #endregion
        }


        #region Методы
        // Метод записи и соотношения брэндов к каналу
        public void AddBrandsToChannelInDb(int selectedChannelId, List<Brand> brands, Window curWindow)
        {

            foreach (var brand in brands)
            {
                Brand? existedBrand = _db.Brands.FirstOrDefault(b => b.Id == brand.Id);

                if (existedBrand != null)
                {
                    existedBrand.Id = brand.Id;
                    existedBrand.ChannelId = selectedChannelId;
                }
                else
                {
                    // Создайте новый объект Brand, если он не существует
                    brand.ChannelId = selectedChannelId;
                    _db.Brands.Add(brand);
                }

                try
                {
                    _db.SaveChanges();
                    curWindow.Close();
                       
                }
                catch (Exception)
                {
                    // Обработка ошибок сохранения, если необходимо
                }
            }
        }



        // Метод получения базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД
                _db.Brands.Load();
            }
            catch (Exception)
            {
                // TODO сделать запись логов
                //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }
        #endregion



    }
}
