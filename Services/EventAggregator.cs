namespace DromAutoTrader.Services
{
    public class EventAggregator : EventArgs
    {
        public static event Action? AddedBrandsCountChanged;

        public static void PublishAddedBrandsCountChanged()
        {
            AddedBrandsCountChanged?.Invoke();
        }
              
    }
}
