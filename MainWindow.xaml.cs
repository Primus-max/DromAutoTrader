using AdsPowerManager;
using DromAutoTrader.DromManager;
using DromAutoTrader.ViewModels;
using DromAutoTrader.Views;
using DromAutoTrader.Views.Pages;
using OfficeOpenXml;
using OpenQA.Selenium;

namespace DromAutoTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Frame ChannelFrameInstance
        //{
        //    get { return ChannelFrame; }
        //}

        public MainWindow()
        {
            InitializeComponent();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            viewModel.SupplierDataGrid = SupplierDataGrid;

            // Передаю Frame черерз локатор
            LocatorService.Current.ChannelFrame = ChannelFrame;

            Loaded += MainWindow_Loaded;
            TestProfileName();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем начальное содержимое Frame при загрузке окна
            ChannelFrame.Navigate(new AllChannelPage());
        }

        public async void TestProfileName()
        {
           await ChannelManager.GetChannelsAsync();

            //List<Profile> profiles = await ProfileManager.GetProfiles();
            //BrowserManager browserManager = new BrowserManager();


            //foreach (Profile profile in profiles)
            //{
            //    if (profile.Name == "MainCraftIrk")
            //    {
            //        var profileName = profile.Name;
            //        IWebDriver driver = await browserManager.InitializeDriver(profileName);

            //        DromAdPublisher dromAdPublisher = new DromAdPublisher(driver);
            //        dromAdPublisher.PublishAd("Защитный комплект амортизатора 16F F10066 (2шт/упак)");
            //    }

            //}
        }


        //public void TestParsing()
        //{
        //    PriceProcessor priceProcessor = new PriceProcessor();
        //    BrandImporter brandImporter = new BrandImporter();

        //    string directoryPath = @"C:\Users\FedoTT\source\repos\DromAutoTrader\ТЗ\Прайсы";

        //    if (Directory.Exists(directoryPath))
        //    {
        //        string[] fileEntries = Directory.GetFiles(directoryPath, "*.xlsx"); // Получить все файлы с расширением .xlsx

        //        foreach (string filePath in fileEntries)
        //        {
        //            var prices = priceProcessor.ProcessExcelPrice(filePath);
        //            brandImporter.ImportBrandsFromPrices(prices);
        //        }
        //    }
        //    else
        //    {
        //        // TODO запись логов
        //        //Console.WriteLine("Указанная директория не существует.");
        //    }








        // Метод для инициализации базы данных


        //#region ПОСТАВЩИКИ

        //#region Методы

        //// TODO реалилзовать метод сохранения данных при поетери фокуса на поле

        //// Событие потери фокусу на DataGrid
        ////private void SupplierDataGrid_LostFocus(object sender, RoutedEventArgs e)
        ////{
        ////    if (SelectedSupplier != null && !SupplierDataGrid.IsReadOnly)
        ////    {
        ////        if (SelectedSupplier.Name == "Имя поставщика")
        ////        {
        ////            // Удаляем поставщика с пустым именем
        ////            DeleteSupplierFromDatabase(SelectedSupplier.Id);
        ////            Suppliers.Remove(SelectedSupplier);
        ////        }
        ////        else
        ////        {
        ////            // Сохраняем изменения в базе данных
        ////            UpdateSupplierInDatabase(SelectedSupplier.Id, SelectedSupplier.Name);
        ////        }

        ////        SupplierDataGrid.IsReadOnly = true;
        ////    }
        ////}


        //private void SupplierDataGrid_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (sender is DataGrid dataGrid)
        //        {
        //            if (dataGrid.SelectedItem is Supplier selectedSupplier)
        //            {
        //                if (selectedSupplier.Name == "Имя поставщика")
        //                {
        //                    // Удаляем поставщика с пустым именем
        //                    DeleteSupplierFromDatabase(selectedSupplier.Id);
        //                    Suppliers.Remove(selectedSupplier);
        //                }
        //                else
        //                {
        //                    // Сохраняем изменения в базе данных
        //                    UpdateSupplierInDatabase(selectedSupplier.Id, selectedSupplier.Name);
        //                }

        //                dataGrid.IsReadOnly = true;
        //            }
        //        }
        //    }
        //}


        //private void EditButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Включение редактирования имени поставщика
        //    var button = (Button)sender;
        //    var supplier = (Supplier)button.DataContext;
        //    if (supplier != null)
        //    {
        //        supplier.Name = string.Empty; // Очищаем имя для редактирования
        //    }            
        //}

        //private void DeleteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Удаление выбранного поставщика
        //    var button = (Button)sender;
        //    var supplier = (Supplier)button.DataContext;
        //    if (supplier != null)
        //    {
        //        Suppliers.Remove(supplier);
        //    }
        //}

        //private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        //{

        //    SupplierDataGrid.IsReadOnly = false;

        //    // Создаем нового поставщика с пустым именем
        //    var newSupplier = new Supplier { Id = Suppliers.Count + 1, };

        //    // Добавляем его в список поставщиков
        //    Suppliers.Add(newSupplier);

        //    // Устанавливаем источник данных для DataGrid
        //    SupplierDataGrid.ItemsSource = Suppliers;

        //}

        //// Метод для получения всех поставщиков из базы данных
        //public List<Supplier> GetAllSuppliers()
        //{
        //    try
        //    {
        //        return _db.Suppliers.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Не удалось получить список поставщиков: {ex.Message}");
        //        return new List<Supplier>(); // Вернуть пустой список или обработать ошибку иным способом
        //    }
        //}

        //private void AddSupplierToDatabase(string name)
        //{
        //    var newSupplier = new Supplier { Name = name };
        //    _db.Suppliers.Add(newSupplier);
        //    _db.SaveChanges();
        //}


        //private void UpdateSupplierInDatabase(int id, string newName)
        //{
        //    var supplierToUpdate = _db.Suppliers.FirstOrDefault(s => s.Id == id);
        //    if (supplierToUpdate != null)
        //    {
        //        supplierToUpdate.Name = newName;
        //        _db.SaveChanges();
        //    }
        //}


        //private void DeleteSupplierFromDatabase(int id)
        //{
        //    var supplierToRemove = _db.Suppliers.FirstOrDefault(s => s.Id == id);
        //    if (supplierToRemove != null)
        //    {
        //        _db.Suppliers.Remove(supplierToRemove);
        //        _db.SaveChanges();
        //    }
        //}
        //#endregion

        //#endregion


        // Другие методы и события вашего MainWindow
    }
}

