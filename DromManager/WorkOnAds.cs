using DromAutoTrader.AdsPowerManager;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

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
            OpenAllBulletinsPage(actualUrl);

            // Размер окна
            SetWindowSize();

            int tryCount = 0;

            while (isBulletinExistsOnPage)
            {
                tryCount++;
                if (tryCount == 30) break;
                // Выбираю все элементы
                GetInputSelectAll();

                // Убираю в архив
                RemoveToArchive();

                // Подтеверждаю, что убираю в архив
                SubmitBtn();

                isBulletinExistsOnPage = ExistsElementChecker();
            }

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
                GoSearchByArticul(serachString);

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

        public async Task SetRatesForWatchingAsync(List<string> parts, string rate, string selectedChannel)
        {
            await InitializeDriver(selectedChannel);
            _wait = new(_driver, TimeSpan.FromSeconds(30));


            foreach (var part in parts)
            {
                string searchUrl = BuildSearchString(part);

                GoSearchByArticul(searchUrl);

                SetWindowSize();

                // Получаю чекбокс [выбрать все]
                GetInputSelectAll();

                // Открываю окно с указанием ставки для показов
                OpenRatePageButton();

                // Устанавливаю ставки для выбранных категорий
                SetRates(rate);

                // Подтеверждаю ставки
                SubmitBtn();
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
        public void GoSearchByArticul(string url)
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
        public void SetWindowSize()
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
            try
            {
                IWebElement removeBtn = _wait.Until(e => e.FindElement(By.Name("applier[deleteBulletin]")));
                removeBtn.Click();
            }
            catch (Exception)
            {

            }
        }

        // Открываю окно для выбора ставок для показов
        private void OpenRatePageButton()
        {
            try
            {
                IWebElement removeBtn = _wait.Until(e => e.FindElement(By.Name("applier[ppcBulletin]")));
                removeBtn.Click();
            }
            catch (Exception)
            {

            }
        }

        // Устанавливаю ставки во все поля
        private void SetRates(string rate)
        {
            IList<IWebElement> rateInputs = null!;
            try
            {
                rateInputs = _wait.Until(e => e.FindElements(By.CssSelector("input[id^='rate_input']")));
            }
            catch (Exception) { }

            foreach (var input in rateInputs)
            {
                try
                {
                    input.Clear();
                    Thread.Sleep(100);
                    input.SendKeys(rate);
                    Thread.Sleep(200);
                }
                catch (Exception) { }
            }
        }
              

        // Метод подтверждения удаления
        private void SubmitBtn()
        { //serviceSubmit
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
