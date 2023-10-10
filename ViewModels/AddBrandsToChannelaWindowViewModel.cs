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

            Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
        }


        #region Методы
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
