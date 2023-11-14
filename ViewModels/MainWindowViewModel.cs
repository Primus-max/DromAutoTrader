using DromAutoTrader.AdsPowerManager;
using DromAutoTrader.DromManager;
using DromAutoTrader.Infrastacture.Commands;
using DromAutoTrader.Prices;
using Microsoft.Win32;
using Serilog.Core;
using System.IO;
using System.Text;

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
        private ObservableCollection<AdPublishingInfo> _adPublishingInfos = null!;
        private ObservableCollection<PostingProgressItem> _postingProgressItems = null!;
        private bool _isModeRunAllWork = true;      
        private readonly Logger _logger = null!;
        #endregion

        #region Поставщики
        private ObservableCollection<Supplier> _suppliers = null!;
        private Supplier _selectedSupplier = null!;
        private string? _newSuplierName = "Имя поставщика";
        private DataGrid _supplierDataGrid = null!;
        #endregion

        #region Бренды
        private ObservableCollection<Brand> _brands = null!;
        private Brand _selectedBrand = null!;
        private int _totalBrandCount = 0;
        private ObservableCollection<ImageService>? _imageServices = null!;
        private ObservableCollection<ImageService> _selectedImageServices = null!;
        private List<BrandImageServiceMapping> _brandImageServiceMappings = null!;
        private ObservableCollection<BrandWithSelectedImageServices> _brandWithSelectedImageServices = null!;
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

        public ObservableCollection<AdPublishingInfo> AdPublishingInfos
        {
            get => _adPublishingInfos;
            set => Set(ref _adPublishingInfos, value);
        }

        public ObservableCollection<PostingProgressItem> PostingProgressItems
        {
            get => _postingProgressItems;
            set => Set(ref _postingProgressItems, value);
        }

        IProgress<PostingProgressItem> _progressReporter = null!;
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
        public Brand SelectedBrand
        {
            get => _selectedBrand;
            set => Set(ref _selectedBrand, value);
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
        public ObservableCollection<ImageService> SelectedImageServices
        {
            get => _selectedImageServices;
            set => Set(ref _selectedImageServices, value);
        }

        public ObservableCollection<BrandWithSelectedImageServices> BrandWithSelectedImageServices
        {
            get => _brandWithSelectedImageServices;
            set => Set(ref _brandWithSelectedImageServices, value);
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
            await Task.Run(async () =>
            {
                await RunAllWork(); // Выполнение RunAllWork в фоновом потоке
            });
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

        #region Брэнды
        // Команда для переключения режима сбора брендов из прайсов
        public ICommand ToggleModeGettingBrandsCommand { get; } = null!;
        private bool CanToggleModeGettingBrandsCommandExecute(object p) => true;
        private async void OnToggleModeGettingBrandsCommandExecuted(object sender)
        {
            _isModeRunAllWork = false;

            GetSelectedFilePaths(); // Выбираю прайсы и записываю пути

            await ParsingPricesAsync(); // Обрабатываю прайсы в режиме только для берндов
        }

        // Команда выбора сервисов для бренда
        public ICommand SelectImageServiceCommand { get; } = null!;
        private bool CanSelectImageServiceCommandExecute(object p) => true;
        private void OnSelectImageServiceCommandExecuted(object sender)
        {
            if (!(sender is CheckBox checkBox) || checkBox.DataContext == null)
                return;

            if (checkBox.DataContext is ImageServiceWithState imageServiceWithState)
            {
                if (checkBox.IsChecked == true)
                {
                    // Добавляем запись в базу данных
                    var mapping = new BrandImageServiceMapping
                    {
                        BrandId = SelectedBrand.Id,
                        ImageServiceId = imageServiceWithState.ImageService.Id
                    };

                    try
                    {
                        _db.BrandImageServiceMappings.Add(mapping);
                        _db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        // Обработка ошибок при сохранении в базу данных
                    }
                }
                else if (checkBox.IsChecked == false)
                {
                    // Удаляем запись из базы данных
                    var mapping = _db.BrandImageServiceMappings.FirstOrDefault(m =>
                        m.BrandId == SelectedBrand.Id && m.ImageServiceId == imageServiceWithState.ImageService.Id);
                    if (mapping != null)
                    {
                        try
                        {
                            // Удаляем запись
                            _db.BrandImageServiceMappings.Remove(mapping);
                            _db.SaveChanges();
                        }
                        catch (Exception)
                        {
                            // Обработка ошибок при удалении из базы данных
                        }
                    }
                }
            }
        }

        // Команда выбора изображения по умолчанию
        public ICommand SelectImageServiceDefaultCommand { get; } = null!;
        private bool CanSelectImageServiceDefaultCommandExecute(object p) => true;
        private void OnSelectImageServiceDefaultCommandExecuted(object sender)
        {
            SaveDefaultImageForBrand(SelectedBrand);
        }
        #endregion

        #region Команда для получения нескольких параметров
        public RelayCommand MyCommand { get; }

        // Метод выполнения команды
        private void ExecuteMethod(object parameter)
        {
            // Ваш код выполнения команды
        }

        // Метод, определяющий, можно ли выполнить команду
        private bool CanExecuteMethod(object parameter)
        {
            // Ваш код определения, можно ли выполнить команду
            return true;
        }
        #endregion

        #endregion

        public MainWindowViewModel()
        {
            // Инициализация базы данных
            InitializeDatabase();

            #region Инициализация команд



            #region Команда для получения нескольких параметров
            MyCommand = new RelayCommand(ExecuteMethod, CanExecuteMethod);
            #endregion

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

            #region Брэнды
            SelectImageServiceCommand = new LambdaCommand(OnSelectImageServiceCommandExecuted, CanSelectImageServiceCommandExecute);
            SelectImageServiceDefaultCommand = new LambdaCommand(OnSelectImageServiceDefaultCommandExecuted, CanSelectImageServiceDefaultCommandExecute);
            ToggleModeGettingBrandsCommand = new LambdaCommand(OnToggleModeGettingBrandsCommandExecuted, CanToggleModeGettingBrandsCommandExecute);
            #endregion
            #endregion

            #region Инициализация источников данных
            PostingProgressItems = new ObservableCollection<PostingProgressItem>();

            #region Поставщики
            //Suppliers = GetAllSuppliers();
            #endregion

            #region Каналы
            Channels = new ObservableCollection<Channel>(_db.Channels.ToList());
            #endregion

            #region Бренды
            //BrandWithSelectedImageServices = GetBrandsWithSelectedImageServices();
            // Brands = new ObservableCollection<Brand>(_db.Brands.ToList());

            Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
            TotalBrandCount = Brands.Count;

            // Дабавляю выбранные сервисы картинов в отображение
            var ImageServices = new ObservableCollection<ImageService>(_db.ImageServices.ToList());

            foreach (var brand in Brands)
            {
                foreach (var imageService in ImageServices)
                {
                    var brandImageServiceMapping = _db.BrandImageServiceMappings.FirstOrDefault(
                        m => m.BrandId == brand.Id && m.ImageServiceId == imageService.Id);

                    var imageServiceWithState = new ImageServiceWithState
                    {
                        ImageService = imageService,
                        IsSelected = brandImageServiceMapping != null
                    };

                    brand.ImageServicesWithState.Add(imageServiceWithState);
                }
            }

            #endregion

            #region Прайсы
            PriceChannelMappings = new List<PriceChannelMapping>();
            #endregion
            #endregion

            // Метод отслеживающий прогресс
            _progressReporter = new Progress<PostingProgressItem>(reportItem =>
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    var existingItem = PostingProgressItems.FirstOrDefault(item => item.PriceName == reportItem.PriceName);
                    if (existingItem != null)
                    {
                        // Обновить существующий объект в коллекции
                        existingItem.ProcessName = reportItem.ProcessName;
                        existingItem.CurrentStage = reportItem.CurrentStage;
                        existingItem.TotalStages = reportItem.TotalStages;
                        existingItem.MaxValue = reportItem.MaxValue;
                        existingItem.DatePublished = reportItem.DatePublished;
                        existingItem.GetFileButton = reportItem.GetFileButton;
                        existingItem.PriceExportPath = reportItem.PriceExportPath;
                    }
                    else
                    {
                        // Если объект не существует, добавьте его в коллекцию
                        PostingProgressItems.Add(reportItem);
                    }
                });
            });

        }

        #region МЕТОДЫ

        // Метод запускающий всю работу
        public async Task RunAllWork()
        {
            // Получаю, обрабатываю, записываю в базу прайсы
            await ParsingPricesAsync();

            Console.WriteLine("Парсинг закончил, приступаю к проверке");
            AdsArchiver adsArchiver = new();
            adsArchiver.CompareAndArchiveAds();

            Console.WriteLine("Проверку закончил, приступаю к размещению");
            await ProcessPublishingAdsAtDrom();

            Console.WriteLine("Публикацию закончил, создаю прайс");
            string pricePath = ExportPrice();
            if (!string.IsNullOrEmpty(pricePath))
            {
                // Здесь передаём путь к файлу для скачивания(локально)
            }

            Console.WriteLine("Прайс создал, убираю в архив");
            await RemoveAtArchive(); // Убираю в архив

            DeleteOutdatedAdsAtDb(); // Убираю старые объявления
        }

        // Метод получения и парсинга прайсов
        private async Task ParsingPricesAsync()
        {
            var tasks = new List<Task>();

            foreach (var path in PathsFilePrices)
            {
                string priceName = Path.GetFileName(path);

               

                var postingProgressItem = new PostingProgressItem
                {
                    ProcessName = $"Начал обработку прайса",
                    MaxValue = PathsFilePrices.Count,
                    PriceName = priceName
                };

                _progressReporter.Report(postingProgressItem);

                Task task = Task.Run(async () =>
                {
                    Console.WriteLine($"Начал парсинг прайса {priceName}");

                    // Парсинг прайсов и обработка данных
                    PriceList prices = await ProcessPriceAsync(path);

                    Console.WriteLine($"Закончил парсинг прайса {priceName}");

                    postingProgressItem.ProcessName = "Получил прайс";
                    postingProgressItem.TotalStages = prices.Count;

                    // Обновление прогресса
                    postingProgressItem.CurrentStage++;
                    // Отправьте обновленный элемент прогресса в IProgress.Report
                    _progressReporter.Report(postingProgressItem);

                    if (prices == null)
                    {
                        return;
                    }

                    // Передаю полученный прайс для записи в БД, 
                    if (_isModeRunAllWork)
                        await BuildingAdsAsync(prices, path, postingProgressItem);



                    //  Добавляю бренды в базу. Флаг регулирует в каком режиме находится метод,
                    // true = полная работа, false = только получение брендов из прайсов
                    if (!_isModeRunAllWork)
                        AddBrandsAtDb(prices);                    
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        // Метод построения объектов для публикации на основе каждого прайса
        private async Task BuildingAdsAsync(PriceList prices, string path, PostingProgressItem postingProgressItem = null!)
        {
            // Получаем к этому прайсу выбранные каналы
            PriceChannelMapping? priceChannels = GetChannelsForPrice(path);
            int elCount = 0;

            if (priceChannels == null)
            {
                MessageBox.Show("Что-то пошло не так, попробуйте выбрать прайс и каналы для него", "Внимание",
                   MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            string priceName = Path.GetFileName(path);

            foreach (var price in prices)
            {
                

                List<AdPublishingInfo> adPublishingInfoList = new List<AdPublishingInfo>();
                foreach (var priceChannelMapping in priceChannels.SelectedChannels)
                {
                    var channelBrands = GetBrandsForChannel(priceChannelMapping.Id);

                    // Проверяю бренд из прайса с выбранным для канала и прайса
                    if (!channelBrands.Any(b => string.Equals(
                        b.Name.Normalize(NormalizationForm.FormD).Replace(" ", "").ToUpper(),
                        price.Brand.Normalize(NormalizationForm.FormD).Replace(" ", "").ToUpper(),
                        StringComparison.OrdinalIgnoreCase)))
                    {
                        break; // Если не нашли совпадение, выходим из цикла
                    }


                    // Конструктор строителя объекта для публикации
                    var builder = new ChannelAdInfoBuilder(price, priceChannelMapping, path);
                    // Строю объект для публикации
                    var adInfo = await builder.Build();
                    if (adInfo == null) break;

                    // Фильтр цен перед сохранением объекта публикации в базе
                    PriceFilter priceFilter = new();
                    priceFilter.FilterAndSaveByPrice(adInfo);
                    elCount++;

                    Console.WriteLine($"Добавил {adInfo.Artikul} || {adInfo.Brand} из прайса {priceName} для канала {priceChannelMapping.Name}");
                }
            }

            Console.WriteLine($"Закончил обработку прайса {priceName}, всего элементов {elCount}");
        }

        // метод получения брендов для канала с отдельным контекстом, чтобы EF не залупался
        public List<Brand?> GetBrandsForChannel(int channelId)
        {
            using (var context = new AppContext())
            {
                return context.BrandChannelMappings
                    .Where(mapping => mapping.ChannelId == channelId)
                    .Include(mapping => mapping.Brand.ImageServices)
                    .Select(mapping => mapping.Brand)
                    .ToList();
            }
        }

        // Асинхронный метод публикации объявления        
        public async Task ProcessPublishingAdsAtDrom()
        {
            using var context = new AppContext();
            var adInfos = context.AdPublishingInfo.ToList(); // Загрузка всех объявлений

            PostingProgressItem postingProgressItem = new();

            // Возвращаемся в основной поток для обновления элементов интерфейса
            Application.Current.Dispatcher.Invoke(() =>
            {
                PostingProgressItems.Add(postingProgressItem);
            });

            var channels = adInfos.Select(adInfo => adInfo.AdDescription).Distinct();

            var tasks = new List<Task>();

            foreach (var channelName in channels)
            {
                var channelAdInfos = adInfos.Where(adInfo => adInfo.AdDescription == channelName).ToList();

                DromAdPublisher dromAdPublisher = new(channelName);

                tasks.Add(ProcessChannelAdsAsync(dromAdPublisher, channelAdInfos));
            }

            await Task.WhenAll(tasks);

            // Закрываю профили
            var channelsForClose = context.Channels.ToList();
            BrowserManager browser = new();
            foreach (var channel in channelsForClose)
            {
                await browser.CloseBrowser(channel.Name);
            }
        }

        // Метод публикации объявлений
        private async Task ProcessChannelAdsAsync(DromAdPublisher dromAdPublisher, List<AdPublishingInfo> channelAdInfos)
        {

            foreach (var adInfo in channelAdInfos)
            {
                if (adInfo.IsArchived == true) continue; // Если объявление в архиве
                if (adInfo.PriceBuy == "1") continue; // Если уже публиковал
                if (adInfo.Artikul == null || adInfo.Brand == null) continue; // Если бренд или артикул пустые

                PostingProgressItem postingProgressItem = new();
                postingProgressItem.TotalStages = channelAdInfos.Count;
                postingProgressItem.ProcessName = "Публикация объявлений на Drom.ru";


                bool isPublished = await dromAdPublisher.PublishAdAsync(adInfo);

                if (isPublished)
                {
                    Console.WriteLine($"Публикация {adInfo.Artikul} || {adInfo.Brand} || канал: {adInfo.AdDescription}");

                    using var context = new AppContext();
                    var existingAdInfo = context.AdPublishingInfo.Find(adInfo.Id);

                    if (existingAdInfo != null)
                    {
                        existingAdInfo.PriceBuy = "1";
                    }

                    try
                    {
                        context.AdPublishingInfo.Update(existingAdInfo);
                        context.SaveChanges(); // Сохраняем изменения в базе данных
                    }
                    catch (Exception ex)
                    {
                        // Обработка ошибок при добавлении в базу данных
                        Console.WriteLine($"ОШибка {ex.ToString()} в методе ProcessChannelAdsAsync");
                    }
                }

            }
        }


        // Метод для формирования прайса
        private string ExportPrice()
        {
            using var context = new AppContext();
            List<AdPublishingInfo> prices = context.AdPublishingInfo.ToList();
            ExcelPriceExporter priceExporter = new ExcelPriceExporter();
            string pricePath = priceExporter.ExportPricesToExcel(prices);

            return pricePath;
        }


        // Метод для перещения публикаций в архив
        private async Task RemoveAtArchive()
        {
            using var context = new AppContext();
            List<AdPublishingInfo> adPublishings = context.AdPublishingInfo.ToList();
            WorkOnAds remover = new();

            await remover.RemoveByFlag(SelectedChannel?.Name, adPublishings);
        }

        // Удаляю публикации не за сегодня (оставляю только актуальные)
        private void DeleteOutdatedAdsAtDb()
        {
            List<AdPublishingInfo> adsPublishedOutDateNow = GetOutdatedAds();

            try
            {
                _db.AdPublishingInfo.RemoveRange(adsPublishedOutDateNow);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                // TODO сделать логирование
            }
        }

        // Получаю объекты публикации не за сегодня
        private List<AdPublishingInfo> GetOutdatedAds()
        {
            var currentDate = DateTime.Now.Date;

            return _db.AdPublishingInfo
                .ToList()
                .Where(a => DateTime.Parse(a.DatePublished).Date != currentDate)
                .ToList();
        }

        // Метод поиска каналов для выбранного прайса
        private PriceChannelMapping GetChannelsForPrice(string path)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            PriceChannelMapping? priceChannels = PriceChannelMappings?.FirstOrDefault(mapping => mapping?.Price.Name == fileName);

            return priceChannels;
        }

        // Метод добавления брендов в базу
        private void AddBrandsAtDb(PriceList prices)
        {
            BrandImporter brandImporter = new();
            var newBrands = brandImporter.ImportBrandsFromPrices(prices);
            int countNewBrands = newBrands.Item1;
            List<string> newBrandsNames = newBrands.Item2;


            if (countNewBrands != 0)
            {
                // Возвращаемся в основной поток для обновления элементов интерфейса
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Brands.Clear();
                });
                Brands = new ObservableCollection<Brand>(_db.Brands.ToList()); // Обновляю свойство

                string brandNamesList = string.Join(", ", newBrandsNames);
                MessageBox.Show($"Добавлено {countNewBrands} брендов. Список: {brandNamesList}");
            }
            else
            {
                MessageBox.Show("Новых брендов не найдено");
            }

            _isModeRunAllWork = true; // Возвращаю в режим поной работы
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
                    // Сообщение об ошибке с указанием причины
                    MessageBox.Show($"Ошибка при обработке прайса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return null;
                }
            });

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(120)); // Время ожидания задачи

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
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
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

        // Метод инициализации базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = new AppContext();
                // загружаем данные о поставщиках из БД и включаем связанные данные (PriceIncreases и Brands)
                _db.Channels
                    .Include(channel => channel.Brands)
                    .Include(channel => channel.PriceIncreases)
                    .Load();

                _db.Brands
                    .Include(b => b.ImageServices)
                    .Load();
                _db.BrandImageServiceMappings.Load();
                _db.AdPublishingInfo.Load();
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

        #region Каналы

        // Вызываю из главного окна и передаю парамтеры
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

            // Обновляю  список выбранных каналов для данного прайса
            mapping.SelectedChannels = selectedChannels;

            if (!PriceChannelMappings.Contains(mapping))
                PriceChannelMappings.Add(mapping);
        }
        #endregion

        #region Брэнды
        // Открываю окно для выбора картинки и сохранения по дефолту
        private void SaveDefaultImageForBrand(Brand selectedBrand)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение для бренда",
                Filter = "Изображения (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                // Сохраните выбранный путь к файлу в поле DefaultImage для выбранного бренда
                selectedBrand.DefaultImage = selectedImagePath;

                // Сохраните изменения в базе данных
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Не удалось сохранить изображение по умолчанию {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public ObservableCollection<BrandWithSelectedImageServices> GetBrandsWithSelectedImageServices()
        {
            var brandImageServiceMappings = _db.BrandImageServiceMappings.ToList();
            var imageServices = _db.ImageServices.ToList();
            var brandWithSelectedImageServices = new ObservableCollection<BrandWithSelectedImageServices>();

            foreach (var brand in _db.Brands.ToList())
            {
                var selectedImageServices = new ObservableCollection<ImageService>();

                foreach (var imageService in imageServices)
                {
                    bool isSelected = brandImageServiceMappings.Any(mapping =>
                        mapping.BrandId == brand.Id && mapping.ImageServiceId == imageService.Id);

                    // Создайте копию ImageService
                    var clonedImageService = new ImageService
                    {
                        Id = imageService.Id,
                        // Копируйте остальные свойства по необходимости
                        // Например, Name, Description и другие
                    };

                    clonedImageService.IsSelected = isSelected;
                    selectedImageServices.Add(clonedImageService);
                }

                brandWithSelectedImageServices.Add(new BrandWithSelectedImageServices
                {
                    Brand = brand,
                    SelectedImageServices = selectedImageServices
                });
            }

            return brandWithSelectedImageServices;
        }

        #endregion

        #region Ставки
        // Устанавливаю ставки за просмотры для канала
        public async Task SetRates(List<string> parts, string rate, string selectedChannel)
        {
            await Task.Run(async () =>
            {
                WorkOnAds workOnAds = new();
                await workOnAds.SetRatesForWatchingAsync(parts, rate, selectedChannel);
            });
        }

        #endregion

        #endregion
    }
}
