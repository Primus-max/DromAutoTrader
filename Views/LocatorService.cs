namespace DromAutoTrader.Views
{
    internal class LocatorService
    {
        public Frame? ChannelFrame { get; set; }
        public Channel? SelectedChannel { get; set; }
        public ListBox? BrandsListBox { get; set; }
        
        public static LocatorService Current { get; } = new LocatorService();
    }
}
