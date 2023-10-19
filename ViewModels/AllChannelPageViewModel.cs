using DromAutoTrader.Data;
using DromAutoTrader.DromManager;
using DromAutoTrader.Models;
using DromAutoTrader.Services;
using DromAutoTrader.Views;
using DromAutoTrader.Views.Pages;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace DromAutoTrader.ViewModels
{
    internal class AllChannelPageViewModel : BaseViewModel
    {
        #region Приватные свойства
        private ObservableCollection<Channel> _channels = null!;
        private Channel _selectedChannel = null!;
        private Frame _channelFrame = null!;
        private AppContext _db = null!;
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
        public Frame ChannelFrame
        {
            get => _channelFrame;
            set => Set(ref _channelFrame, value);
        }
        #endregion

        #region Команды
        public ICommand OpenEditChannelPageCommand { get; } = null!;

        private bool CanOpenEditChannelPageCommandExecute(object p) => true;

        private void OnOpenEditChannelPageCommandExecuted(object sender)
        {
            SelectedChannel = sender as Channel;
            OpenEditChannelPage();
        }
        #endregion
        public AllChannelPageViewModel()
        {
            // Получаю через Locator Frame от MainWindow
            ChannelFrame = LocatorService.Current.ChannelFrame;
            
            #region Инициализация команд
            OpenEditChannelPageCommand = new LambdaCommand(OnOpenEditChannelPageCommandExecuted, CanOpenEditChannelPageCommandExecute);
            #endregion

            InitializeDatabase();            

            // Получаю имена каналов из профилей AdsPower
            Task.Run(async () => await GetAndSetChannelNameFromPrifileAsync());
        }
                

        #region Методы
        private void OpenEditChannelPage() 
        {
            // Передаю выбранный Channel черерз локатор в EditChannelPageViewModel
            LocatorService.Current.SelectedChannel = SelectedChannel;

            ChannelFrame.Navigate(new EditeChannelPage());
        }        

        // Метод получения каналов из профилей 
        private async Task GetAndSetChannelNameFromPrifileAsync()
        {
            await ChannelManager.GetChannelsAsync();

            Channels = new ObservableCollection<Channel>(_db.Channels.ToList());
        }

        // Инициализация базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД и включаем связанные данные (PriceIncreases и Brands)
                _db.Channels.Include(c => c.PriceIncreases).Include(c => c.Brands).Load();
            }
            catch (Exception)
            {
                // TODO сделать запись логов
                //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }
        #endregion
    }
}
