namespace DromAutoTrader.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для PageDescriptionTextBlock.xaml
    /// </summary>
    public partial class PageDescriptionTextBlock : UserControl
    {
        public static readonly DependencyProperty DesctiptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(PageDescriptionTextBlock), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return  (string)GetValue(DesctiptionProperty);  }
            set { SetValue(DesctiptionProperty, value);}
        }

        public PageDescriptionTextBlock()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
