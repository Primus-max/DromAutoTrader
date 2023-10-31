﻿using DromAutoTrader.Models;
using DromAutoTrader.Prices;
using DromAutoTrader.Services;

namespace DromAutoTrader.ViewModels
{
    public class ChannelAdInfoBuilder
    {
        private AdPublishingInfo? _adPublishingInfo = null;
        private readonly Channel? _channel = null;
        private readonly FormattedPrice? _price = null;

        public ChannelAdInfoBuilder(FormattedPrice price, Channel channel)
        {
            _price = price;
            _channel = channel;
            _adPublishingInfo = new();
        }

        public async Task<AdPublishingInfo> Build()
        {
            if (_channel == null) return new AdPublishingInfo();

            // Проверяем, что цена в прайсе не меньше чем в таблице накрутки цен
            decimal minTo = (decimal)(_channel?.PriceIncreases.Min(inc => (decimal)inc.To));

            if (_price.PriceBuy < minTo) return null;
            List<string> imagesPaths = new List<string>();

            _adPublishingInfo.Brand = _price?.Brand; // Имя брэнда
            _adPublishingInfo.Artikul = _price?.Artikul; // Артикул
            _adPublishingInfo.Description = _channel.Description; // Описание товара (из прайса) Пока нигде не потребовалось
            _adPublishingInfo.KatalogName = _price?.KatalogName; // Это попадает в заголовок объявления
            _adPublishingInfo.OutputPrice = CalcPrice.Calculate(_price.PriceBuy, _channel?.PriceIncreases); // Считаю цену исходя из цены прайса

            // Получаю пути к изображениям
            SelectionImagesPathsService imagesPathsservice = new SelectionImagesPathsService();
            imagesPaths = await imagesPathsservice.SelectPaths(_price?.Brand, _price?.Artikul);
            _adPublishingInfo.ImagesPaths = imagesPaths;
            // Создаю дату регистрации объявления           
            _adPublishingInfo.DatePublished = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // TODO получние адресов картинки (ок) 



            return _adPublishingInfo;

        }
    }
}
