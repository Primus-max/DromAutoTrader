namespace DromAutoTrader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Подписываемся на событие DispatcherUnhandledException при инициализации приложения
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Обработка необработанного исключения, например, логирование
            Console.WriteLine($"Unhandled Exception: {e.Exception.Message}");

            // Помечаем исключение как обработанное (если не пометить, приложение может завершиться)
            e.Handled = true;

            // Можете добавить дополнительные действия, например, запись в журнал ошибок или отправку уведомления
        }
    }
}
