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

        public AddBrandsToChannelaWindow(int channelId)
        {
            InitializeComponent();

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
            
            SelectItemsForChannel();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            List<Brand> selectedBrands = BrandsListBox.SelectedItems.Cast<Brand>().ToList();

            _brandsToChannelaWindowViewModel.AddBrandsToChannelInDb(_selectedCHannelId, selectedBrands, this);

            SelectItemsForChannel();
        }

        // Метод отображения выбранных брэндов для канала
        private void SelectItemsForChannel()
        {
            foreach (Brand brand in BrandsListBox.Items)
            {
                if (brand.ChannelId == _selectedCHannelId)
                {
                    ListBoxItem listBoxItem = BrandsListBox.ItemContainerGenerator.ContainerFromItem(brand) as ListBoxItem;

                    if (listBoxItem != null)
                    {
                        listBoxItem.IsSelected = true;
                    }
                }
            }
        }


    }
}
