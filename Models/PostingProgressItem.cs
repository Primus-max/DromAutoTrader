namespace DromAutoTrader.Models
{
    public class PostingProgressItem
    {
        public string? ChannelName { get; set; }
        public string? PriceName { get; set; } // Имя прайса
        public string? ProcessName { get; set; } 
        public int CurrentStage { get; set; }
        public int TotalStages { get; set; }
        public int MaxValue { get; set; }
        public string? DatePublished { get; set; }
        public string? GetFileButton { get; set; }
        public string? PriceExportPath { get; set; } // Путь к экспортированному прайсу
    }

}
