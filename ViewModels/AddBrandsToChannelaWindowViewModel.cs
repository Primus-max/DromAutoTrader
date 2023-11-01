using DromAutoTrader.Data;
using DromAutoTrader.Views;
using DromAutoTrader.Views.Windows;
using MaterialDesignThemes.MahApps;
using System.Drawing;

namespace DromAutoTrader.ViewModels
{
    internal class AddBrandsToChannelaWindowViewModel : BaseViewModel
    {
        #region Приватные свойства
        private AppContext _db = null!;
        private ObservableCollection<Brand> _brands = null!;
        private int _channelId;
        private ListBox _brandsListBox = null!;
        private int _addedBrandsCount = 0;
        #endregion

        #region Публичные свойства
        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set => Set(ref _brands, value); 
        }  
        public int ChannelId
        {
            get => _channelId;
            set => Set(ref _channelId, value);
        }
        public ListBox BrandsListBox
        {
            get => _brandsListBox;
            set => Set(ref _brandsListBox, value);
        }
        public int AddedBrandsCount
        {
            get => _addedBrandsCount;
            set => Set(ref _addedBrandsCount, value);
        }
        #endregion

        #region Команды

        #endregion

        public AddBrandsToChannelaWindowViewModel()
        {

            // Получаю базу данных
            InitializeDatabase();

            #region Инициализация источников данных
            ChannelId = LocatorService.Current.SelectedChannel.Id;
            Brands = new ObservableCollection<Brand>(_db.Brands.OrderBy(brand => brand.ChannelId != ChannelId).ThenBy(brand => brand.Name).ToList());            
            #endregion
        }


        #region Методы
        // Метод записи и соотношения брэндов к каналу
        public void UpdateBrandChannelMappings(int selectedChannelId, List<Brand> selectedBrands, Window curWindow)
        {
            // Получаем текущие связи из таблицы BrandChannelMapping
            var existingMappings = _db.BrandChannelMappings.ToList();

            // Создаем новые связи для выбранных брендов
            var newMappings = selectedBrands.Select(brand => new BrandChannelMapping
            {
                BrandId = brand.Id,
                ChannelId = selectedChannelId
            }).ToList();

            // Удаляем устаревшие связи, которых больше нет в выбранных брендах
            var mappingsToRemove = existingMappings
                .Where(existingMapping => !newMappings.Any(newMapping => newMapping.BrandId == existingMapping.BrandId))
                .ToList();
            _db.BrandChannelMappings.RemoveRange(mappingsToRemove);

            // Добавляем новые связи
            _db.BrandChannelMappings.AddRange(newMappings);

            try
            {
                _db.SaveChanges();
                curWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

                // Загружаем данные о BrandChannelMappings с зависимостями
                _db.BrandChannelMappings
                    .Include(mapping => mapping.Brand)
                    .Include(mapping => mapping.Channel)
                    .Load();
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
