using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Web;

namespace DromAutoTrader.ImageServices
{
    internal class TmpartsImageService : ImageServiceBase
    {

        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://tmparts.ru/";

        protected override string SearchPageUrl => "https://tmparts.ru/Lookup/SecondLook?brand=Mazda&article=F10049686";

        protected override string UserName => "ПЛ0044166";

        protected override string Password => "G97GP?ct4";

        public override string ServiceName => "tmparts.ru";
        #endregion

        public TmpartsImageService()
        {
            InitializeDriver();
        }



        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            // TODO везде обернуть эти места в try catch
            try
            {
                _driver.Manage().Window.Maximize();
                _driver.Navigate().GoToUrl(LoginPageUrl);
            }
            catch (Exception)
            {

            }
        }

        protected override void Authorization()
        {
            try
            {
                try
                {
                    IWebElement logInput = _driver.FindElement(By.Id("inputEmail4"));
                    Actions builder = new Actions(_driver);

                    builder.MoveToElement(logInput)
                           .Click()
                           .SendKeys(UserName)
                           .Build()
                           .Perform();

                    Thread.Sleep(500);
                }
                catch (Exception) { }

                try
                {
                    IWebElement passInput = _driver.FindElement(By.Id("inputPassword4"));

                    Thread.Sleep(200);

                    passInput.SendKeys(Password);
                }
                catch (Exception) { }

                try
                {
                    IWebElement sumbitBtn = _driver.FindElement(By.CssSelector(".btn.btn-default"));

                    sumbitBtn.Click();

                    Thread.Sleep(200);
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                // TODO сделать логирование
                string message = $"Произошла ошибка в методе Authorization: {ex.Message}";
                Console.WriteLine(message);
            }
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
            // <h4 class="red" style="margin-bottom: 8px;">Извините, артикул не найден!</h4>

            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.CssSelector("h4.red"));

                // Если получили этот элемент значит по запросу ничего не найдено
                return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
        }

        protected override void OpenSearchedCard()
        {
            //try
            //{
            //    IWebElement imgElement = _driver.FindElement(By.XPath("//img[contains(@src, '/Images/i.png')]"));
            //    IWebElement anchorElement = imgElement.FindElement(By.XPath("./..")); // Выбрать родительский элемент <a>

            //    anchorElement.Click();
            //}
            //catch (Exception)
            //{

            //}
            ////https://tmparts.ru/Lookup/SecondLook?brand=Mazda&article=F10049686
            //string secondLookUrl = "https://tmparts.ru/Lookup/SecondLook?brand=Mazda&article=F10049686";

            //var uri = new Uri(secondLookUrl);
            //var query = HttpUtility.ParseQueryString(uri.Query);
            //query["brand"] = Brand;
            //query["article"] = Articul;

            //// Построение нового URL с обновленными параметрами
            //string newUrl = uri.GetLeftPart(UriPartial.Path) + "?" + query.ToString();

            //_driver.Navigate().GoToUrl(secondLookUrl);


            //IWebElement searchedCard = null!;

            //try
            //{
            //    searchedCard = _driver.FindElement(By.CssSelector("div.panelpanel-default"));
            //    IWebElement titleCard = _driver.FindElement(By.CssSelector("h3.panel-title"));

            //    string titleCardText = titleCard.Text;
            //    if (titleCardText.Contains("Полное совпадение запроса "))
            //    {
            //        try
            //        {
            //            IList<IWebElement> searchCardTrs = searchedCard.FindElements(By.CssSelector("tr.tissTooltip"));
            //            IWebElement searchCardLink = searchCardTrs[1];

            //            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            //            js.ExecuteScript("arguments[0].click();", searchCardLink);
            //        }
            //        catch (Exception)
            //        {

            //            throw;
            //        }
            //    }




            //}
            //catch (Exception ex)
            //{
            //    string message = $"Произошла ошибка в методе GetSearchedCard: {ex.Message}";
            //    Console.WriteLine(message);
            //}
        }

        protected override bool IsImagesVisible()
        {
            // id="modalBodyAM"
            bool isImagesVisible = false;
            try
            {
                IWebElement trElement = _driver.FindElement(By.CssSelector("td.tdpart"));
                IWebElement imgElement = trElement.FindElement(By.CssSelector("img[src*='/Images/ph.png']"));


                IWebElement anchorElement = imgElement.FindElement(By.XPath("./..")); // Выбрать родительский элемент <a>

                anchorElement.Click();
                //imgElement.Click();

                isImagesVisible = true;
            }
            catch (Exception)
            {
                // Обработка исключения, если элементы не найдены
            }
            return isImagesVisible;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // //img[@src='/Images/ph.png']
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IWebElement mainImageParentDiv = null!;

           
            // Получаю контейнер с картинками
            try
            {
                mainImageParentDiv = _driver.FindElement(By.CssSelector("div.container_img"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IList<IWebElement> imagesThumb = mainImageParentDiv.FindElements(By.CssSelector("img.infoWinPicBig"));

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

        protected override void SpecificRunAsync(string brandName, string articul)
        {

        }
        #endregion

        #region Специфичные методы класса 
        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            //var uri = new Uri(SearchPageUrl);
            //var query = HttpUtility.ParseQueryString(uri.Query);
            //query["SearchNumber"] = Articul;

            var uri = new Uri(SearchPageUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["brand"] = Brand;
            query["article"] = Articul;

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
