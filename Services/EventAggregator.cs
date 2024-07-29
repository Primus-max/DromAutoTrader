namespace DromAutoTrader.Services
{
    public class EventAggregator : EventArgs
    {
        public static event Action? AddedBrandsCountChanged;

        public static void PublishAddedBrandsCountChanged()
        {
            AddedBrandsCountChanged?.Invoke();
        }

        public static event EventHandler<PostingProgressItem>? PostingProgressItemUpdated;

        public static void RaisePostingProgressItemUpdated(object sender, PostingProgressItem progressItem)
        {
            PostingProgressItemUpdated?.Invoke(sender, progressItem);
        }
    }
}
