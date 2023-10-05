using System.Windows.Controls;
using Channel = DromAutoTrader.Models.Channel;

namespace DromAutoTrader.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditeChannelPage.xaml
    /// </summary>
    public partial class EditeChannelPage : Page
    {
        private Channel _selectedChannel;

        public EditeChannelPage(Channel channel)
        {
            _selectedChannel = channel;

            InitializeComponent();
        }


    }
}
