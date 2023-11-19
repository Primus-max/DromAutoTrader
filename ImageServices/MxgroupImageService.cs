using Org.BouncyCastle.Asn1.Cms;
using System.IO.Compression;
using System.Text;
using BrotliStream = BrotliSharpLib.BrotliStream;

namespace DromAutoTrader.ImageServices
{
    public class MxgroupImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://new.mxgroup.ru/#login-client";

        protected override string SearchPageUrl => "https://new.mxgroup.ru/b/search/n/";

        protected override string UserName => "krasikov98975rus@gmail.com";

        protected override string Password => "H2A8AMZ757";

        public override string ServiceName => "https://new.mxgroup.ru";
        #endregion

        #region Приватные        
        private readonly string _profilePath = @"C:\SeleniumProfiles\Mxgroup";
        private string _tempProfilePath = string.Empty;
        #endregion

        public MxgroupImageService()
        {

        }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса       
        protected override void GoTo()
        {
            //Task.Run(async () => await GoToAsync()).Wait();
        }



        protected async Task GoToAsync(string url = null!)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обращении к Berg {ex.Message}");
            }
        }




        protected override void Authorization()
        {
            string apiUrl = "https://new.mxgroup.ru/b/search/n/SS-3025";
            string accessToken = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpZCI6Ijg4MDJkMjUwNT" +
                "NiYTJkYTU0YzgyMDBlN2U2OWQwZDEwNGRjNjA1NTQiLCJqdGkiOiI4ODAyZDI1MDUzYmEyZGE1NGM4MjAwZT" +
                "dlNjlkMGQxMDRkYzYwNTU0IiwiaXNzIjoiTVggZ3JvdXAiLCJhdWQiOiJvel9mcm9udF9wcm9kIiwic3ViIj" +
                "oiZjc1NTUyMDQtYTEwMi0xMWVkLTkyYmMtMDAxNTVkMDA1NjRjIiwiZXhwIjoxNzAwNDEyMTY5LCJpYXQiOj" +
                "E3MDAzNzYxNjksInRva2VuX3R5cGUiOiJiZWFyZXIiLCJzY29wZSI6ImRlZmF1bHQifQ.tWD03Md9TWZFh6H" +
                "VhMbsrjU0QQaF0NAZ86cEUXVYe1WtTtAzyy9MmhDab3B-EV79rvCJ3ORd0NMLfyezv18ujA"; // Токен авторизации

            using HttpClient client = new HttpClient();
            // Добавление заголовков запроса
            //client.DefaultRequestHeaders.Add("authority", "api.mxgroup.ru");
            //client.DefaultRequestHeaders.Add("method", "GET");
            //client.DefaultRequestHeaders.Add("path", "/client/main");
            //client.DefaultRequestHeaders.Add("scheme", "https");
            //client.DefaultRequestHeaders.Add("accept", "application/json");
            //client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
            //client.DefaultRequestHeaders.Add("accept-language", "ru,en-US;q=0.9,en;q=0.8,ru-RU;q=0.7");
            client.DefaultRequestHeaders.Add("authorization", accessToken); // Заголовок Authorization с токеном
            //client.DefaultRequestHeaders.Add("content-type", "application/json");
            //client.DefaultRequestHeaders.Add("origin", "https://new.mxgroup.ru");
            //client.DefaultRequestHeaders.Add("referer", "https://new.mxgroup.ru/");
            //client.DefaultRequestHeaders.Add("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"102\"");
            //client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            //client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
            //client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            //client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            //client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
            //client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("x-session", "8802d25053ba2da54c8200e7e69d0d104dc60554%3A1700376169%3A1700412169%3A");

            try
            {
                // Выполнение GET-запроса
                HttpResponseMessage response = client.GetAsync(apiUrl).Result; // Внимание: использование Result блокирует выполнение, рекомендуется использовать async/await в асинхронном методе

                // Проверка успешности запроса
                if (response.IsSuccessStatusCode)
                {
                    // Чтение ответа
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    
                }
                else
                {
                    Console.WriteLine($"Ошибка при запросе: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }


        // Декодирую ответ от сервера
        static string DecompressBrotli(string compressedData)
        {
            byte[] compressedBytes = Encoding.UTF8.GetBytes(compressedData);

            using (var compressedStream = new MemoryStream(compressedBytes))
            using (var brotliStream = new BrotliStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(brotliStream))
            {
                return reader.ReadToEnd();
            }
        }


        protected override void SetArticulInSearchInput()
        {
            string? searchUrl = BuildUrl();

            _driver.Navigate().GoToUrl(searchUrl);


            // Проверка на наличие спиннеров свидетельствующих о загрузке страницы
            bool isSpinner = true;
            while (isSpinner)
            {
                Thread.Sleep(500);
                try
                {
                    IWebElement spinner1 = _driver.FindElement(By.CssSelector(".icon.spinner.mx-spin"));

                    try
                    {
                        IWebElement spinner12 = _driver.FindElement(By.CssSelector(".indicator-wrapper.disabled"));
                        break;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                catch (Exception)
                {
                    isSpinner = false;
                }
            }
        }

        protected override bool IsNotMatchingArticul()
        {
            bool isNotMatchingArticul = false;
            try
            {
                IWebElement attentionMessage = _driver.FindElement(By.ClassName("mx-text_secondary"));

                string warningTest = attentionMessage.Text;

                if (!string.IsNullOrEmpty(warningTest))
                    return true;

            }
            catch (Exception)
            {
                return isNotMatchingArticul;
            }
            return isNotMatchingArticul;
        }

        protected override void OpenSearchedCard()
        {

        }

        protected override bool IsImagesVisible()
        {
            return true;
        }

        protected override async Task<List<string>> GetImagesAsync()
        {
            // Еще одна проверка на загрузку страницы
            bool isSpinner = true;
            int tryCount = 0;
            while (isSpinner)
            {
                tryCount++;
                if (tryCount == 500) break;
                //class="indicator-wrapper disabled"
                Thread.Sleep(500);
                try
                {
                    IWebElement spinner2 = _driver.FindElement(By.CssSelector(".indicator-wrapper.enabled"));


                    //IWebElement spinner22 = _driver.FindElement(By.CssSelector(".indicator-wrapper.disabled"));
                    continue;
                }
                catch (Exception)
                {
                    break;
                }
            }

            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IWebElement mainImageParentUl = null!;
            IWebElement imgPopup = null!;

            // Если открылась другая таблица, то по другому забираем фото
            string offerLink = _driver.Url;
            if (offerLink.Contains("offers"))
            {
                downloadedImages = await GetImagesFromOtherSourceAsync();

                if (downloadedImages.Count > 0)
                    return downloadedImages;
            }

            // Получаю контейнер с картинками
            try
            {
                mainImageParentUl = _driver.FindElement(By.CssSelector("ul.mx-sr-item__products"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IList<IWebElement>? itemProductsLi = mainImageParentUl?.FindElements(By.TagName("li"));

                IWebElement? itemProductIcon = itemProductsLi[0]?.FindElement(By.ClassName("mx-sr-item__img"));


                if (itemProductIcon is not null)
                {
                    itemProductIcon.Click();
                    await Task.Delay(1000);

                    try
                    {
                        imgPopup = _driver.FindElement(By.CssSelector("div.slider__item.img-viewer__slider-item img"));
                    }
                    catch (Exception)
                    {

                    }

                    string imagePath = imgPopup.GetAttribute("src");
                    images.Add(imagePath);
                }
            }
            catch (Exception)
            {

            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync()
        {
            try
            {
                _driver.Close();
                _driver.Quit();
                _driver.Dispose();

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
        // Метод на случай если появилась другая таблица, в ней другие селекторы
        private async Task<List<string>> GetImagesFromOtherSourceAsync()
        {
            // Список изображений которые возвращаем из метода
            List<string> downloadedImages = new List<string>();

            // Временное хранилище изображений
            List<string> images = new List<string>();
            IList<IWebElement> mainImageParentTable = null!;
            IWebElement imgPopup = null!;

            // Получаю контейнер с картинками
            try
            {
                mainImageParentTable = _driver.FindElements(By.CssSelector("div.sr-table__row.offer.sr-table__row_mx"));
            }
            catch (Exception) { }

            // Получаю все картинки thumbs
            try
            {
                // Находим все img элементы в li элементах с data-type='thumb'
                IWebElement? itemProductsTd = mainImageParentTable[0]?.FindElement(By.CssSelector("div.sr-table__td.td_product"));

                IWebElement? itemProductIcon = itemProductsTd?.FindElement(By.ClassName("mx-sr-item__img"));


                if (itemProductIcon is not null)
                {
                    itemProductIcon.Click();
                    await Task.Delay(1000);

                    try
                    {
                        imgPopup = _driver.FindElement(By.CssSelector("div.slider__item.img-viewer__slider-item img"));
                    }
                    catch (Exception)
                    {

                    }

                    string imagePath = imgPopup.GetAttribute("src");
                    images.Add(imagePath);
                }
            }
            catch (Exception)
            {

            }

            if (images.Count != 0)
                downloadedImages = await ImagesProcessAsync(images);

            return downloadedImages;
        }

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            string url = $"{SearchPageUrl}{Articul}";

            return url;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new(_tempProfilePath);
            _driver = webDriver.GetDriver();
        }

        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            await Task.Delay(1000);

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
