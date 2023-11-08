using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Threading;

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
        #endregion        

        public BergImageService()
        {
            InitializeDriver();

            // Создаю временную копию профиля (на эту сессию)
            ProfilePathService profilePathService = new();
            _tempProfilePath = profilePathService.CreateTempPath(_profilePath);
        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса  

        // Метод перехода по ссылке
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

        // Метод авторизации
        protected override void Authorization()
        {

        }

        // Метод отправки поискового запроса
        protected override void SetArticulInSearchInput()
        {
            string? searchUrl = BuildUrl();

            try
            {
                _driver.Navigate().GoToUrl(searchUrl);
            }
            catch (Exception)
            {

            }
        }

        // Метод открытия каротчки с полученным запросом
        protected override void OpenSearchedCard()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(7));
            try
            {
                // Получаю все карточки на странице
                IList<IWebElement> searchedCards = wait.Until(e => e.FindElements(By.CssSelector("div.search_result__row")));

                foreach (var card in searchedCards)
                {
                    // Получаю бренд и артикул из карточек
                    string brand = card.FindElement(By.ClassName("brand_name")).Text.ToLower().Replace(" ", "");
                    string articul = card.FindElement(By.XPath("//div[@class='article']/a[1]")).Text;
                    string? globalNameBrand = Brand.ToLower().Replace(" ", "");

                    // Если совпадает открываю Popup
                    if (globalNameBrand == brand && Articul == articul)
                    {
                        IWebElement popUp = card.FindElement(By.CssSelector("a.pseudo_link.part_description__link"));
                        // Получаю ссылку на Popup
                        string popupUrl = popUp.GetAttribute("href");
                        string popUpLink = $"{ServiceName}{popupUrl}";

                        try
                        {
                            _driver.Navigate().GoToUrl(popupUrl);
                        }
                        catch (Exception) { }

                    }
                }

            }
            catch (Exception)
            {

            }
        }

        // Метод проверки, появились картинки или нет
        protected override bool IsImagesVisible()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(3));

            try
            {
                IWebElement mainImageParentDiv = wait.Until(e => e.FindElement(By.ClassName("photo_gallery")));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Метод проверки результатов поиска детали
        protected override bool IsNotMatchingArticul()
        {
            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.ClassName("attention_message"));

                // Если получили этот элемент значит по запросу ничего не найдено
                return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
        }

        // Метод сбора картинок из открытой карточки
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
                mainImageParentDiv = _driver.FindElement(By.ClassName("photo_gallery"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IList<IWebElement> imagesThumb = mainImageParentDiv.FindElements(By.XPath("//li[@data-type='thumb']/img"));

                foreach (var image in imagesThumb)
                {
                    string imagePath = image.GetAttribute("src");
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
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        protected override void CloseDriver()
        {
            try
            {
                _driver.Close();

                // Удаляю временную директорию профиля после закрытия браузера
                Directory.Delete(_tempProfilePath, true);

            }
            catch (Exception)
            {

            }
        }
        #endregion

        #region Специфичные методы класса       

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            // Построение нового URL с обновленными параметрами
            string newUrl = SearchPageUrl + Articul;

            return newUrl;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new(_profilePath);
            _driver = webDriver.GetDriver();
        }

        #endregion
    }
}
