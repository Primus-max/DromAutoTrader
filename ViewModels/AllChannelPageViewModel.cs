using DromAutoTrader.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DromAutoTrader.ViewModels
{
    internal class AllChannelPageViewModel : BaseViewModel
    {
        #region Приватные свойства
        private ObservableCollection<Channel> _channels = null!;
        private Channel _selectedChannel = null!;
        #endregion

        #region Публичные свойства
        public ObservableCollection<Channel> Channels
        {
            get => _channels;
            set => Set(ref _channels, value);
        }
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set => Set(ref _selectedChannel, value);
        }
        #endregion

        #region Команды
        public ICommand OpenEditChannelWindowCommand { get; } = null!;

        private bool CanOpenEditChannelWindowCommandExecute(object p) => true;

        private void OnOpenEditChannelWindowCommandExecuted(object sender)
        {

        }
        #endregion
        public AllChannelPageViewModel()
        {
            OpenEditChannelWindowCommand = new LambdaCommand(OnOpenEditChannelWindowCommandExecuted, CanOpenEditChannelWindowCommandExecute);

            // Тестовые данные
            Channels = new ObservableCollection<Channel>();
            for (int i = 0; i < 5; i++)
            {
                Channel channel = new Channel
                {
                    Id = i,
                    Name = "Имя канала" + i.ToString(),
                };

                Channels.Add(channel);
            }
        }
    }
}
