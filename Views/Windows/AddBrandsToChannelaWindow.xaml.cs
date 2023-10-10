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
       
        public AddBrandsToChannelaWindow()
        {
            InitializeComponent();

            Loaded += Window_Loaded;

        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
            //BrandsListBox.ItemsSource = Brands;
            //BrandsListBox.DisplayMemberPath = "Name";
        }
    }
}
