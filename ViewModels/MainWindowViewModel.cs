namespace DromAutoTrader.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private string? _title;
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public MainWindowViewModel()
        {
            Title = "Is fucking worked";
        }
    }
}
