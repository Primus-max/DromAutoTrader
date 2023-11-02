using DromAutoTrader.Prices;
using DromAutoTrader.Services;
using System.IO;

namespace DromAutoTrader.ViewModels
{
    public class ChannelAdInfoBuilder
    {
        private AdPublishingInfo? _adPublishingInfo = null;
        private readonly Channel? _channel = null;
        private readonly FormattedPrice? _price = null;
        private readonly string? _pricePath = null;

        public ChannelAdInfoBuilder(FormattedPrice price, Channel channel, string pricePath)
        {
            _price = price;
            _channel = channel;
            _adPublishingInfo = new();
            _pricePath = pricePath;
        }

        public async Task<AdPublishingInfo> Build()
        {
            if (_channel == null) return new AdPublishingInfo();

            // Проверяем, что цена в прайсе не меньше чем в таблице накрутки цен
            decimal minTo = (decimal)(_channel?.PriceIncreases.Min(inc => (decimal)inc.To));

            if (_price.PriceBuy < minTo) return null;
            List<string> imagesPaths = new List<string>();
            string? namePrice = Path.GetFileName(_pricePath);

            _adPublishingInfo.PriceName = namePrice;
            _adPublishingInfo.Brand = _price?.Brand; // Имя брэнда
            _adPublishingInfo.Artikul = _price?.Artikul; // Артикул
            _adPublishingInfo.Description = _channel.Description; // Описание товара (из прайса) Пока нигде не потребовалось
            _adPublishingInfo.KatalogName = _price?.KatalogName; // Это попадает в заголовок объявления
            _adPublishingInfo.InputPrice = _price.PriceBuy; // Прайс на деталь от поставщика
            _adPublishingInfo.OutputPrice = CalcPrice.Calculate(_price.PriceBuy, _channel?.PriceIncreases); // Считаю цену исходя из цены прайса
            _adPublishingInfo.AdDescription = _channel.Name; // Имя канала в котором опубликовал
            _adPublishingInfo.Count = _price.Count; // Количество запчастей у поставщика
            SelectionImagesPathsService imagesPathsservice = new SelectionImagesPathsService(); // Фабрика для выбора нужного сервиса по поиску изображения
            imagesPaths = await imagesPathsservice.SelectPaths(_price?.Brand, _price?.Artikul); // Получаю путь к изображению
            _adPublishingInfo.ImagesPaths = imagesPaths; // TODO временное хранение путей в виде List, далее надо обнулить (в базе не хранится)
            _adPublishingInfo.ImagesPath = string.Join(";", imagesPaths); // Формирую пути в одну строку с разделителем для хранения в базе
            // Создаю дату регистрации объявления
            // // TODO(Делать только посе публикации объявления) Дата формирования объявления
            _adPublishingInfo.DatePublished = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss");


            return _adPublishingInfo;

        }
    }
}
