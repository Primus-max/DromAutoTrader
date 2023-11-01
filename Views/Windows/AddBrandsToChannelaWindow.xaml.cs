using DromAutoTrader.Data;
using DromAutoTrader.Services;
using DromAutoTrader.ViewModels;
using System.Drawing;
using System.Windows.Data;

namespace DromAutoTrader.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddBrandsToChannelaWindow.xaml
    /// </summary>
    public partial class AddBrandsToChannelaWindow : Window
    {
        public int _selectedCHannelId;
        private AddBrandsToChannelaWindowViewModel _brandsToChannelaWindowViewModel = null!;
        private AppContext _db = null!;

        public AddBrandsToChannelaWindow(int channelId)
        {
            InitializeComponent();
            InitializeDatabase();

            _selectedCHannelId = channelId;

            _brandsToChannelaWindowViewModel = new AddBrandsToChannelaWindowViewModel();

            Loaded += Window_Loaded;

            LocatorService.Current.BrandsListBox = BrandsListBox;
        }

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
            var collection = (CollectionViewSource)textBox.FindResource("BrandsCollenctionViewSoiurce");

            collection.View.Refresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            List<Brand> selectedBrands = BrandsListBox.SelectedItems.Cast<Brand>().ToList();

            _brandsToChannelaWindowViewModel.UpdateBrandChannelMappings(_selectedCHannelId, selectedBrands, this);

            SelectItemsForChannel();

            EventAggregator.PublishAddedBrandsCountChanged();           
        }

        private void BrandsListBox_Loaded(object sender, RoutedEventArgs e)
        {
            SelectItemsForChannel();
        }

        // Метод отображения выбранных брэндов для канала
        private void SelectItemsForChannel()
        {
            foreach (BrandChannelMapping mapping in _db.BrandChannelMappings.Include(m => m.Brand).Where(m => m.ChannelId == _selectedCHannelId))
            {
                Brand brand = mapping.Brand;
                int index = BrandsListBox.Items.IndexOf(brand);

                if (index >= 0 && index < BrandsListBox.Items.Count)
                {
                    ListBoxItem listBoxItem = BrandsListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;

                    if (listBoxItem != null)
                    {
                        listBoxItem.IsSelected = true;
                    }
                }
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
               
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

    }
}
