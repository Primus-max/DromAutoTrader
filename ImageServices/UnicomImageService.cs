using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    public class UnicomImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://uniqom.ru/#login";

        protected override string SearchPageUrl => "https://uniqom.ru/search?term=";

        protected override string UserName => "autobest038@gmail.com";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "https://uniqom.ru";
        #endregion

        #region Приватные поля      
       
        private readonly string _profilePath = @"C:\SeleniumProfiles\Unicom";
        private string _tempProfilePath = string.Empty;
        #endregion


        public UnicomImageService()
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
            catch (Exception) { }
        }

        protected override void Authorization()
        {           
        }        

        protected override void SetArticulInSearchInput()
        {
            try
            {
                string searchUrl = BuildUrl();

                _driver.Navigate().GoToUrl(searchUrl);
            }
            catch (Exception) { }
        }

        // Метод проверяет если ничего не найдено
        protected override bool IsNotMatchingArticul()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(3));
            try
            {
                IWebElement attentionMessage = wait.Until(e => e.FindElement(By.CssSelector("h4.not-found__title")));

                // Если получили этот элемент значит по запросу ничего не найдено
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        // Открываю карточку с изображениями (в данном классе реализация не требуется)
        protected override void OpenSearchedCard() { }

        // Метод проверки наличия изображения для дальнейшего получения
        protected override bool IsImagesVisible()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5));
            try
            {
                // Получаем элемент-родитель если арткул найден
                var divElement = wait.Until(e => e.FindElements(By.CssSelector("div.product__card")))[0];
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
        protected override async Task<List<string>> GetImagesAsync()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5));
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new();

            // Временное хранилище изображений
            List<string> images = new();

            try
            {
                // Получаем изображение
                // Получаем элемент-родитель если арткул найден
                var divElement = wait.Until(e => e.FindElements(By.CssSelector("div.product__card")))[0];

                // Теперь, используя родительский элемент, получить ссылку на изображение
                var imageUrlElement = divElement.FindElement(By.ClassName("feip-productsList-photoCell-image"));

                string imgUrl = imageUrlElement.GetAttribute("href");

                if (!string.IsNullOrEmpty(imgUrl))
                    images.Add(imgUrl);

            }
            catch (Exception)
            {
                CloseDriver();
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

            Thread.Sleep(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        // Строю ссылку
        public string BuildUrl()
        {
            var uri = new Uri(SearchPageUrl);

            // Построение нового URL с обновленными параметрами
            string newUrl = uri + Articul;

            return newUrl;
        }

        protected override void CloseDriver()
        {
            try
            {
                _driver.Close();

                // Удаляю временную директорию профиля после закрытия браузера
                Directory.Delete(_tempProfilePath, true);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Специфичные методы класса   
        protected bool WaitReadyStatePage()
        {
            bool isModalWinOpen = true;
            while (isModalWinOpen)
            {
                // f-modal__container
                try
                {
                    IWebElement modalWin = _driver.FindElement(By.CssSelector("div.f-modal__container"));

                    return isModalWinOpen;
                }
                catch (Exception)
                {
                    isModalWinOpen = false;
                }
            }
            return isModalWinOpen;
        }


        public void ClearAndEnterText(IWebElement element, string text)
        {
            Random random = new Random();
            // Используем JavaScriptExecutor для выполнения JavaScript-кода
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)((IWrapsDriver)element).WrappedDriver;

            // Очищаем поле ввода с помощью JavaScript
            jsExecutor.ExecuteScript("arguments[0].value = '';", element);
            // Установить стиль display элемента в block
            jsExecutor.ExecuteScript("arguments[0].style.display = 'block';", element);
            // Вставляем текст по одному символу без изменений
            foreach (char letter in text)
            {
                if (letter == '\b')
                {
                    // Если символ является символом backspace, удаляем последний введенный символ
                    element.SendKeys(Keys.Backspace);
                }
                else
                {
                    // Вводим символ
                    element.SendKeys(letter.ToString());
                }

                Thread.Sleep(random.Next(50, 100));  // Добавляем небольшую паузу между вводом каждого символа
            }
        }

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
            UndetectDriver webDriver = new(_profilePath);
            _driver = webDriver.GetDriver();
        }
        #endregion
    }
}
