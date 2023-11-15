
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Diagnostics.CodeAnalysis;

namespace DromAutoTrader.ImageServices
{
    /// <summary>
    /// Класс для получения изображений деталей брэндов с сайта https://berg.ru/ 
    /// </summary>
    internal class BergImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://berg.ru/login";

        protected override string SearchPageUrl => "https://berg.ru/search?search=";

        protected override string UserName => "autobest038";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "https://berg.ru";
        #endregion

        #region Приватный поля        
        private readonly string _profilePath = @"C:\SeleniumProfiles\Berg";
        private string _tempProfilePath = string.Empty;
        private IHtmlDocument _document = null!;
        #endregion        

        public BergImageService()
        {
            // Создаю временную копию профиля (на эту сессию)
            //ProfilePathService profilePathService = new();
            //_tempProfilePath = profilePathService.CreateTempProfile(_profilePath);

            //InitializeDriver();
        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса  

        // Метод перехода по ссылке
        protected override void GoTo()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }

        // Метод авторизации
        protected override void Authorization() { }

        // Метод отправки поискового запроса
        protected override void SetArticulInSearchInput() { }

        // Метод открытия каротчки с полученным запросом
        protected override void OpenSearchedCard()
        {
            try
            {
                // Получаю все карточки на странице
                var cards = _document?.QuerySelectorAll("div.search_result__row");

                if (cards != null)
                {
                    foreach (var card in cards)
                    {
                        // Получаю бренд и артикул из карточек
                        string brand = card.QuerySelector(".brand_name")?.TextContent.ToLower().Replace(" ", "") ?? "";
                        string articul = card.QuerySelector("div.article a")?.TextContent ?? "";
                        string globalNameBrand = Brand.ToLower().Replace(" ", "");

                        // Если совпадает, открываю Popup
                        if (globalNameBrand == brand && Articul == articul)
                        {
                            var popUp = card.QuerySelector("a.pseudo_link.part_description__link");
                            // Получаю ссылку на Popup
                            string popupUrl = popUp?.GetAttribute("href") ?? "";
                            string popUpLink = $"{ServiceName}{popupUrl}";

                            try
                            {
                                // Переходим по ссылке, используя метод GoToAsync                                
                                Task.Run(async () => await GoToAsync(popupUrl)).Wait();
                            }
                            catch (Exception)
                            {
                                // Обработка исключения, например, логирование
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }


        // Метод проверки, появились картинки или нет
        protected override bool IsImagesVisible()
        {
            try
            {
                var photoGallery = _document?.QuerySelector(".photo_gallery");

                // Если получили этот элемент, значит, изображения видны
                return photoGallery != null;
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
                return false;
            }
        }

        // Метод проверки результатов поиска детали
        protected override bool IsNotMatchingArticul()
        {
            try
            {
                var attentionMessage = _document?.QuerySelector(".attention_message");

                // Если получили этот элемент, значит, по запросу ничего не найдено
                return attentionMessage != null;
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
                return false;
            }
        }


        // Метод сбора картинок из открытой карточки
        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений, которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IElement mainImageParentDiv = null;

            // Получаем контейнер с картинками
            try
            {
                mainImageParentDiv = _document?.QuerySelector(".photo_gallery");
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }

            // Получаем все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                var imagesThumb = mainImageParentDiv?.QuerySelectorAll("li[data-type='thumb'] img");

                if (imagesThumb != null)
                {
                    foreach (var image in imagesThumb)
                    {
                        string relativePath = image.GetAttribute("data-src");

                        // Формируем абсолютный URL
                        Uri baseUri = new Uri(ServiceName);
                        Uri fullUri = new Uri(baseUri, relativePath);
                        string fullUrl = fullUri.AbsoluteUri;

                        images.Add(fullUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }


        // Метод создания директории и скачивания изображений
        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            await Task.Delay(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync()
        {
           
        }
        #endregion

        #region Специфичные методы класса    
        // Асинхронный метод получения DOM
        protected async Task GoToAsync(string url = null!)
        {
            try
            {
                // Создаем объект HttpClient
                using var httpClient = new HttpClient();
                var searchUrl = string.Empty;

                if (url != null)
                {
                    // Если передали ссылку, строим полный адрес
                    var uri = new Uri(new Uri(ServiceName), url);
                    searchUrl = uri.AbsoluteUri;
                }
                else
                {
                    searchUrl = BuildUrl(); // Получаем ссылку для поиска
                }

                // Создаем контейнер для хранения кук
                var cookieContainer = new System.Net.CookieContainer();

                // Устанавливаем куки (замените значения на актуальные)
                cookieContainer.Add(new Uri(ServiceName), new System.Net.Cookie("BERG_SESSID", "b39b60eadfac0d2fc91d7f8acf45687e"));

                // Создаем объект HttpClientHandler и передаем ему контейнер с куками
                var handler = new HttpClientHandler { CookieContainer = cookieContainer };

                // Создаем новый HttpClient с настроенным обработчиком
                using var httpClientWithCookies = new HttpClient(handler);
                // Отправляем GET-запрос и получаем ответ
                var response = await httpClientWithCookies.GetAsync(searchUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Получаем документ
                    var pageSource = await response.Content.ReadAsStringAsync();
                    var context = BrowsingContext.New(Configuration.Default);
                    var parser = context.GetService<IHtmlParser>();
                    _document = parser?.ParseDocument(pageSource);
                }
                else
                {
                    Console.WriteLine($"Не удалось получить DOM для Berg {response.Headers}");
                    // TODO: сделать логирование
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обращении к Berg {ex.Message}");
            }
        }

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            // Построение нового URL с обновленными параметрами
            string newUrl = SearchPageUrl + Articul;

            return newUrl;
        }
        #endregion
    }
}
