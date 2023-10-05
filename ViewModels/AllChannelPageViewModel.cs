﻿using DromAutoTrader.Models;
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
            OpenEditChannelPage();
        }
        #endregion
        public AllChannelPageViewModel()
        {
            ChannelFrame = LocatorService.Current.ChannelFrame;

            OpenEditChannelPageCommand = new LambdaCommand(OnOpenEditChannelPageCommandExecuted, CanOpenEditChannelPageCommandExecute);

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

        #region Методы
        private void OpenEditChannelPage() 
        {
            ChannelFrame.Navigate(new EditeChannelPage());
        }
        #endregion
    }
}
