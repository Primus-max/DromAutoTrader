namespace DromAutoTrader.ImageServices
{
    public class StarvoltImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://startvolt.com";

        protected override string SearchPageUrl => "https://startvolt.com/catalogue/";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "https://startvolt.com";
        #endregion

        #region Приватные поля
        private IHtmlDocument _document = null!;
        #endregion

        public StarvoltImageService()
        {

        }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }

        protected override void Authorization() { }

        protected override void SetArticulInSearchInput()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }

        protected override bool IsNotMatchingArticul()
        {
            bool isMatching = false;
            try
            {
                Thread.Sleep(500);
                IHtmlElement? wrongMessageElement = _document?.QuerySelector("h1.page-title__heading.page-title__heading--fullwidth") as IHtmlElement;

                string? wrongMessage = wrongMessageElement?.Text();


                string? cleanedText = Regex.Unescape(wrongMessage.Trim().Replace("\n", "").Replace("\r", ""));
                string? comparisonStr = $"По точному совпадению результатов не найдено";

                if (wrongMessage.Contains(comparisonStr, StringComparison.OrdinalIgnoreCase))
                {
                    isMatching = true;
                }
            }
            catch (Exception)
            {
                return isMatching;
            }
            return isMatching;
        }

        protected override void OpenSearchedCard()
        {

        }


        protected override bool IsImagesVisible()
        {
            try
            {
                // Находим div с классом "catalog__main-products-card"
                var productDiv = _document.QuerySelector(".catalog__main-products-card");

                if (productDiv != null)
                {
                    // Находим первый элемент picture внутри div
                    var pictureElement = productDiv.QuerySelector("picture");

                    if (pictureElement != null)
                    {
                        // Находим тег source внутри picture
                        var sourceElement = pictureElement.QuerySelector("source");

                        if (sourceElement != null)
                        {
                            // Извлекаем относительный путь из атрибута srcset
                            string? relativePath = sourceElement.GetAttribute("srcset");

                            // Преобразуем относительный путь в абсолютный                           
                            string absoluteUrl = new Uri(new Uri(ServiceName), relativePath).ToString();

                            if (!string.IsNullOrEmpty(absoluteUrl))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new();

            // Временное хранилище изображений
            List<string> images = new();

            using HttpClient httpClient = new();

            await Task.Delay(500);

            // Получаем изображение
            try
            {
                // Находим div с классом "catalog__main-products-card"
                var productDiv = _document.QuerySelector(".catalog__main-products-card");

                if (productDiv != null)
                {
                    // Находим первый элемент picture внутри div
                    var pictureElement = productDiv.QuerySelector("picture");

                    if (pictureElement != null)
                    {
                        // Находим тег source внутри picture
                        var sourceElement = pictureElement.QuerySelector("source");

                        if (sourceElement != null)
                        {
                            // Извлекаем относительный путь из атрибута srcset
                            string? relativePath = sourceElement.GetAttribute("srcset");

                            // Преобразуем относительный путь в абсолютный                           
                            string absoluteUrl = new Uri(new Uri(ServiceName), relativePath).ToString();

                            if (!string.IsNullOrEmpty(absoluteUrl))
                            {
                                images.Add(absoluteUrl);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return downloadedImages;
            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        protected override Task CloseDriverAsync()
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Специфичные методы класса
        // Асинхронный метода перехода на страницу поиска и поиск
        protected async Task GoToAsync(string url = null!)
        {
            // TODO Решить проблему с двойным переходом
            try
            {
                using HttpClient httpClient = new();
                string? fullUrl = string.Empty;

                if (url != null)
                {
                    httpClient.BaseAddress = new Uri(LoginPageUrl);
                    fullUrl = new Uri(httpClient.BaseAddress, url).AbsoluteUri;
                }
                else
                {
                    fullUrl = $"{SearchPageUrl}?q={Articul}";
                }

                var response = await httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Получаю документ
                    var pageSource = await response.Content.ReadAsStringAsync();
                    var contextt = BrowsingContext.New(Configuration.Default);
                    var parser = contextt.GetService<IHtmlParser>();
                    _document = parser?.ParseDocument(pageSource);
                }
                else
                {
                    // TODO сделать логирование                    
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

        // TODO вынести этот метод в базовый и сделать для всех
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
        #endregion
    }
}
