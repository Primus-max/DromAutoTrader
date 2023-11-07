using DromAutoTrader.AdsPowerManager;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Specialized;
using System.Threading;
using System.Web;
using System;

namespace DromAutoTrader.DromManager
{
    public class WorkOnAds
    {
        private string gooodsUrl = new("https://baza.drom.ru/adding?type=goods");
        private string archivedUrl = new("https://baza.drom.ru/personal/archived/bulletins");
        private string actualUrl = new("https://baza.drom.ru/personal/actual/bulletins");
        private string allUrl = new("https://baza.drom.ru/personal/all/bulletins");
        private string searchUrl = new("https://baza.drom.ru/personal/actual/bulletins?find=");

        private string partSearchLink = new("https://baza.drom.ru/personal/actual/bulletins?find=");

        private IWebDriver _driver = null!;
        private BrowserManager adsPower = null!;
        private WebDriverWait _wait = null!;

        public WorkOnAds()
        {

        }

        /// <summary>
        /// Метод для перемещения объявлений в архив всех объявлений
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public async Task RemoveAll(string channelName)
        {
            await InitializeDriver(channelName);
            _wait = new(_driver, TimeSpan.FromSeconds(30));

            bool isBulletinExistsOnPage = true;

            // Открываю страницу  с объявлениями
            OpenAllBulletinsPage(actualUrl); // Открываю странциу
            CloseAllTabsExceptCurrent(); // Закрываю все вкладки кроме текущей            
            SetWindowSize(); // Размер окна

            bool isPaginationExists = false; // Есть или нет пагинация на странице
            string lastPageUrl = string.Empty; // Запоминаю страницу перед переходом для подтверждения ставок
            int countRemoved = 0;

            do
            {
                isPaginationExists = HasNextPage();

                //string currentUrl = _driver.Url;
                //if (currentUrl.Contains("https://baza.drom.ru/personal/actual/bulletins?System.Collections.Specialized.NameValueCollection"))
                //{
                //    try
                //    {
                //        OpenAllBulletinsPage(lastPageUrl);
                //    }
                //    catch (Exception) { }
                //}

                // Запоминаю последнюю страницу перед переходом в карточку ставок
                lastPageUrl = _driver.Url;
                // Получаю чекбокс [выбрать все]
                GetInputSelectAll();

                //// Убираю в архив
                RemoveToArchive();

                // Подтеверждаю ставки
                SubmitBtn();

                if (isPaginationExists == true)
                    NextPage(lastPageUrl); // Пагинация  

                countRemoved++;
            } while (isPaginationExists);


            CloseDriver();
            MessageBox.Show($"Все объявления успешно перемещены в архив, всего {countRemoved}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Метод перемещения объявлений в архив по одному
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="adPublishings"></param>
        /// <returns></returns>
        public async Task RemoveByFlag(string channelName, List<AdPublishingInfo> adPublishings)
        {
            await InitializeDriver(channelName);
            _wait = new(_driver, TimeSpan.FromSeconds(30));

            foreach (var ads in adPublishings)
            {
                if (ads.Artikul == null || ads.Brand == null) continue;
                if (ads.IsArchived == false) continue;

                // Формирую поисковую строку
                string serachString = BuildSearchString(ads.Artikul);

                // Перехожу на поисковую страницу по артикулу
                GoSearchByParamString(serachString);

                // Размер окна
                SetWindowSize();

                // Выбираю все элементы
                GetInputSelectAll();

                // Убираю в архив
                RemoveToArchive();

                // Подтеверждаю, что убираю в архив
                SubmitBtn();
            }

        }

        /// <summary>
        /// Метод приклеивания ставок для показов объявлений
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="rate"></param>
        /// <param name="selectedChannel"></param>
        /// <returns></returns>
        public async Task SetRatesForWatchingAsync(List<string> parts, string rate, string selectedChannel)
        {
            await InitializeDriver(selectedChannel);

            Dictionary<string, int> countSetRates = new();
            int countRates = 0;
            int waitingTime = 15;
            _wait = new(_driver, TimeSpan.FromSeconds(waitingTime));
            bool isPaginationExists = false; // Есть или нет пагинация на странице
            string lastPageUrl = string.Empty; // Запоминаю страницу перед переходом для подтверждения ставок           

            foreach (var part in parts)
            {
                string searchUrl = BuildSearchString(part);
                GoSearchByParamString(searchUrl);
                SetWindowSize();
                CloseAllTabsExceptCurrent();

                do
                {
                    if (!IsPartExists())
                    {
                        MessageBox.Show($"Проверьте правильность написания запчасти,  {part.ToUpper()} не найдена", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    isPaginationExists = HasNextPage();

                    // Запоминаю последнюю страницу перед переходом в карточку ставок
                    lastPageUrl = _driver.Url;
                    // Получаю чекбокс [выбрать все]
                    GetInputSelectAll();

                    // Открываю окно с указанием ставки для показов
                    OpenRatePageButton();

                    // Устанавливаю ставки для выбранных категорий
                    countRates += SetRates(rate);

                    // Подтеверждаю ставки
                    SubmitBtn();

                    if (isPaginationExists == true)
                        NextPage(lastPageUrl); // Пагинация

                } while (isPaginationExists);

                countSetRates[part] = countRates;
            }

            CloseDriver();

            // Готовлю информацию для информировании о завршении
            string resultMessage = "Ставки приклеены:\n";
            foreach (var entry in countSetRates)
            {
                resultMessage += $"{entry.Key}: {entry.Value} ставок\n";
            }

            // Отображаем MessageBox с информацией
            MessageBox.Show(resultMessage, "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Метод проверки того, что запчасть найдена
        private bool IsPartExists()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5));
            try
            {
                IWebElement notFoundEl = wait.Until(e => e.FindElement(By.XPath("//form[@class='personalBullsListForm']//p")));

                string notFoundElText = notFoundEl.Text;
                if (notFoundElText.Contains("Объявления не найдены."))
                    return false;
            }
            catch (Exception)
            {
                return true;
            }
            return true;
        }

        // Закрываю драейвер
        private void CloseDriver()
        {
            try
            {
                _driver.Close();
            }
            catch (Exception) { }
        }

        // Переход на следующую страницу
        private void NextPage(string lastUrl)
        {
            Thread.Sleep(500);
            try
            {
                Uri uri = new Uri(lastUrl);
                string nextPageUrl = lastUrl;

                string queryPage = "page";

                if (uri.Query.Contains(queryPage))
                {
                    NameValueCollection queryParameters = HttpUtility.ParseQueryString(uri.Query);
                    string[] currentPageValues = queryParameters.GetValues(queryPage);

                    if (currentPageValues != null && currentPageValues.Length > 0 && int.TryParse(currentPageValues[0], out int currentPage))
                    {
                        int nextPage = currentPage + 1;
                        queryParameters[queryPage] = nextPage.ToString();

                        UriBuilder uriBuilder = new UriBuilder(uri)
                        {
                            Query = queryParameters.ToString()
                        };

                        nextPageUrl = uriBuilder.Uri.ToString();
                    }
                }
                else
                {
                    // Если параметра "page" нет в URL, добавляем его со значением "2"
                    nextPageUrl = lastUrl + "&page=2";
                }

                // Переходим на следующую страницу
                try
                {
                    _driver.Navigate().GoToUrl(nextPageUrl);
                }
                catch (Exception) { }
            }
            catch (Exception)
            {
                // Обработка ошибки
            }
        }


        // Метод открытия актуальных объявлений
        private void OpenAllBulletinsPage(string url)
        {
            try
            {
                // Открытие URL
                _driver.Navigate().GoToUrl(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при открытии веб-сайта: " + ex.Message);
            }
        }

        // Открываю страница с актуальными объявлениями
        private void GoSearchByParamString(string url)
        {
            try
            {
                // Открытие URL
                _driver.Navigate().GoToUrl(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при открытии веб-сайта: " + ex.Message);
            }
        }

        // Метод установки размера экрана
        private void SetWindowSize()
        {
            try
            {
                // Установка размера окна браузера
                _driver.Manage().Window.Maximize();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при установке размера окна: " + ex.Message);
            }
        }

        // Строю поисковую строку с артикулом
        private string BuildSearchString(string searchParam)
        {
            return searchUrl + searchParam;
        }

        // Метод получения чекбокса (выбрать все) 
        private void GetInputSelectAll()
        {
            try
            {
                IWebElement selectAllInput = _wait.Until(e => e.FindElement(By.CssSelector("input.personal-select__select-all-control")));

                selectAllInput.Click();
            }
            catch (Exception)
            {

            }
        }

        // Метод переноса в архив
        private void RemoveToArchive()
        {
            bool isClickedRemoveBtn = true;
            while (isClickedRemoveBtn)
            {
                Thread.Sleep(500);
                try
                {
                    IWebElement removeBtn = _wait.Until(e => e.FindElement(By.Name("applier[deleteBulletin]")));
                    removeBtn.Click();

                    isClickedRemoveBtn = false;
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        // Открываю окно для выбора ставок для показов
        private void OpenRatePageButton()
        {
            bool isNotRetesOpened = true;
            int tryCount = 0;

            while (isNotRetesOpened)
            {
                tryCount++;
                if (tryCount == 20) break;

                Thread.Sleep(200);
                try
                {
                    IWebElement removeBtn = _wait.Until(e => e.FindElement(By.Name("applier[ppcBulletin]")));
                    removeBtn.Click();

                    isNotRetesOpened = false;
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        // Устанавливаю ставки во все поля
        private int SetRates(string rate)
        {
            Thread.Sleep(1000);
            int countRates = 0;

            IList<IWebElement> rateInputs = null!;
            try
            {
                rateInputs = _wait.Until(e => e.FindElements(By.CssSelector("input[id^='rate_input']")));
            }
            catch (Exception) { }

            foreach (var input in rateInputs)
            {
                countRates++;
                try
                {
                    input.Clear();
                    Thread.Sleep(100);
                    input.SendKeys(rate);
                    Thread.Sleep(200);
                }
                catch (Exception) { }
            }

            return countRates;
        }

        // Метод подтверждения удаления
        private void SubmitBtn()
        {
            Thread.Sleep(200);
            try
            {
                IWebElement submitRemoveBtn = _wait.Until(e => e.FindElement(By.Id("serviceSubmit")));

                submitRemoveBtn.Click();
            }
            catch (Exception)
            {
            }
        }

        private bool ExistsElementChecker()
        {

            try
            {
                IWebElement bullList = _wait.Until(e => e.FindElement(By.CssSelector("div.bull-item-content")));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Проверяю наличие пагинации на странице
        private bool HasNextPage()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5)); // Отдельное ожидание от общего

            try
            {
                IWebElement pagination = wait.Until(e => e.FindElement(By.ClassName("nextnumber")));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Закрываю все кладки кроме текущей
        private void CloseAllTabsExceptCurrent()
        {
            string currentWindowHandle = _driver.CurrentWindowHandle;

            foreach (string windowHandle in _driver.WindowHandles)
            {
                if (windowHandle != currentWindowHandle)
                {
                    _driver.SwitchTo().Window(windowHandle);
                    _driver.Close();
                }
                Thread.Sleep(200);
            }

            // Вернуться на исходную вкладку
            _driver.SwitchTo().Window(currentWindowHandle);
        }

        // Инициализация драйвера
        private async Task InitializeDriver(string channelName)
        {
            BrowserManager adsPower = new();
            List<Profile> profiles = await ProfileManager.GetProfiles();

            foreach (Profile profile in profiles)
            {
                if (profile.Name != channelName || profile == null) continue;

                _driver = await adsPower.InitializeDriver(profile.UserId);
            }

        }
    }
}
