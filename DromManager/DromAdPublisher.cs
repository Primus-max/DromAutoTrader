using Newtonsoft.Json;
using System.Net;

public class DromAdPublisher
{
    private string gooodsUrl = "https://baza.drom.ru/adding?type=goods";
    private CookieContainer _cookieContainer = new CookieContainer();
    private readonly HttpClientHandler _handler;
    private readonly HttpClient _httpClient;
    private IWebDriver _driver = null!;
    private BrowserManager adsPower = null!;
    private List<Profile> profiles = null!;
    private WebDriverWait _wait = null!;
    private readonly string? _channelName = null!;
    private readonly Logger _logger;
    private string _logMessage;

    public DromAdPublisher(string channelName)
    {
        _channelName = channelName;
        adsPower = new BrowserManager();

        _logger = new LoggingService().ConfigureLogger();

        // Инициализация драйвера Chrome
        InitializeDriver(channelName).GetAwaiter().GetResult();

        // Открываю страницу, сохраняю куки
        NavigateToGoodsUrlAsync();

        _handler = new HttpClientHandler { CookieContainer = _cookieContainer };
        _httpClient = new HttpClient(_handler);
    }

    public async Task<long> Run(AdPublishingInfo adPublishingInfo)
    {
        if (adPublishingInfo == null) return 0;

        // Если флаг для обновления, обновляем и выходим
        if (adPublishingInfo.Status == "Updating")
        {
            await UdateBulletinAsync(adPublishingInfo);
            return 0;
        }

        // На всякий случай делаю рандом
        Random random = new Random();
        await Task.Delay(random.Next(4000, 12000));
        // Иначе публикуем
        long dromId = await SavebulletinAsync(adPublishingInfo);

        return dromId;
    }


    // Создаю и добавляю объявление
    private async Task<long> SavebulletinAsync(AdPublishingInfo adPublishingInfo)
    {
        if (adPublishingInfo == null)
            return 0;

        long dromId = 0;

        var Fields = new Dictionary<string, object>
        {
            { "subject", adPublishingInfo?.KatalogName }, // Заголовок
            { "condition", "new" },
            { "autoPartsOemNumber", adPublishingInfo?.Artikul }, // Артикул
            { "autoPartsAuthenticity", "original" },
            { "manufacturer", adPublishingInfo?.Brand }, // Бренд
            { "text", adPublishingInfo?.Description }, // Описание
            { "goodPresentState", "present"}, // В наличии
            {  "cityId", 340},
            { "guarantee", "Принимаем товар в оригинальной упаковке и без следов установки к обмену и возврату в течение 7 дней со дня покупки"},
            { "delivery.comment", "Наша компания осуществляет доставку по городу Иркутск совершенно бесплатно, оплата производится после получения товара на руки!, Оплата наличными,, безналичный расчет, или переводом на карту СБЕР ВТБ"},


        };

        // Изображения
        var imeageIds = await UploadImagesAsync(adPublishingInfo); // Загружаю изображения, получаю их ID
        var Images = new Images()
        {
            isShowCompanyLogo = false,
            images = imeageIds,
            masterImageId = imeageIds.FirstOrDefault(),
        };

        // Контакты
        var contacts = new
        {
            contactInfo = "+7 914 905-70-76",
            email = "",
            is_email_hidden = false
        };

        // Объект для отправки
        var payload = new PayLoad
        {
            AddingType = "bulletin",
            DirectoryId = 14,
            Fields = Fields,
            images = Images
        };

        // Дополнительный поля
        Fields.Add("price", new List<object> { adPublishingInfo?.OutputPrice, "RUB" });
        Fields.Add("contacts", contacts);
        string side = adPublishingInfo.KatalogName.Contains("лев") ? "left" : "right";
        Fields.Add("name", side);


        // Преобразовываем Payload в нужный формат
        var formattedPayload = $"changeDescription={System.Text.Json.JsonSerializer.Serialize(payload)}";

        try
        {
            // Отправка POST-запроса с использованием StringContent
            var content = new StringContent(formattedPayload, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = _httpClient.PostAsync("https://baza.drom.ru/api/1.0/save/bulletin", content).Result;

            // Обработка ответа
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                DromResponse responseObj = JsonConvert.DeserializeObject<DromResponse>(responseContent);

                if (responseObj == null) return dromId;
                if (responseObj.Id == 0) return dromId;

                // Получаю id дрома на это объявление
                adPublishingInfo.DromeId = responseObj.Id;

                if (responseObj.isDraft == true)
                {
                    dromId =  await PublishAdAsync(adPublishingInfo);
                    return dromId;
                }  
            }
            else
            {
                _logMessage = $"Не удалось добавить объявление, {adPublishingInfo?.Id}";
                _logger.Error(_logMessage);
                return dromId;
            }
        }
        catch (Exception ex)
        {
            _logMessage = $"Не удалось добавить объявление, {adPublishingInfo?.Id} по причине: {ex.Message}";
            _logger.Error(_logMessage);
            return dromId;
        }
        return dromId;
    }

    // Выгрузка изображений
    public async Task<List<long>> UploadImagesAsync(AdPublishingInfo adPublishingInfo)
    {
        if (adPublishingInfo == null || adPublishingInfo?.ImagesPaths?.Count == 0)
            return null!;

        var ids = new List<long>();
        var apiUrl = "https://baza.drom.ru/upload-image-jquery";

        List<string> images = adPublishingInfo?.ImagesPath?.Split(";").ToList();
        if (images.Count == 0) return ids;

        foreach (var image in images)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileBytes = File.ReadAllBytes(image); // Предполагается, что в вашем классе Image есть свойство Path
                content.Add(new ByteArrayContent(fileBytes), "up[]", "image.jpg");

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var decode = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    var imageId = (long?)decode?.id;

                    if (imageId.HasValue)
                    {
                        ids.Add(imageId.Value);
                    }
                }
                else
                {
                    _logMessage = $"Не удалось загрузить изображение, {adPublishingInfo?.Id} ";
                    _logger.Error(_logMessage);
                    return ids;
                }
            }
            catch (Exception ex)
            {
                _logMessage = $"Не удалось загрузить изображение, {adPublishingInfo?.Id} по причине: {ex.Message}";
                _logger.Error(_logMessage);
                return ids;
            }
        }

        return ids;
    }

    // Публикация
    public async Task<long> PublishAdAsync(AdPublishingInfo adPublishingInfo)
    {
        if (adPublishingInfo.DromeId <= 0 || adPublishingInfo == null)
            return 0;

        var apiUrl = $"https://baza.drom.ru/bulletin/{adPublishingInfo.DromeId}/draft/publish";

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return adPublishingInfo.DromeId;
            }
            else
            {
                _logMessage = $"Не удалось опубликовать объявление, {adPublishingInfo.Id}";
                _logger.Error(_logMessage);
                return 0;
            }
        }
        catch (Exception ex)
        {
            _logMessage = $"Не удалось опубликовать объявление, {adPublishingInfo?.Id} по причине: {ex.Message}";
            _logger.Error(_logMessage);
            return 0;
        }
    }

    // Метод обновления объявления
    private async Task UdateBulletinAsync(AdPublishingInfo adPublishingInfo)
    {
        string url = $"https://baza.drom.ru/bulletin/{adPublishingInfo.DromeId}/edit";

        if (adPublishingInfo == null)
            return;


        var Fields = new Dictionary<string, object>
        {
            { "subject", adPublishingInfo?.KatalogName }, // Заголовок
            { "condition", "new" },
            { "autoPartsOemNumber", adPublishingInfo?.Artikul }, // Артикул
            { "autoPartsAuthenticity", "original" },
            { "manufacturer", adPublishingInfo?.Brand }, // Бренд
            { "text", adPublishingInfo?.Description }, // Описание
            { "goodPresentState", "present"}, // В наличии
            {  "cityId", 340},
        };

        var imeageIds = await UploadImagesAsync(adPublishingInfo); // Загружаю изображения, получаю их ID
        var Images = new Images()
        {
            isShowCompanyLogo = false,
            images = imeageIds,
            masterImageId = imeageIds.FirstOrDefault(),
        };

        var contacts = new
        {
            //contactInfo = _channelName == "AutoBot38" ? "+7 950 077-76-98" : "+7 914 905-70-76",
            contactInfo =  "+79144848459",
            email = "hironero11@yandex.ru",
            is_email_hidden = false
        };

        var payload = new PayLoad
        {
            AddingType = "bulletin",
            DirectoryId = 14,
            Fields = Fields,
            images = Images
        };


        Fields.Add("price", new List<object> { adPublishingInfo?.OutputPrice, "RUB" });
        Fields.Add("contacts", contacts);

        // Преобразовываем Payload в нужный формат
        var formattedPayload = $"changeDescription={System.Text.Json.JsonSerializer.Serialize(payload)}";

        try
        {
            // Отправка POST-запроса с использованием StringContent
            var content = new StringContent(formattedPayload, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = _httpClient.PostAsync(url, content).Result;
        }
        catch (Exception ex)
        {
            _logMessage = $"Не удалось обновить объявление {adPublishingInfo.DromeId}, причина {ex.Message}";
            _logger.Error(_logMessage);
            return;
        }
    }


    // Метод перехода по ссылке и сохранения куки
    private void NavigateToGoodsUrlAsync()
    {
        if (_driver != null)
        {
            _driver.Navigate().GoToUrl(gooodsUrl);
            // Сохранение cookies в CookieContainer
            var allCookies = _driver.Manage().Cookies.AllCookies;
            foreach (var cookie in allCookies)
            {
                _cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
        }
    }

    // Инициализация драейвера
    private async Task InitializeDriver(string channelName)
    {
        try
        {
            profiles = await ProfileManager.GetProfiles();

            foreach (Profile profile in profiles)
            {
                if (profile.Name != channelName || profile == null) continue;

                _driver = await adsPower.InitializeDriver(profile.UserId);
            }
        }
        catch (Exception) { }
    }
}
