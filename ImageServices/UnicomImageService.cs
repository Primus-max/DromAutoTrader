using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    public class UnicomImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://uniqom.ru/#login";

        protected override string SearchPageUrl => "https://uniqom.ru";

        protected override string UserName => "autobest038@gmail.com";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "uniqom.ru";
        #endregion

        #region Приватный поля
        //private bool _isFirstRunning = true;
        public string? _imagesLocalPath = string.Empty;
        protected IWebDriver _driver = null!;        
        #endregion

        #region Публичные поля        
        //protected string? Brand { get; set; }
        //public string? Articul { get; set; }
        //public List<string>? BrandImages { get; set; }
        #endregion

        public UnicomImageService()
        {
            InitializeDriver();
        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        protected override void SpecificRunAsync(string brandName, string articul)
        {
            throw new NotImplementedException();
        }

        // Метод перехода по ссылке
        protected override void GoTo()
        {
            string asd = Brand;
            string dasd = Articul;
            //_brand = brand;
            //_articul = articul;

            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(LoginPageUrl);
        }

        protected override void Authorization()
        {

            try
            {
                // Поле для ввода логина
                IWebElement usernameElement = _driver.FindElement(By.Name("username"));
                Thread.Sleep(200);
                // Ввести логин
                usernameElement.SendKeys(UserName);
            }
            catch (Exception) { }

            try
            {
                // Поле для ввода пароля
                IWebElement passwordElement = _driver.FindElement(By.Name("password"));
                Thread.Sleep(200);
                // Ввести пароль
                passwordElement.SendKeys(Password);
            }
            catch (Exception) { }

            try
            {
                // Кнопка для входа и нажать на нее
                IWebElement loginButton = _driver.FindElement(By.CssSelector(".login__button"));
                loginButton.Click();
            }
            catch (Exception) { }
        }

        protected override void SetArticulInSearchInput()
        {
            try
            {
                // Ожидание загурзки страницы
                WaitReadyStatePage();

                Thread.Sleep(1000);
                // Находим поле для ввода
                IWebElement searchField = _driver.FindElement(By.Id("m-header-search-l"));

                // Используем JavaScript, чтобы очистить поле
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", searchField);

                // Вводим новый текст
                searchField.SendKeys(Articul);

                // Нажимаем Enter
                searchField.SendKeys(Keys.Enter);

            }
            catch (Exception) { }
        }

        // Метод проверяет если ничего не найдено
        protected override bool IsNotMatchingArticul()
        {
            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.ClassName("not-found__title"));

                // Если получили этот элемент значит по запросу ничего не найдено
                return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
        }

        // Открываю карточку с изображениями (в данном классе реализация не требуется)
        protected override void OpenSearchedCard() { }

        // Метод проверки наличия изображения для дальнейшего получения
        protected override bool IsImagesVisible()
        {
            try
            {
                Thread.Sleep(300);
                // Получаем элемент-родитель если арткул найден
                IWebElement divElement = _driver.FindElement(By.XPath("//h1[contains(text(),'Искомый товар')]/ancestor::div[not(@class)]"));
                // Получаем в элементе-родителе этот элемент
                IWebElement pictureNotFounfDiv = divElement.FindElement(By.ClassName("picture_not-found"));

                // Если элемент получили, значит картинки нет и значит не получаем
                return false;
            }
            catch (Exception)
            {
                // Если элемент не получили, значит картинка есть и мы можем её получать
                return true;
            }
        }

        // Метод получения изображений
        protected override async Task<List<string>> GetImages()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new();

            // Временное хранилище изображений
            List<string> images = new();

            try
            {
                Thread.Sleep(200);
                // Получаем изображение
                // Найти элемент h1 с текстом "Искомый товар"
                var h1Element = _driver.FindElement(By.XPath("//h1[contains(text(),'Искомый товар')]"));

                // Затем получить родительский элемент (div)
                var parentDiv = h1Element.FindElement(By.XPath("./parent::div"));

                // Теперь, используя родительский элемент, получить ссылку на изображение
                var imageUrlElement = parentDiv.FindElement(By.ClassName("feip-productsList-photoCell-image"));

                string imgUrl = imageUrlElement.GetAttribute("href");

                if (!string.IsNullOrEmpty(imgUrl))
                    images.Add(imgUrl);

            }
            catch (Exception) { }


            // TODO Отрефакторить и вынести в отдельный метод и в базовый класс

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            Thread.Sleep(1000);
            // Скачиваю изображения
            ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
            downloadedImages = await downloader.DownloadImagesAsync();

            return downloadedImages;
        }
        #endregion

        #region Специфичные методы класса   
        

        void SelectProduct()
        {
            // Нажать на элемент, представляющий выбранный продукт
            IWebElement productElement = _driver.FindElement(By.CssSelector("div:nth-child(1) > div > .product__card > .card__section .svg-inline--fa"));
            productElement.Click();
        }

        void OpenProductImage()
        {
            // Нажать на элемент, представляющий открытие изображения продукта
            IWebElement imageElement = _driver.FindElement(By.CssSelector(".uk-lightbox-toolbar-icon line:nth-child(2)"));
            imageElement.Click();
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
