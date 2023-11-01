using DromAutoTrader.Data;
using DromAutoTrader.Services;
using DromAutoTrader.Views;
using DromAutoTrader.Views.Windows;

namespace DromAutoTrader.ViewModels
{
    internal class EditeChannelPageViewModel : BaseViewModel
    {
        #region Приватные поля
        private TablePriceOfIncrease _priceIncrease = null!;
        private Channel _selectedChannel = null!;
        private AppContext _db = null!;
        private ObservableCollection<TablePriceOfIncrease> _tablePriceOfIncreases = null!;
        private ObservableCollection<TablePriceOfIncrease> _filteredTablePriceOfIncreases = null!;
        private int _totalBrandCount = 0;
        private string? _descriptionChannel = string.Empty;
        #endregion

        #region Публичные поля
        public TablePriceOfIncrease PriceIncrease
        {
            get => _priceIncrease;
            set => Set(ref _priceIncrease, value);
        }
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set => Set(ref _selectedChannel, value);
        }
        public ObservableCollection<TablePriceOfIncrease> TablePriceOfIncreases
        {
            get => _tablePriceOfIncreases;
            set => Set(ref _tablePriceOfIncreases, value);
        }
        public ObservableCollection<TablePriceOfIncrease> FilteredTablePriceOfIncreases
        {
            get => _filteredTablePriceOfIncreases;
            set => Set(ref _filteredTablePriceOfIncreases, value);
        }
        public int TotalBrandCount
        {
            get => _totalBrandCount;
            set => Set(ref _totalBrandCount, value);
        }
        public string? DescriptionChannel
        {
            get => _descriptionChannel;
            set => Set(ref _descriptionChannel, value);
        }
        #endregion

        #region Команды
        public ICommand AddRowTablePriceOfIncreasesCommand { get; } = null!;

        private bool CanAddRowTablePriceOfIncreasesCommandExecute(object p) => true;

        private void OnAddRowTablePriceOfIncreasesCommandExecuted(object sender)
        {
            if (sender is DataGrid TableOfIncreaseDataGrid)
                AddRowTablePriceOfIncreases(TableOfIncreaseDataGrid);
        }

        public ICommand SaveTablePriceOfIncreasesCommand { get; } = null!;

        private bool CanSaveTablePriceOfIncreasesCommandExecute(object p) => true;

        private void OnSaveTablePriceOfIncreasesCommandExecuted(object sender)
        {
            SaveTablePriceOfIncreases();
            SaveDescriptionChannel();
        }
        public ICommand RemoveTablePriceOfIncreasesCommand { get; } = null!;

        private bool CanRemoveTablePriceOfIncreasesCommandExecute(object p) => true;

        private void OnRemoveTablePriceOfIncreasesCommandExecuted(object sender)
        {
            TablePriceOfIncrease tablePriceOfIncrease = sender as TablePriceOfIncrease;
            if (tablePriceOfIncrease == null)
            {
                return; // Если объект не является TablePriceOfIncrease, просто выйдем из метода
            }
            RemoveTablePriceOfIncreases(tablePriceOfIncrease);
        }

        public ICommand GoBackAllChannelsCommand { get; } = null!;

        private bool CanGoBackAllChannelsCommandExecute(object p) => true;

        private void OnGoBackAllChannelsCommandExecuted(object sender)
        {
            LocatorService.Current.ChannelFrame.GoBack();
        }

        #region Брэнды
        public ICommand OpenAddBrandToChannelWindowCommand { get; } = null!;

        private bool CanOpenAddBrandToChannelWindowExecute(object p) => true;

        private void OnOpenAddBrandToChannelWindowExecuted(object sender)
        {
            OpenAddBrandToChannelWindow();
        }
        #endregion

        #endregion

        public EditeChannelPageViewModel()
        {
            InitializeDatabase();

            // Получаю выбранный Channel в AllChannelPage через LocatorService
            SelectedChannel = LocatorService.Current.SelectedChannel;

            #region Инициализация источников данных
            TablePriceOfIncreases = new ObservableCollection<TablePriceOfIncrease>(_db.TablePriceOfIncreases.ToList());
            TotalBrandCount = CalcCountBrandsForChannel();
            DescriptionChannel = SelectedChannel.Description;
            #endregion

            #region Инициализация команд
            AddRowTablePriceOfIncreasesCommand = new LambdaCommand(OnAddRowTablePriceOfIncreasesCommandExecuted, CanAddRowTablePriceOfIncreasesCommandExecute);
            SaveTablePriceOfIncreasesCommand = new LambdaCommand(OnSaveTablePriceOfIncreasesCommandExecuted, CanSaveTablePriceOfIncreasesCommandExecute);
            RemoveTablePriceOfIncreasesCommand = new LambdaCommand(OnRemoveTablePriceOfIncreasesCommandExecuted, CanRemoveTablePriceOfIncreasesCommandExecute);
            GoBackAllChannelsCommand = new LambdaCommand(OnGoBackAllChannelsCommandExecuted, CanGoBackAllChannelsCommandExecute);
            OpenAddBrandToChannelWindowCommand = new LambdaCommand(OnOpenAddBrandToChannelWindowExecuted, CanOpenAddBrandToChannelWindowExecute);
            #endregion

            #region Вызов методов
            UpdateFilteredTablePriceOfIncreases();
            #endregion

            #region Подписка на события
            EventAggregator.AddedBrandsCountChanged += UpdateAddedBrandsCount;
            #endregion
        }

        #region Методы
        // Обновляю колличество выбранных каналов
        private void UpdateAddedBrandsCount()
        {
            CalcCountBrandsForChannel();
        }

        private int CalcCountBrandsForChannel()
        {
            int count = 0;
            count = _db.BrandChannelMappings.Count(m => m.ChannelId == SelectedChannel.Id);

            TotalBrandCount = count;    
            return count;
        }
        private void AddRowTablePriceOfIncreases(DataGrid dataGrid)
        {
            //SelectedChannel;
            FilteredTablePriceOfIncreases.Add(new TablePriceOfIncrease());
            dataGrid.ItemsSource = FilteredTablePriceOfIncreases;
        }

        // Сохраняем таблицу накрутки цен
        public void SaveTablePriceOfIncreases()
        {
            try
            {
                foreach (var price in FilteredTablePriceOfIncreases)
                {
                    // Проверяем, существует ли запись с таким Id
                    var existingRecord = _db.TablePriceOfIncreases.FirstOrDefault(x => x.Id == price.Id);

                    if (existingRecord != null)
                    {

                        // Проверяем, изменились ли какие-либо поля
                        if (existingRecord.From != price.From || existingRecord.To != price.To || existingRecord.PriceIncrease != price.PriceIncrease)
                        {
                            // Если есть изменения, обновляем запись
                            existingRecord.From = price.From;
                            existingRecord.To = price.To;
                            existingRecord.PriceIncrease = price.PriceIncrease;
                        }
                        // Иначе ничего не делаем, запись уже существует и не изменилась
                    }
                    else
                    {
                        // Запись с таким Id не существует, добавляем ее с новым Id
                        // Устанавливаем ChannelId только для новых записей                       
                        price.ChannelId = SelectedChannel.Id;
                        _db.TablePriceOfIncreases.Add(price);
                    }
                }

                _db.SaveChanges();

                //UpdateFilteredTablePriceOfIncreases();
            }
            catch (Exception)
            {
                // Обработка ошибок сохранения данных
            }
        }

        // Метод сохранения описания к каналу
        private void SaveDescriptionChannel()
        {
            var curChannel = _db.Channels.FirstOrDefault(x => x.Id == SelectedChannel.Id);

            if (curChannel == null) return;
            curChannel.Description = DescriptionChannel;

            try
            {
                _db.Channels.Update(curChannel);
                _db.SaveChanges();
            }
            catch (Exception)
            {

            }

        }

        // Метод открытия окна для выбора брэндов для каналов
        private void OpenAddBrandToChannelWindow()
        {
            int selectedChannelId = SelectedChannel.Id;

            AddBrandsToChannelaWindow addBrands = new(selectedChannelId);
            addBrands.Show();
        }

        // Метод фильтрации по каналам
        private void UpdateFilteredTablePriceOfIncreases()
        {
            if (SelectedChannel != null)
            {
                // Фильтруем записи по выбранному каналу
                FilteredTablePriceOfIncreases = new ObservableCollection<TablePriceOfIncrease>(
                    TablePriceOfIncreases.Where(price => price.ChannelId == SelectedChannel.Id)
                );
            }
            else
            {
                // Если канал не выбран, показываем все записи
                FilteredTablePriceOfIncreases = new ObservableCollection<TablePriceOfIncrease>(TablePriceOfIncreases);
            }
        }

        // Метод удаления записей из таблицы накрутки цен
        private void RemoveTablePriceOfIncreases(TablePriceOfIncrease selectedPrice)
        {
            try
            {
                // Удалите запись из коллекции
                FilteredTablePriceOfIncreases.Remove(selectedPrice);

                // Удалите запись из базы данных
                _db.TablePriceOfIncreases.Remove(selectedPrice);
                _db.SaveChanges();

                //UpdateFilteredTablePriceOfIncreases();
            }
            catch (Exception)
            {
                // Обработка ошибок удаления данных
            }
        }

        // Инициализирую базу данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД
                _db.TablePriceOfIncreases.Load();

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
