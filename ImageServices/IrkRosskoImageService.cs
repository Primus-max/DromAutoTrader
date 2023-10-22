using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

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

        protected override void Authorization()
        {

        }

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
            throw new NotImplementedException();
        }

        protected override bool IsImagesVisible()
        {
            throw new NotImplementedException();
        }

        protected override Task<List<string>> GetImagesAsync()
        {
            throw new NotImplementedException();
        }

        protected override void SpecificRunAsync(string brandName, string articul)
        {
            throw new NotImplementedException();
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

            Thread.Sleep(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        // Строю строку для запроса
        public string BuildUrl()
        {
            var uri = new Uri(SearchPageUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["search"] = Articul;
            query["brand"] = Brand;

            // Построение нового URL с обновленными параметрами
            string newUrl = uri.GetLeftPart(UriPartial.Path) + "?" + query.ToString();

            return newUrl;
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
