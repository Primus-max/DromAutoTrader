﻿using DromAutoTrader.Data;
using DromAutoTrader.DromManager;
using DromAutoTrader.Prices;
using DromAutoTrader.ViewModels;
using DromAutoTrader.Views;
using DromAutoTrader.Views.Pages;
using OfficeOpenXml;
using OpenQA.Selenium;
using System.Windows.Data;
using System.Windows.Media;

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
        private MainWindowViewModel _mainViewModel = null!;
        private bool isPriceSelected = false;

        public MainWindow()
        {
            InitializeComponent();

            // Объявляю какакую версию EPPlus использую
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var viewModel = new MainWindowViewModel();
            _mainViewModel = viewModel;
            DataContext = _mainViewModel;          

            // Передаю Frame черерз локатор
            LocatorService.Current.ChannelFrame = ChannelFrame;

            #region Подписка на события
            Loaded += MainWindow_Loaded;
            #endregion
        }

        #region Cкрытые метод для разработчика
        // Скрытый метод для добавления сервисов для поиска картинок
        private void AddImageService()
        {
            AppContext _db = AppContextFactory.GetInstance();
            // загружаем данные о поставщиках из БД и включаем связанные данные (PriceIncreases и Brands)
            _db.ImageServices.Load();


            List<string> urls = new() { "https://berg.ru/", " https://uniqom.ru/", "https://lynxauto.info/", " https://luzar.ru/", " https://startvolt.com/", " https://irk.rossko.ru/", " https://tmparts.ru/" };

            foreach (var url in urls)
            {
                ImageService imageService = new ImageService();
                imageService.Name = url;

                _db.ImageServices.Add(imageService);
                _db.SaveChanges();
            }
        }
        #endregion

        #region Обработчики событий
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем начальное содержимое Frame при загрузке окна
            ChannelFrame.Navigate(new AllChannelPage());
        }

        #region ФИЛЬТРЫ
        #region Брэнды       

        private void BrandCollection_Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            if (e.Item is not Brand brand) return;
            if (string.IsNullOrEmpty(brand.Name)) return;

            string searhingText = FilterSearchParameterTextBox.Text;
            if (string.IsNullOrEmpty(searhingText)) return;

            if (brand.Name.Contains(searhingText, StringComparison.OrdinalIgnoreCase)) return;

            e.Accepted = false;
        }

        private void FilterSearchParameterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var collection = (CollectionViewSource)textBox.FindResource("BrandsCollection");

            collection.View.Refresh();
        }

        #region Выбор прайса и каналов для него
        private void PriceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isPriceSelected = PriceListBox.SelectedItem != null;
        }

        private void RadioButtonGroupChoiceChip_Selected(object sender, RoutedEventArgs e)
        {
            if (!isPriceSelected)
            {
                MessageBox.Show("Пожалуйста, выберите прайс кликнув на его имя, затем выберите каналы для этого прайса", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DependencyObject obj = (DependencyObject)e.OriginalSource;
            string selectedPrice = PriceListBox.SelectedItem?.ToString();
            Price price = new() { Name = selectedPrice };

            while (obj != null && obj != PriceListBox)
            {
                if (obj is ListBox listBox)
                {
                    List<Channel> selectedChannels = listBox.SelectedItems.Cast<Channel>().ToList();

                    if (!string.IsNullOrEmpty(price.Name))
                    {
                        _mainViewModel.OnSelectChannel(price, selectedChannels);
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, выберите прайс кликнув на его имя, затем выберите каналы для этого прайса", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            isPriceSelected = false;
        }














        #endregion

        #endregion

        #endregion

        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}

