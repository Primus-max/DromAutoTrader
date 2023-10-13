using DromAutoTrader.Data;
using DromAutoTrader.Models;
using DromAutoTrader.Prices;
using Microsoft.Win32;
using System.IO;

namespace DromAutoTrader.ViewModels
{
    // TODO изменить тип данных в TablePriceOfIncrease на decimal
    // TODO сделать появления 2 пункта в загрузке прайсов если в списке есть прайсы
    // TODO решить своевременную загрузку брендов, создать отдельную кнопку для получения, потому что при пустой базе, чтобы получить 
    // прайсы, соответственно бренды из них, надо выбрать каналы. Это кольцевая зависимость.
    class MainWindowViewModel : BaseViewModel
    {
        #region ПРИВАТНЫЕ ПОЛЯ
        #region Базовые
        private string? _title = string.Empty;
        private AppContext _db = null!;
        private List<string>? _pathsFilePrices = null!;
        private List<string>? _prices = null!;
        private Price _selectedPrice = null!;
        private List<PriceChannelMapping> _priceChannelMappings = null!;
        #endregion

        #region Поставщики
        private ObservableCollection<Supplier> _suppliers = null!;
        private Supplier _selectedSupplier = null!;
        private string? _newSuplierName = "Имя поставщика";
        private DataGrid _supplierDataGrid = null!;
        #endregion

        #region Бренды
        private ObservableCollection<Brand> _brands = null!;
        private int _totalBrandCount = 0;
        private ObservableCollection<ImageService>? _imageServices = null!;       
        #endregion

        #region Каналы
        private Channel _selectedChannel = null!;
        private ObservableCollection<Channel> _channels = null!;
        private List<Channel> _selectedChannels = null!;
        private ObservableCollection<Channel> _selectedChannelsToView = null!;
        #endregion

        #endregion

        #region ПУБЛИЧНЫЕ ПОЛЯ
        #region Базовые
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        public AppContext Db
        {
            get => _db;
            set => Set(ref _db, value);
        }
        public List<string> PathsFilePrices
        {
            get => _pathsFilePrices;
            set => Set(ref _pathsFilePrices, value);
        }
        public List<string>? Prices
        {
            get => _prices;
            set => Set(ref _prices, value);
        }
        public Price SelectedPrice
        {
            get => _selectedPrice;
            set => Set(ref _selectedPrice, value);
        }
        public List<PriceChannelMapping> PriceChannelMappings
        {
            get => _priceChannelMappings;
            set => Set(ref _priceChannelMappings, value);
        }
        #endregion

        #region Поставщики
        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
        }
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set => Set(ref _selectedSupplier, value);
        }
        public string? NewSuplierName
        {
            get => _newSuplierName;
            set => Set(ref _newSuplierName, value);
        }
        public DataGrid SupplierDataGrid
        {
            get => _supplierDataGrid;
            set => Set(ref _supplierDataGrid, value);
        }
        #endregion

        #region Бренды
        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set => Set(ref _brands, value);
        }

        public int TotalBrandCount
        {
            get => _totalBrandCount;
            set => Set(ref _totalBrandCount, value);
        }

        public ObservableCollection<ImageService> ImageServices
        {
            get => _imageServices;
            set => Set(ref _imageServices, value);
        }
        #endregion

        #region Каналы
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set => Set(ref _selectedChannel, value);
        }
        public ObservableCollection<Channel> Channels
        {
            get => _channels;
            set => Set(ref _channels, value);
        }
        public List<Channel> SelectedChannels
        {
            get => _selectedChannels;
            set => Set(ref _selectedChannels, value);
        }
        public ObservableCollection<Channel> SelectedChannelsToView
        {
            get => _selectedChannelsToView;
            set => Set(ref _selectedChannelsToView, value);
        }
        #endregion

        #endregion

        #region КОМАНДЫ

        #region Поставщики
        public ICommand AddSupplierCommand { get; } = null!;

        private bool CanAddSupplierCommandExecute(object p) => true;

        private void OnAddSupplierCommandExecuted(object sender)
        {

            
        }

        public ICommand EnterKeyPressedCommand { get; } = null!;

        private bool CanEnterKeyPressedCommandExecute(object p) => true;

        private void OnEnterKeyPressedCommandExecuted(object sender)
        {
            // Записываю в базу
            
        }

        public ICommand EditeSuplierCommand { get; } = null!;

        private bool CanEditeSuplierCommandExecute(object p) => true;

        private void OnEditeSuplierCommandExecuted(object sender)
        {
            if (sender is Supplier selectedSupplier)
            {
               
            }
        }

        public ICommand DeleteSuplierCommand { get; } = null!;

        private bool CanDeleteSuplierCommandExecute(object p) => true;

        private void OnDeleteSuplierCommandExecuted(object sender)
        {
            if (sender is Supplier selectedSupplier)
            {
                //DeleteSupplierFromDatabase(selectedSupplier.Id);
            }
        }

        public ICommand SelectFilePriceCommand { get; } = null!;

        private bool CanSelectFilePriceCommandExecute(object p) => true;

        private void OnSelectFilePriceCommandExecuted(object sender)
        {
            GetSelectedFilePaths();
        }

        public ICommand RunAllWorkCommand { get; } = null!;

        private bool CanRunAllWorkCommandExecute(object p) => true;

        private async void OnRunAllWorkCommandExecuted(object sender)
        {
            await RunAllWork();
        }

        #endregion

        #region Каналы
        public ICommand SelectChannelCommand { get; } = null!;

        private bool CanSelectChannelCommandExecute(object p) => true;

        private void OnSelectChannelCommandExecuted(object sender)
        {
            Channel channel = SelectedChannel;
        }
        #endregion        

        #endregion

        public MainWindowViewModel()
        {
            // Инициализация базы данных
            InitializeDatabase();

            #region Инициализация команд

            #region Поставщики
            AddSupplierCommand = new LambdaCommand(OnAddSupplierCommandExecuted, CanAddSupplierCommandExecute);
            EnterKeyPressedCommand = new LambdaCommand(OnEnterKeyPressedCommandExecuted, CanEnterKeyPressedCommandExecute);
            EditeSuplierCommand = new LambdaCommand(OnEditeSuplierCommandExecuted, CanEditeSuplierCommandExecute);
            DeleteSuplierCommand = new LambdaCommand(OnDeleteSuplierCommandExecuted, CanDeleteSuplierCommandExecute);
            SelectFilePriceCommand = new LambdaCommand(OnSelectFilePriceCommandExecuted, CanSelectFilePriceCommandExecute);
            RunAllWorkCommand = new LambdaCommand(OnRunAllWorkCommandExecuted, CanRunAllWorkCommandExecute);
            #endregion

            #region Каналы
            SelectChannelCommand = new LambdaCommand(OnSelectChannelCommandExecuted, CanSelectChannelCommandExecute);
            #endregion
            #endregion

            #region Инициализация источников данных

            #region Поставщики
            //Suppliers = GetAllSuppliers();
            #endregion

            #region Каналы
            Channels = new ObservableCollection<Channel>(_db.Channels.ToList());
            #endregion

            #region Бренды
            Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
            TotalBrandCount = Brands.Count;
            ImageServices = new ObservableCollection<ImageService>(_db.ImageServices.ToList());
            #endregion

            #region Прайсы
            PriceChannelMappings = new List<PriceChannelMapping>();
            #endregion

            #endregion            
        }


        #region МЕТОДЫ

        // Метод запускающий всю работу
        public async Task RunAllWork()
        {
            //PriceProcessor priceProcessor = new();
            BrandImporter brandImporter = new();

            foreach (var path in PathsFilePrices)
            {
                if (string.IsNullOrEmpty(path))
                    MessageBox.Show("Для начала работы необходимо выбрать прайс");

                PriceList prices = await ProcessPriceAsync(path);

                if (prices == null) return;

                // Добавляю брэнды в базу
                brandImporter.ImportBrandsFromPrices(prices);
                Brands.Clear();
                Brands = new ObservableCollection<Brand>(_db.Brands.ToList()); // Обновляю свойство

                // Модель для публикации объявления
                AdPublishingInfo adPublishingInfo = new AdPublishingInfo();

                // Получаем к этому прайсу выбранные каналы
                string fileName = Path.GetFileNameWithoutExtension(path);
                PriceChannelMapping? priceChannels = PriceChannelMappings?.FirstOrDefault(mapping => mapping?.Price.Name == fileName);

                if (priceChannels == null)
                {
                    MessageBox.Show("Что-то пошло не так, попробуйте выбрать прайс и каналы для него", "Внимание",
                       MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }



                foreach (var price in prices)
                {
                    #region Проверка на наличие брэнда в выбранном канале
                    bool hasMatchingBrand = false;

                    foreach (var channel in priceChannels?.SelectedChannels)
                    {
                        // Проверяю, есть ли бренд из price в текущем канале
                        if (channel.Brands.Any(brand => brand.Name == price?.Brand))
                        {
                            hasMatchingBrand = true;
                            break; // Если нашли совпадение, выходим из цикла
                        }
                    }

                    if (!hasMatchingBrand)
                        continue; // Если не соответствует ни одному каналу, пропускаем итерацию 
                    #endregion

                    // Цикл по каждому канал в отдельном потоке
                    foreach (var priceChannelMapping in priceChannels.SelectedChannels)
                    {
                        // Для каждого канала создаем отдельную задачу
                        await Task.Run(async () =>
                          {
                              // Создаем ChannelAdInfoBuilder для данного канала и цены
                              var builder = new ChannelAdInfoBuilder(price, priceChannelMapping);

                              // Строим AdPublishingInfo для данного канала
                              var adInfo = await builder.Build();

                              // Вы можете здесь использовать adInfo для дальнейших операций
                          });
                    }
                }
            }
        }



        #region Асинхронные методы

        #region Прайс
        // Метод асинхронного получения прайса
        public async Task<PriceList> ProcessPriceAsync(string pathToFile)
        {
            PriceProcessor priceProcessor = new();

            var processTask = Task.Run(() =>
            {
                try
                {
                    return priceProcessor.ProcessExcelPrice(pathToFile);
                }
                catch (Exception ex)
                {
                    // Вывести сообщение об ошибке с указанием причины
                    MessageBox.Show($"Ошибка при обработке прайса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    // Перебросить исключение для обработки в вызывающем коде, если это необходимо
                    throw;
                }
            });

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(15)); // Время ожидания задачи

            if (await Task.WhenAny(processTask, timeoutTask) == processTask)
            {
                return await processTask;
            }
            else
            {
                // В этом случае прошло слишком много времени
                MessageBox.Show("Обработка прайса заняла слишком много времени. Пожалуйста, попробуйте еще раз.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null; // или выберите другое значение по умолчанию
            }
        }
        #endregion

        #endregion

        #region Базовые

        // Метод получения прайсов
        private List<string> GetSelectedFilePaths()
        {
            List<string> selectedFilePaths = new List<string>();

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true, // Разрешить выбор нескольких файлов
                Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePaths.AddRange(openFileDialog.FileNames);
            }
            PathsFilePrices = selectedFilePaths;

            // Получаю имена прайсов для отображения в списке
            Prices = PathsFilePrices.Select(path => System.IO.Path.GetFileNameWithoutExtension(path)).ToList();


            return PathsFilePrices;
        }

        // Метод выбора файла для парсинга
        //private void SelectFilePrice()
        //{
        //    // Создаем диалоговое окно выбора файла
        //    OpenFileDialog openFileDialog = new()
        //    {
        //        // Устанавливаем фильтры для типов файлов, которые вы хотите разрешить выбирать
        //        Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        // Получаем путь к выбранному файлу
        //        PathsFilePrices = openFileDialog.FileName;
        //    }
        //}

        // Метод инициализации базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД и включаем связанные данные (PriceIncreases и Brands)
                _db.Channels.Include(c => c.PriceIncreases).Include(c => c.Brands).Load();
                _db.ImageServices.Load();
            }
            catch (Exception)
            {
                // TODO сделать запись логов
                //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }
        #endregion

        #region Каналы
        public void OnSelectChannel(Price price, List<Channel> selectedChannels)
        {
            SelectedPrice = price;
            SelectedChannels = selectedChannels;

            // Найдите соответствующий объект PriceChannelMapping по имени прайса или создайте новый, если его нет
            var mapping = PriceChannelMappings.FirstOrDefault(m => m.Price.Name == price.Name);
            if (mapping == null)
            {
                mapping = new PriceChannelMapping(price);
                PriceChannelMappings.Add(mapping);
            }

            // Обновите список выбранных каналов для данного прайса
            mapping.SelectedChannels = selectedChannels;

            if (!PriceChannelMappings.Contains(mapping))
                PriceChannelMappings.Add(mapping);
        }
        #endregion       

        #endregion
    }
}
