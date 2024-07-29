namespace DromAutoTrader.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        public MultiSelectComboBox()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Channel>), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        public IEnumerable<Channel> ItemsSource
        {
            get { return (IEnumerable<Channel>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable<Channel>), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        public IEnumerable<Channel> SelectedItems
        {
            get { return (IEnumerable<Channel>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
    }
        
}
