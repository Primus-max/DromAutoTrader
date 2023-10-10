using DromAutoTrader.Data;
using DromAutoTrader.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            //Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
            //BrandsListBox.ItemsSource = Brands;
            //BrandsListBox.DisplayMemberPath = "Name";
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            List<Brand> selectedBrands = BrandsListBox.SelectedItems.Cast<Brand>().ToList();

            _brandsToChannelaWindowViewModel.AddBrandsToChannelInDb(_selectedCHannelId, selectedBrands, this);
        }
    }
}
