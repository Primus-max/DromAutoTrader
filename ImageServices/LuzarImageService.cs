using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    public class LuzarImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://luzar.ru/";

        protected override string SearchPageUrl => "https://luzar.ru/catalogue/?q=";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "https://luzar.ru";
        #endregion

        #region Приватные поля
        private IHtmlDocument _document = null!;
        #endregion

        public LuzarImageService() { }


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
                IHtmlElement wrongMessageElement = _document?.QuerySelector("h1.page__h1.page-title__heading.page-title__heading--fullwidth") as IHtmlElement;

                string? wrongMessage = wrongMessageElement.Text();


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
            try
            {
                // Находим div с классом "module-spisok-product"
                var productDiv = _document.QuerySelector(".module-spisok-product");

                if (productDiv != null)
                {
                    // Находим первый элемент li внутри div
                    var firstLi = productDiv.QuerySelector("li");

                    if (firstLi != null)
                    {
                        // Извлекаем ссылку из тега a
                        var linkElement = firstLi.QuerySelector("a");
                        if (linkElement != null)
                        {
                            string? link = linkElement.GetAttribute("href");

                            Task.Run(async () => await GoToAsync(link)).Wait();
                            // Здесь можно осуществить переход по полученной ссылке и продолжить парсинг следующей страницы
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected override bool IsImagesVisible()
        {           
            return true;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new();

            // Временное хранилище изображений
            List<string> images = new();

            using HttpClient httpClient = new();

            try
            {
               await Task.Delay(500);
                // Получаем изображение

                var imageBlocks = _document.QuerySelectorAll(".product-card-image-list-item");

                List<string> imageUrls = new List<string>();

                foreach (var imageBlock in imageBlocks)
                {
                    // Находим изображение внутри блока
                    var imgElement = imageBlock.QuerySelector("img");
                    if (imgElement != null)
                    {
                        string? imgUrl = imgElement.GetAttribute("data-big-image");
                        httpClient.BaseAddress = new Uri(LoginPageUrl);
                        string? fullUrl = new Uri(httpClient.BaseAddress, imgUrl).AbsoluteUri;

                        images.Add(fullUrl);
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
                    fullUrl = $"{SearchPageUrl}{Articul}";
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

            Thread.Sleep(1000);

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
