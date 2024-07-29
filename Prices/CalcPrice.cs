namespace DromAutoTrader.Prices
{
    public  class CalcPrice
    {

        public  decimal Calculate(decimal inputPrice, List<TablePriceOfIncrease> priceOfIncreases)
        {
            decimal outputPrice = 0;

            foreach (var priceOfIncrease in priceOfIncreases)
            {
                if (inputPrice >= priceOfIncrease.From && inputPrice <= priceOfIncrease.To)
                {
                    outputPrice = inputPrice + priceOfIncrease.PriceIncrease;
                    break; // Мы нашли диапазон, можно завершить цикл
                }
            }

            return outputPrice;
        }
    }
}
