using DromAutoTrader.ImageServices;
using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Prices;
using Microsoft.Win32;

namespace DromAutoTrader.ViewModels
{
    internal class ParsingViewModel : BaseViewModel
    {
        // Словарь с сервисами
        Dictionary<int, Type> imageServiceTypes = new Dictionary<int, Type>
        {
                { 1, typeof(BergImageService) },
                { 2, typeof(IrkRosskoImageService) },
                { 3, typeof(LynxautoImageService) },
                { 4, typeof(LuzarImageService) },
                { 5, typeof(StarvoltImageService) },
                { 6, typeof(UnicomImageService) },
                { 7, typeof(TmpartsImageService) },
        };

        public List<string>? PathsFilePrices { get; private set; }
        public List<string>? Prices { get; private set; }

        #region Команды
        public ICommand SelectFilePriceCommand { get; } = null!;

        private bool CanSelectFilePriceCommandExecute(object p) => true;

        private void OnSelectFilePriceCommandExecuted(object sender)
        {
            GetSelectedFilePaths();
        }

        public ICommand StartParsingCommand { get; } = null!;

        private bool CanStartParsingCommandExecute(object p) => true;

        private void OnStartParsingCommandExecuted(object sender)
        {
            Thread thread = new Thread(RunPars);
            thread.Start();
        }
        #endregion

        public ParsingViewModel()
        {
            SelectFilePriceCommand = new LambdaCommand(OnSelectFilePriceCommandExecuted, CanSelectFilePriceCommandExecute);
            StartParsingCommand = new LambdaCommand(OnStartParsingCommandExecuted, CanStartParsingCommandExecute);
        }

        #region Методы
        // Метод-точка входа в парсинг
        private async void RunPars()
        {
            if (PathsFilePrices == null) return;

            foreach (var path in PathsFilePrices)
            {
                if (string.IsNullOrEmpty(path))
                    MessageBox.Show("Для начала работы необходимо выбрать прайс");

                PriceList prices = ProcessPrice(path);

                if (prices == null) return;


                // Имя файла
                string fileName = Path.GetFileNameWithoutExtension(path);

                List<string>? downLoadedImagesPaths = new List<string>();

                foreach (var price in prices)
                {
                    string? brand = price.Brand;
                    string? articul = price.Artikul;

                    FolderManager folderManager = new FolderManager();
                    folderManager.EnsureFolderStructure(brand, articul);

                    bool IsContainsFiles = folderManager.ArticulFolderContainsFiles(brand, articul);
                    if (IsContainsFiles) continue;

                    // Прохожу по сервисам, ищу где есть изображение
                    //foreach (var service in imageServiceTypes.Values)
                    //{
                    //    if (Activator.CreateInstance(service) is IImageService imageService)
                    //    {
                    //        // Выполнение RunAsync для найденного сервиса
                    //        await imageService.RunAsync(brand, articul);

                    //        // Получение результатов
                    //        downLoadedImagesPaths = imageService.BrandImages;
                    //    }

                    //    if(downLoadedImagesPaths is not null || downLoadedImagesPaths?.Count > 0) break; // Если скачали, то выходим
                    //}

                    //BergImageService bergImageService = new();
                    // UnicomImageService unicomImageService = new();
                    //LynxautoImageService lynxautoImageService = new LynxautoImageService();
                    //LuzarImageService LuzarimageService = new LuzarImageService();
                    //StarvoltImageService starvoltImageService = new StarvoltImageService();
                    //IrkRosskoImageService irkRosskoImageService = new();
                    MxgroupImageService imageService = new MxgroupImageService();
                    // TmpartsImageService tmpartsImageService = new TmpartsImageService();
                    // await bergImageService.RunAsync(brand, articul);
                     await imageService.RunAsync(brand, articul);

                     var testImages = imageService.BrandImages;
                }
            }
        }



        // Метод парсинга файла excel
        public PriceList ProcessPrice(string pathToFile)
        {
            PriceProcessor priceProcessor = new();

            try
            {
                return priceProcessor.ProcessExcelPrice(pathToFile);
            }
            catch (Exception ex)
            {
                // Вывести сообщение об ошибке с указанием причины
                Console.WriteLine($"Ошибка при обработке прайса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new();
            }
        }

        // Открываю окно для выбора файлов (прайсов)
        private void GetSelectedFilePaths()
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
        }
        #endregion
    }
}
