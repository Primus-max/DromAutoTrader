namespace DromAutoTrader.Services
{
    public class EventAggregator : EventArgs
    {
        public static event Action? AddedBrandsCountChanged;

        public static void PublishAddedBrandsCountChanged()
        {
            AddedBrandsCountChanged?.Invoke();
        }

        // Уведомление об обновлении UI если загрузились новые каналы
        public static event Action? AddedNewChannels;
        public static void PublishAddedNewChannels()
        {
            AddedNewChannels?.Invoke();
        }
    }
}
