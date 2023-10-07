using DromAutoTrader.Data;
using DromAutoTrader.Views;
using OpenQA.Selenium.DevTools.V115.Page;

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
        #endregion

        #region Команды
        public ICommand AddRowTablePriceOfIncreasesCommand { get; } = null!;

        private bool CanAddRowTablePriceOfIncreasesCommandExecute(object p) => true;

        private void OnAddRowTablePriceOfIncreasesCommandExecuted(object sender)
        {
            AddRowTablePriceOfIncreases();
        }

        public ICommand SaveTablePriceOfIncreasesCommand { get; } = null!;

        private bool CanSaveTablePriceOfIncreasesCommandExecute(object p) => true;

        private void OnSaveTablePriceOfIncreasesCommandExecuted(object sender)
        {
            SaveTablePriceOfIncreases();
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

        #endregion

        public EditeChannelPageViewModel()
        {
            InitializeDatabase();

            // Получаю выбранный Channel в AllChannelPage через LocatorService
            SelectedChannel = LocatorService.Current.SelectedChannel;

            #region Инициализация источников данных
            TablePriceOfIncreases = new ObservableCollection<TablePriceOfIncrease>(_db.TablePriceOfIncreases.ToList());
            #endregion

            #region Инициализация команд
            AddRowTablePriceOfIncreasesCommand = new LambdaCommand(OnAddRowTablePriceOfIncreasesCommandExecuted, CanAddRowTablePriceOfIncreasesCommandExecute);
            SaveTablePriceOfIncreasesCommand = new LambdaCommand(OnSaveTablePriceOfIncreasesCommandExecuted, CanSaveTablePriceOfIncreasesCommandExecute);
            RemoveTablePriceOfIncreasesCommand = new LambdaCommand(OnRemoveTablePriceOfIncreasesCommandExecuted, CanRemoveTablePriceOfIncreasesCommandExecute);
            GoBackAllChannelsCommand = new LambdaCommand(OnGoBackAllChannelsCommandExecuted, CanGoBackAllChannelsCommandExecute);
            #endregion

            #region Вызов методов
            UpdateFilteredTablePriceOfIncreases();
            #endregion
        }

        #region Методы
        private void AddRowTablePriceOfIncreases()
        {
            TablePriceOfIncreases.Add(new TablePriceOfIncrease());
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
