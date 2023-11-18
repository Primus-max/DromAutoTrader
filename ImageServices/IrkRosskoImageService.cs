using AngleSharp.Html.Dom;
using System.IO.Compression;
using System.Text;

namespace DromAutoTrader.ImageServices
{
    public class IrkRosskoImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://irk.rossko.ru";

        protected override string SearchPageUrl => "https://irk.rossko.ru/search";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "https://irk.rossko.ru";
        #endregion

        #region Приватные поля
        private string _response = string.Empty;
        #endregion

        public IrkRosskoImageService()        {                  }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //

        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }

        protected async Task GoToAsync(string url = null!)
        {
            string searchApiUrl = $"https://searchform.rossko.ru/api/Search/Autocomplete?searchString={Articul}";

            // Создаем экземпляр HttpClientHandler, чтобы управлять куками
            var handler = new HttpClientHandler();
            handler.CookieContainer = new System.Net.CookieContainer();

            using HttpClient client = new HttpClient(handler);
            try
            {
                // Устанавливаем заголовки
                client.DefaultRequestHeaders.Add("accept", "*/*");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("access-control-allow-origin", "*");
                client.DefaultRequestHeaders.Add("authorization-domain", "https://irk.rossko.ru");
                client.DefaultRequestHeaders.Add("authorization-session", "m-nxPNCgzTDN9WngzVAgfRAdj2B3m2mtvHCh");
                client.DefaultRequestHeaders.Add("cookie", "_ym_d=1699948795; _ym_uid=1699948795417199734; _ym_isad=2; _ym_visorc=b");
                client.DefaultRequestHeaders.Add("source", "frontend");

                // Отправка GET-запроса
                HttpResponseMessage response = await client.GetAsync(searchApiUrl);

                // Проверка успешности запроса
                if (response.IsSuccessStatusCode)
                {
                    // Получение содержимого ответа в виде строки
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Парсим JSON для извлечения ссылки
                    var searchResults = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SearchResult>>(responseBody);

                    // Проверяем, что есть результаты
                    if (searchResults.Count > 0)
                    {
                        string goodsCode = searchResults[0].goodsCode;

                        // Строим URL для второго запроса
                        string productApiUrl = $"https://productcard.rossko.ru/api/Product/Card/{goodsCode}?CurrencyCode=643&uin=&tariffTimings=true&newCart=true&addressGuid=&deliveryType=000000001&newClaimSystem=true";

                        // Второй запрос по полученной ссылке
                        HttpResponseMessage productResponse = await client.GetAsync(productApiUrl);

                        if (productResponse.IsSuccessStatusCode)
                        {
                            string productResponseBody = await productResponse.Content.ReadAsStringAsync();
                            _response = await DecodeGzip(productResponse.Content); // Докодирую ответ                            
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка второго запроса: {productResponse.StatusCode} - {productResponse.ReasonPhrase}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет результатов поиска");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка первого запроса: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Класс для десериализации ответа от сервера
        public class SearchResult
        {
            public string brandName { get; set; }
            public string article { get; set; }
            public string goodsName { get; set; }
            public string goodsLink { get; set; }
            public string goodsCode { get; set; }
        }

        // Декодирую ответ от сервера
        private async Task<string> DecodeGzip(HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzip, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }











        protected override void Authorization() { }

        protected override void SetArticulInSearchInput()
        {

        }

        protected override bool IsNotMatchingArticul()
        {
            return true;
        }

        protected override void OpenSearchedCard()
        {

        }

        protected override bool IsImagesVisible()
        {
            return true;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            //// Временное хранилище изображений
            //List<string> images = new List<string>();

            //// Устанавливаю ожидание
            //WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5));

            //// Получаю все картинки thumbs
            //try
            //{
            //    // Находим div элемент с изображением, исключая тот, у которого есть подпись
            //    IWebElement imageDiv = wait.Until(e => e.FindElement(By.XPath("//div[contains(@class,'src-features-product-card-components-info-___index__image___KeiQL') and not(contains(., 'Посмотреть на Яндекс Картинках'))]")));
            //    string imagePath = imageDiv.GetAttribute("style");

            //    // Находим позиции, где начинается URL и заканчивается
            //    int startIndex = imagePath.IndexOf("https://");
            //    int endIndex = imagePath.LastIndexOf("jpg") + 3;

            //    if (startIndex >= 0 && endIndex > startIndex)
            //    {
            //        string imageUrl = imagePath.Substring(startIndex, endIndex - startIndex);

            //        if (!string.IsNullOrEmpty(imageUrl))
            //            images.Add(imageUrl);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //string asdf = ex.Message;
            //}

            //if (images.Count != 0)
            //    downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync()
        {

        }

        #endregion

        #region Специфичные методы класса 

        // TODO вынести этот метод в базовый и сделать для всех
        // Метод создания директории и скачивания изображений
        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            await Task.Delay(500);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }


        #endregion

    }
}
