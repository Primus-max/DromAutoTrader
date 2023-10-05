using DromAutoTrader.Models;
using DromAutoTrader.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DromAutoTrader.ViewModels
{
    internal class EditeChannelPageViewModel:BaseViewModel
    {
        #region Приватные поля
        private Channel _selectedChannel = null!;
        #endregion

        #region Публичные поля
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set => Set(ref _selectedChannel, value);
        }
        #endregion

        #region Команды
        public ICommand GetChannelCommand { get; } = null!;

        private bool CanGetChannelExecute(object p) => true;

        private void OnGetChannelExecuted(object sender)
        {
            Test();
        }
        #endregion
        public EditeChannelPageViewModel()
        {
            // Получаю выбранный Channel в AllChannelPage через LocatorService
            SelectedChannel = LocatorService.Current.SelectedChannel;

            #region Инициализация команд
            GetChannelCommand = new LambdaCommand(OnGetChannelExecuted, CanGetChannelExecute);
            #endregion
        }

        #region Методы
        public void Test()
        {
            string nameChannel = SelectedChannel.Name;
        }
        #endregion
    }
}
