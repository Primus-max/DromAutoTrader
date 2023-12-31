﻿namespace DromAutoTrader.Prices
{
    public class PublishedPrice: BasePrice
    {
        public int Id { get; set; }       
        public decimal InputPrice { get; set; }
        public decimal OutputPrice { get; set; }
        public string? DatePublished { get; set; }
        public bool IsArchived { get; set; }
    }
}
