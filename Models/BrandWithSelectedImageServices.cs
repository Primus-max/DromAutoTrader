namespace DromAutoTrader.Models
{
    public class BrandWithSelectedImageServices
    {
        public Brand? Brand { get; set; }
        public ObservableCollection<ImageService>? SelectedImageServices { get; set; }
    }
}
