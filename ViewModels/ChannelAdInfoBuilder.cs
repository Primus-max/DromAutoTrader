using DromAutoTrader.Managers;

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
            if (_channel == null || _price == null || _pricePath == null)
                return new AdPublishingInfo();


            // Проверяем, что цена в прайсе не меньше чем в таблице накрутки цен
            decimal minTo = (decimal)(_channel?.PriceIncreases.Min(inc => (decimal)inc.From));

            if (_price.PriceBuy < minTo)
                return null!;

            // Иногда в прайсе Тисс к артикулу подписывают это, значит пропускаем
            if (_price.Artikul.Contains("ПоврежУпак"))
                return null!;

            List<string> imagesPaths = new List<string>();
            string? namePrice = Path.GetFileName(_pricePath);

            // Создаю калькулятор, считаю цену с накруткой
            CalcPrice calcPrice = new();
            decimal calculatedPrice = calcPrice.Calculate(_price.PriceBuy, _channel?.PriceIncreases);

            // Проверяю  на полное совпадение
            try
            {
                using var context = new AppContext();
                var isAdExists = context.AdPublishingInfo
               .Any(existing => existing.Artikul == _price.Artikul
                               && existing.Brand.ToLower() == _price.Brand.ToLower()
                               && existing.InputPrice == _price.PriceBuy                              
                               && existing.OutputPrice == (decimal)Math.Round(calculatedPrice, 0)                               
                               );

                if (isAdExists)
                    return null; // Если полное совпадение, то выходим
            }
            catch (Exception) { }

            _adPublishingInfo.PriceName = namePrice;
            _adPublishingInfo.Brand = _price?.Brand; // Имя брэнда
            _adPublishingInfo.Artikul = RemoveNonAlphanumeric(_price?.Artikul); // Артикул
            _adPublishingInfo.Description = _channel.Description; // Описание товара (из прайса) Пока нигде не потребовалось
            _adPublishingInfo.KatalogName = _price?.KatalogName; // Это попадает в заголовок объявления
            _adPublishingInfo.InputPrice = _price.PriceBuy; // Прайс на деталь от поставщика
            _adPublishingInfo.OutputPrice = (decimal)Math.Round(calculatedPrice, 0); // Округляю полученную цену и записываю
            _adPublishingInfo.AdDescription = _channel.Name; // Имя канала в котором опубликовал
            _adPublishingInfo.Count = _price.Count; // Количество запчастей у поставщика // TODO изменить при парсинге, иногда приходит 10-100 (от и до, в этом случае мы получаем 0)
            _adPublishingInfo.IsArchived = false;
            // Создаю дату регистрации объявления
            // // TODO(Делать только посе публикации объявления) Дата формирования объявления
            _adPublishingInfo.DatePublished = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss");

            // TODO сделать получение только после проверки налаичия элемента в базе
            SelectionImagesPathsService imagesPathsservice = new SelectionImagesPathsService(); // Фабрика для выбора нужного сервиса по поиску изображения
            imagesPaths = await imagesPathsservice.SelectPaths(_price?.Brand, _price?.Artikul); // Получаю путь к изображению
            _adPublishingInfo.ImagesPaths = imagesPaths; // Это временное хранение путей не для хранения в базе
            _adPublishingInfo.ImagesPath = string.Join(";", imagesPaths); // Формирую пути в одну строку с разделителем для хранения в базе

            // Сразу выгружаю фото на сервер
            ImagesManager imagesManager = new();
            await imagesManager.UploadImages(_price.Brand, _price.Artikul, imagesPaths);

            return _adPublishingInfo;
        }

        // Убираю лишние символы из арткула
        public static string RemoveNonAlphanumeric(string input)
        {
            // Создаем регулярное выражение, которое соответствует всем символам, кроме букв и цифр
            string pattern = @"[^\p{L}\p{N}]";
            // Заменяем найденные символы пустой строкой
            string result = Regex.Replace(input, pattern, "");
            return result;
        }
    }
}
