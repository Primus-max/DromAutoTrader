﻿using DromAutoTrader.Prices;
using DromAutoTrader.Services;
using Microsoft.Win32;
using System.IO;

namespace DromAutoTrader.ViewModels
{
    internal class ParsingViewModel : BaseViewModel
    {
        private string? _rootPathDirectory = "brand_images";
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
            RunPars();
        }
        #endregion

        public ParsingViewModel()
        {
            SelectFilePriceCommand = new LambdaCommand(OnSelectFilePriceCommandExecuted, CanSelectFilePriceCommandExecute);
            StartParsingCommand = new LambdaCommand(OnStartParsingCommandExecuted, CanStartParsingCommandExecute);
        }

        #region Методы
        // Метод-точка входа в парсинг
        private void  RunPars()
        {
            if (PathsFilePrices == null) return;

            foreach (var path in PathsFilePrices)
            {
                if (string.IsNullOrEmpty(path))
                    MessageBox.Show("Для начала работы необходимо выбрать прайс");

                PriceList prices =  ProcessPrice(path);

                if (prices == null) return;              
               

                // Имя файла
                string fileName = Path.GetFileNameWithoutExtension(path);

                foreach (var price in prices)
                {
                    string? brand = price.Brand;
                    string? articul = price.Artikul;

                    FolderManager folderManager = new FolderManager();
                    folderManager.EnsureFolderStructure(_rootPathDirectory, brand, articul);

                    bool IsContainsFiles = folderManager.ArticulFolderContainsFiles(_rootPathDirectory, brand, articul);
                    if (IsContainsFiles) continue;

                    // TODO здесь запускаю парсинг по разным сервисам
                }
            }
        }

        // Метод парсинга файла excel
        public  PriceList ProcessPrice(string pathToFile)
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
