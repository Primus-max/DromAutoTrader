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
    public class LynxautoImageService : ImageServiceBase
    {

        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://lynxauto.info/";

        protected override string SearchPageUrl => "https://lynxauto.info/index.php?route=product/category/search";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "https://lynxauto.info";
        #endregion

        #region Приватные поля
        private IHtmlDocument _document = null!;
        #endregion

        public LynxautoImageService() { }

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

        // Метод проверки есть ли данных по артикулу
        protected override bool IsNotMatchingArticul()
        {
            bool isMatching = false;
            try
            {
                Thread.Sleep(500);
                IHtmlElement wrongMessageElement = _document?.QuerySelector(".content-container.min-width") as IHtmlElement;

                string wrongMessage = wrongMessageElement.Text();


                string cleanedText = Regex.Unescape(wrongMessage.Trim().Replace("\n", "").Replace("\r", ""));
                string comparisonStr = $"По вашему запросу \"{Articul}\" нет информации";

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

        // Метод проверки наличия сообщения, что ничего не найдено
        protected override bool IsImagesVisible()
        {
            Thread.Sleep(500);
            return true;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new();

            // Временное хранилище изображений
            List<string> images = new();

            try
            {
                Thread.Sleep(500);
                // Получаем изображение
                var linkElement = _document.QuerySelector("a.lightbox");

                string imgUrl = linkElement?.GetAttribute("href");

                if (!string.IsNullOrEmpty(imgUrl))
                    images.Add(imgUrl);

            }
            catch (Exception)
            {
                return downloadedImages;
            }


            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }


        protected override void SpecificRunAsync(string brandName, string articul)
        {
            // TODO если метод не пригодился, убрать
        }

        protected override void CloseDriver()
        {
            
        }
        #endregion


        #region Специфичные методы класса 
        // Асинхронный метода перехода на страницу поиска и поиск
        protected async Task GoToAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                var fullUrl = $"{SearchPageUrl}&keyword={Articul}&search_type=right"; // Строим URL с параметрами
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
                    var strasding = response.ReasonPhrase;
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
