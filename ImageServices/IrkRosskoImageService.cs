using AngleSharp.Html.Dom;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    public class IrkRosskoImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://irk.rossko.ru";

        protected override string SearchPageUrl => "https://irk.rossko.ru/search";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "irk.rossko.ru";
        #endregion

        #region Приватные поля
        private IHtmlDocument _document = null!;
        #endregion

        public IrkRosskoImageService() { InitializeDriver(); }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //

        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(LoginPageUrl);
        }

        protected override void Authorization() { }

        protected override void SetArticulInSearchInput()
        {
            IWebElement searchInput = null!;

            while (true)
            {
                Thread.Sleep(2000);
                try
                {

                    searchInput = _driver.FindElement(By.Id("1"));

                    searchInput.SendKeys(Articul);

                    searchInput.Submit();

                    break;

                }
                catch (Exception)
                {
                    continue;
                }
            }

        }

        protected override bool IsNotMatchingArticul()
        {
            bool isMatching = false;
            try
            {
                Thread.Sleep(500);
                IWebElement? wrongMessageElement = (_driver?.FindElement(By.CssSelector("div.src-components-SearchNotFound-___styles__notFoundTitle___IWpeT")));

                string? wrongMessage = wrongMessageElement?.Text;

                string? cleanedText = Regex.Unescape(wrongMessage.Trim().Replace("\n", "").Replace("\r", ""));
                string? comparisonStr = $"Ничего не нашлось";

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
                IWebElement searchCardLink = _driver.FindElement(By.CssSelector("div.src-features-search-components-result-item-___index__link___q1afC.src-features-search-components-result-item-___index__isLinkFocused___-+EPf"));

                Thread.Sleep(200);
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].click();", searchCardLink);
            }
            catch (Exception)
            {

            }
        }

        protected override bool IsImagesVisible()
        {
            Thread.Sleep(500);
            return true;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы 
                IWebElement imagesThumb = _driver.FindElement(By.ClassName("src-features-product-card-components-info-___index__image___KeiQL"));

                string imagePath = imagesThumb.GetAttribute("style");

                string pattern = @"url\(""(https://[^""]+)""\);";

                Match match = Regex.Match(imagePath, pattern);
                if (match.Success)
                {
                    string imageUrl = match.Groups[1].Value;

                    images.Add(imageUrl);
                }


            }
            catch (Exception) { }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        protected override void SpecificRunAsync(string brandName, string articul) { }
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

            Thread.Sleep(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();
        }
        #endregion

    }
}
