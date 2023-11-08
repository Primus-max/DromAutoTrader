using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using System.IO;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    internal class TmpartsImageService : ImageServiceBase
    {

        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://tmparts.ru/";

        protected override string SearchPageUrl => "https://tmparts.ru/StaticContent/ProductImages/";

        protected override string UserName => "ПЛ0044166";

        protected override string Password => "G97GP?ct4";

        public override string ServiceName => "https://tmparts.ru";
        #endregion

        #region Приватные
        private readonly string _profilePath = @"C:\SeleniumProfiles\Tmparts";
        private string _tempProfilePath = string.Empty;
        #endregion

        public TmpartsImageService()
        {

            // Создаю временную копию профиля (на эту сессию)
            ProfilePathService profilePathService = new();
            _tempProfilePath = profilePathService.CreateTempPath(_profilePath);

            InitializeDriver();
        }



        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            try
            {
                _driver.Manage().Window.Maximize();
            }
            catch (Exception)
            {

            }
        }

        protected override void Authorization()
        {

        }

        protected override void SetArticulInSearchInput()
        {
            // TODO везде обернуть эти места в try catch           
            try
            {
                string? searchUrl = BuildUrl();
                _driver.Navigate().GoToUrl(searchUrl);
            }
            catch (Exception)
            {
                Thread.Sleep(5000);
            }
            Thread.Sleep(1500);
        }

        protected override bool IsNotMatchingArticul()
        {
            return false;
        }

        protected override void OpenSearchedCard()
        {

        }

        protected override bool IsImagesVisible()
        {
            Thread.Sleep(700);
            bool isImagesVisible = false;
            try
            {
                IWebElement documentBody = _driver.FindElement(By.TagName("body"));
                IWebElement img = documentBody.FindElement(By.TagName("img"));
                isImagesVisible = true;
                return isImagesVisible;
            }
            catch (Exception)
            {
                // Обработка исключения, если элементы не найдены
            }
            return isImagesVisible;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IWebElement mainImageParentDiv = null!;


            // Получаю контейнер с картинками
            try
            {
                mainImageParentDiv = _driver.FindElement(By.TagName("body"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                IWebElement img = mainImageParentDiv.FindElement(By.TagName("img"));

                if (img != null)
                {
                    string imagePath = img.GetAttribute("src");
                    //string fullPath = LoginPageUrl + imagePath;

                    images.Add(imagePath);
                }


            }
            catch (Exception) { }

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

            Thread.Sleep(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImageWithCookiesAsync(_driver);
            }
            return downloadedImages;
        }

        protected override void CloseDriver()
        {
            _driver.Close();

            // Удаляю временную директорию профиля после закрытия браузера
            Directory.Delete(_tempProfilePath, true);
        }
        #endregion

        #region Специфичные методы класса   
        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            string buildetLink = $"{SearchPageUrl}{Brand}/{Articul}.jpg";

            return buildetLink;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new(_tempProfilePath);
            _driver = webDriver.GetDriver();
        }
        #endregion
    }
}
