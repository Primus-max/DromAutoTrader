using DromAutoTrader.Data;
using DromAutoTrader.DromManager;
using DromAutoTrader.Infrastacture.Commands;
using DromAutoTrader.Prices;
using Microsoft.Win32;
using System.Diagnostics;
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
        private ObservableCollection<AdPublishingInfo> _adPublishingInfos = null!;
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

        #region Брэнды
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
            //BrandWithSelectedImageServices = GetBrandsWithSelectedImageServices();
            // Brands = new ObservableCollection<Brand>(_db.Brands.ToList());

            Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
            TotalBrandCount = Brands.Count;

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
        }


        #region МЕТОДЫ

        // Метод запускающий всю работу
        public async Task RunAllWork()
        {
            //PriceProcessor priceProcessor = new();
            
            // Цикл по выбранным прайсам
            foreach (var path in PathsFilePrices)
            {
                if (string.IsNullOrEmpty(path))
                    MessageBox.Show("Для начала работы необходимо выбрать прайс");

                // Получаю только имя прайс
                string priceName = Path.GetFileName(path);

                AdPublishingInfo ad = new();
                ad.PriceName = priceName;

                PriceList prices = await ProcessPriceAsync(path);

                if (prices == null) return;

                AddBrandsAtDb(prices);

                // Модель для публикации объявления
                //AdPublishingInfo adPublishingInfo = new AdPublishingInfo();

                // Получаем к этому прайсу выбранные каналы                 
                PriceChannelMapping? priceChannels = GetChannelsForPrice(path);

                if (priceChannels == null)
                {
                    MessageBox.Show("Что-то пошло не так, попробуйте выбрать прайс и каналы для него", "Внимание",
                       MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }

                /************************************************************************/

                foreach (var price in prices)
                {          

                    var tasks = new List<Task>();
                    List<AdPublishingInfo> adPublishingInfoList = new List<AdPublishingInfo>();
                    foreach (var priceChannelMapping in priceChannels.SelectedChannels)
                    {
                        // Проверяю, есть ли бренд из price в текущем канале
                        if (!priceChannelMapping.Brands.Any(brand => brand.Name == price?.Brand))
                        {                            
                            break; // Если нашли совпадение, выходим из цикла
                        }

                        var task = ProcessChannelAsync(priceChannelMapping, price, adPublishingInfoList);
                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);

                }
            }
        }

        
        // Асинхронный метод для обработки каждого канала в собественном потоке
        public async Task ProcessChannelAsync(Channel priceChannelMapping, FormattedPrice price, List<AdPublishingInfo> adPublishingInfoList)
        {            
            // Создаем ChannelAdInfoBuilder для данного канала и цены
            var builder = new ChannelAdInfoBuilder(price, priceChannelMapping, path);

            // Строим AdPublishingInfo для данного канала
            var adInfo = await builder.Build();

            if (adInfo == null) return; 

            // TODO Здесь логика добавления объявления
            DromAdPublisher dromAdPublisher = new DromAdPublisher();
            bool isPublished = await dromAdPublisher.PublishAdAsync(adInfo, priceChannelMapping.Name);

            if (isPublished)
            {
                adInfo.ImagesPath = string.Join(";", adInfo?.ImagesPaths);
                adInfo.ImagesPaths = null;
                adInfo.AdDescription = priceChannelMapping.Name;
                try
                {
                    _db.AdPublishingInfo.Add(adInfo);
                    _db.SaveChanges();
                }
                catch (Exception)
                {
                    // Обработка ошибки при сохранении в базе данных
                }

                adPublishingInfoList.Add(adInfo);
            }
        }


        // Метод поиска каналов для выбранного прайса
        private PriceChannelMapping GetChannelsForPrice(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            PriceChannelMapping? priceChannels = PriceChannelMappings?.FirstOrDefault(mapping => mapping?.Price.Name == fileName);

            return priceChannels;
        }

        // Метод добавления брендов в базу
        private void AddBrandsAtDb(PriceList prices)
        {
            BrandImporter brandImporter = new();           
            brandImporter.ImportBrandsFromPrices(prices);
            Brands.Clear();
            Brands = new ObservableCollection<Brand>(_db.Brands.ToList()); // Обновляю свойство
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
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД и включаем связанные данные (PriceIncreases и Brands)
                _db.Channels
                    .Load();
                _db.Brands
                    .Include(b => b.ImageServices)
                    .Load();
                _db.BrandImageServiceMappings
                    // Загрузка связанных ImageService
                    .Load();
                _db.BrandImageServiceMappings.Load();
                _db.AdPublishingInfo.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                    MessageBox.Show($"Не удалось сохранить изображение по умолчанию {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        #endregion
    }
}
