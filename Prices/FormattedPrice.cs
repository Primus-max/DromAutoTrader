using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromAutoTrader.Prices
{
    public class FormattedPrice
    {
        public string Brand { get; set; }
        public string Artikul { get; set; }
        public string Description { get; set; }
        public decimal PriceBuy { get; set; }
        public int Count { get; set; }
        public string KatalogName { get; set; }
    }
}
