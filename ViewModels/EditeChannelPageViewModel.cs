using DromAutoTrader.Data;
using DromAutoTrader.Views;


namespace DromAutoTrader.ViewModels
{
    internal class EditeChannelPageViewModel : BaseViewModel
    {
        #region Приватные поля
        private TablePriceOfIncrease _priceIncrease = null!;
        private Channel _selectedChannel = null!;
        private AppContext _db = null!;
        private ObservableCollection<TablePriceOfIncrease> _tablePriceOfIncreases = null!;
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
                foreach (var price in TablePriceOfIncreases)
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
            }
            catch (Exception)
            {
                // Обработка ошибок сохранения данных
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
