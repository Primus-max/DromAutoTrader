using DromAutoTrader.ImageServices;
using DromAutoTrader.Prices;
using DromAutoTrader.Services;
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace DromAutoTrader.ViewModels
{
    internal class ParsingViewModel : BaseViewModel
    {

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

                //BergImageService bergImageService = new();
                //UnicomImageService unicomImageService = new();
                //LynxautoImageService lynxautoImageService = new LynxautoImageService();
                LuzarImageService LuzarimageService = new LuzarImageService();

                foreach (var price in prices)
                {
                    string? brand = price.Brand;
                    string? articul = price.Artikul;

                    FolderManager folderManager = new FolderManager();
                    folderManager.EnsureFolderStructure(brand, articul);

                    bool IsContainsFiles = folderManager.ArticulFolderContainsFiles(brand, articul);
                    if (IsContainsFiles) continue;

                    // TODO здесь запускаю парсинг по разным сервисам


                   // await bergImageService.RunAsync(brand, articul);
                    await LuzarimageService.RunAsync(brand, articul);

                    var testImages = LuzarimageService.BrandImages;
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
                MessageBox.Show($"Ошибка при обработке прайса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                // Перебросить исключение для обработки в вызывающем коде, если это необходимо
                throw;
            }
        }

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
