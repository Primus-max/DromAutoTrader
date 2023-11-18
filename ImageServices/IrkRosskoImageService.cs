using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumUndetectedChromeDriver;
using System.Diagnostics;
using System.Reflection;
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

        public override string ServiceName => "https://irk.rossko.ru";
        #endregion

        #region Приватные поля
        private readonly string _profilePath = @"C:\SeleniumProfiles\IrkRossko";
        private string _tempProfilePath = string.Empty;
        private IHtmlDocument _document = null!;
        #endregion

        public IrkRosskoImageService()
        {
            // Создаю временную копию профиля (на эту сессию)
            ProfilePathService profilePathService = new();
            _tempProfilePath = profilePathService.CreateTempProfile(_profilePath);

            InitializeDriver();
        }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //

        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }

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













        protected override void Authorization() { }

        protected override void SetArticulInSearchInput()
        {
            WebDriverWait driverWait = new(_driver, TimeSpan.FromSeconds(10));
            IWebElement searchInput = null!;
            try
            {

                searchInput = driverWait.Until(e => e.FindElement(By.Id("1")));
                //searchInput.Clear();

                ClearAndEnterText(searchInput, Articul);
                //Thread.Sleep(200);
                //searchInput.SendKeys(Articul);
                Console.WriteLine($"Ввёл Артикул в инпут");
                searchInput.Submit();
                Console.WriteLine($"Нажал сабмит на инпуте");
            }
            catch (Exception)
            {

            }
        }

        protected override bool IsNotMatchingArticul()
        {
            WebDriverWait driverWait = new(_driver, TimeSpan.FromSeconds(5));
            bool isMatching = false;
            try
            {

                IWebElement? wrongMessageElement = driverWait.Until(e => e.FindElement(By.CssSelector("div.src-components-SearchNotFound-___styles__notFoundTitle___IWpeT")));

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
            WebDriverWait driverWait = new(_driver, TimeSpan.FromSeconds(7));
            try
            {
                IWebElement searchCardLink = driverWait.Until(e => e.FindElement(By.ClassName("src-features-search-components-result-item-___index__isLinkFocused___-+EPf")));


                Console.WriteLine("Открываю карточку");
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].click();", searchCardLink);
                Console.WriteLine("Открыл карточку");

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
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();

            // Устанавливаю ожидание
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5));

            // Получаю все картинки thumbs
            try
            {
                // Находим div элемент с изображением, исключая тот, у которого есть подпись
                IWebElement imageDiv = wait.Until(e => e.FindElement(By.XPath("//div[contains(@class,'src-features-product-card-components-info-___index__image___KeiQL') and not(contains(., 'Посмотреть на Яндекс Картинках'))]")));
                string imagePath = imageDiv.GetAttribute("style");

                // Находим позиции, где начинается URL и заканчивается
                int startIndex = imagePath.IndexOf("https://");
                int endIndex = imagePath.LastIndexOf("jpg") + 3;

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string imageUrl = imagePath.Substring(startIndex, endIndex - startIndex);

                    if (!string.IsNullOrEmpty(imageUrl))
                        images.Add(imageUrl);
                }
            }
            catch (Exception ex)
            {
                //string asdf = ex.Message;
            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync()
        {
            try
            {
                Console.WriteLine("Перед закрытием драйвера");
                _driver.Close();
                _driver.Quit();
                _driver.Dispose();
                Console.WriteLine("После закрытия драйвера");

                // Удаляю временную директорию профиля после закрытия браузера
                ProfilePathService profilePathService = new();
                await profilePathService.DeleteDirectoryAsync(_tempProfilePath);
            }
            catch (Exception)
            {

            }
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

            await Task.Delay(500);

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
            UndetectDriver webDriver = new(_tempProfilePath);
            _driver = webDriver.GetDriver();

            //string driverProcess = GetSessionId();
        }

        private string GetSessionId()
        {
            try
            {
                var browserField = typeof(UndetectedChromeDriver).GetField("_browser", BindingFlags.NonPublic | BindingFlags.Instance);
                var browserInstance = browserField?.GetValue(_driver);

                if (browserInstance != null)
                {
                    var sessionIdField = browserInstance.GetType().GetField("Id", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sessionId = sessionIdField?.GetValue(browserInstance);

                    if (sessionId != null)
                    {
                        return sessionId.ToString();
                    }
                    else
                    {
                        Console.WriteLine("Не удалось получить SessionId");
                    }
                }
                else
                {
                    Console.WriteLine("Не удалось получить BrowserInstance");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при получении SessionId: {ex.Message}");
            }

            return null;
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

                Thread.Sleep(random.Next(10, 50));  // Добавляем небольшую паузу между вводом каждого символа
            }
        }
        #endregion

    }
}
