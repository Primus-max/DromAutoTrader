namespace DromAutoTrader.Models
{
    public enum ProcessState
    {
        Start,
        ParsingPrice,
        BuildingAdsForPublish,
        FilteringAdsByPrice,       
        Publishing,
        ExportingPrice,
        RemovingAdsArchive
    }
}
