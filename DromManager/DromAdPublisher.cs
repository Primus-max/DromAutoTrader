using DromAutoTrader.AdsPowerManager;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Threading;

namespace DromAutoTrader.DromManager
{
    /// <summary>
    /// Класс для публикации объявлений на Drom
    /// </summary>
    public class DromAdPublisher
    {
        private string gooodsUrl = new("https://baza.drom.ru/adding?type=goods");
        private string archivedUrl = new("https://baza.drom.ru/personal/archived/bulletins");
        private IWebDriver _driver = null!;
        private BrowserManager adsPower = null!;
        private List<Profile> profiles = null!;
        private WebDriverWait _wait = null!;
        private readonly string? _channelName = null!;

        public DromAdPublisher(string channelName)
        {
            _channelName = channelName;
            adsPower = new BrowserManager();

            // Инициализация драйвера Chrome
            InitializeDriver(channelName).GetAwaiter().GetResult();            
        }

        /// <summary>
        /// Метод точка входа для размещения объявления на Drom
        /// </summary>
        /// <param name="adTitle"></param>
        public async Task<bool> PublishAdAsync(AdPublishingInfo adPublishingInfo)
        {
            bool isPublited = false;
            if (adPublishingInfo == null) return isPublited;

            // Глобально ожидание
            _wait = new(_driver, TimeSpan.FromSeconds(20));


            await Task.Delay(200);
            OpenGoodsPage();
            SetWindowSize();

            CloseAllTabsExceptCurrent();

            //ClickSubjectField();
            // Устанавливаю заголовок объявления
            TitleInput(adPublishingInfo.KatalogName);
            PressEnterKey();

            await Task.Delay(500);
            ClickDirControlVariant();
            ClickBulletinTypeVariant();
            List<string> ImagesPaths = adPublishingInfo.ImagesPath.Split(";").ToList();

            // Вставляю изображение
            foreach (var imagePath in ImagesPaths)
            {
                string absolutePath = Path.Combine(Environment.CurrentDirectory, imagePath);
                InsertImage(absolutePath);
            }

            // Бренд для публикации
            BrandInput(adPublishingInfo?.Brand);


            // Артикул для публикации
            ArticulInput(adPublishingInfo?.Artikul);
            // Цена для публикации
            PriceInput(adPublishingInfo?.OutputPrice?.ToString());
            //Сondition();
            await Task.Delay(500);

            DescriptionTextInput(adPublishingInfo?.Description);

            await Task.Delay(500);
            // Кнопка наличие или под заказ
            GoodPresentState();

            // Проверяю заполненность полей
            CheckAndFillRequiredFields();

           await Task.Delay(500);
            // Публикую
            //ClickPublishButton();

            isPublited = ClickPublishButton();

            //await adsPower.CloseBrowser(_channelName);
            //_driver.Quit();

            return isPublited;
        }

        // Метод открытия страницы с размещением объявления
        public void OpenGoodsPage()
        {
            try
            {
                // Открытие URL
                _driver.Navigate().GoToUrl(gooodsUrl);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"ОШибка {ex.ToString()} в методе OpenGoodsPage");
            }
        }

        // Метод открытия страницы с архивом объявлений
        public void OpenarchivedPage()
        {
            try
            {
                // Открытие URL
                _driver.Navigate().GoToUrl(archivedUrl);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при открытии веб-сайта: " + ex.Message);
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
                //MessageBox.Show("Ошибка при установке размера окна: " + ex.Message);
            }
        }

        // Метод скроллинга
        public void ScrollToElement(IWebElement element)
        {
            try
            {
                // Прокрутка страницы к указанному элементу
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Ошибка при прокрутке к элементу: " + ex.Message);
            }
        }

        // Метод получение input заголовка и ввода текста
        public void TitleInput(string text)
        {
            try
            {
                // Ввод текста в поле ввода
                IWebElement subjectInput = _wait.Until(e => e.FindElement(By.Name("subject")));

                subjectInput.Clear();
                subjectInput.SendKeys(text);   
                //ClearAndEnterText(subjectInput, text);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при вводе текста в поле 'subject': " + ex.Message);
            }
        }

        // Метод нажатия Enter
        public void PressEnterKey()
        {
            try
            {
                // Нажатие клавиши Enter
                IWebElement subjectInput = _wait.Until(e => e.FindElement(By.Name("subject")));               
                subjectInput.SendKeys(Keys.Enter);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при нажатии клавиши Enter: " + ex.Message);
            }
        }

        // Метод открытия разделов
        public void ClickDirControlVariant()
        {
            try
            {
                // Нахождение и клик по элементу по CSS селектору
                IWebElement dirControlVariant = _wait.Until(e => e.FindElement(By.CssSelector(".dir_control__variant")));
                ScrollToElement(dirControlVariant);

                dirControlVariant.Click();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при клике на элемент 'dir_control__variant': " + ex.Message);
            }
        }

        // Метод выбора раздела
        public void ClickBulletinTypeVariant()
        {
            try
            {
                // Нахождение и клик по элементу по CSS селектору
                IWebElement bulletinTypeVariant = _wait.Until(e => e.FindElement(By.CssSelector(".choice-w-caption__variant:nth-child(1) .bulletin-type__variant-title")));
                ScrollToElement(bulletinTypeVariant);

                bulletinTypeVariant.Click();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при клике на элемент 'bulletin-type__variant-title': " + ex.Message);
            }
        }

        // Метод вставки картинок
        public void InsertImage(string imgPath)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement fileInput = _wait.Until(e => e.FindElement(By.Name("up[]")));

                ScrollToElement(fileInput);

                // Вставить путь к изображению в элемент
                fileInput.SendKeys(imgPath);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки имени брэнда
        public void BrandInput(string brandName)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement brandNameInput = _wait.Until(e => e.FindElement(By.Name("manufacturer")));

                if (string.IsNullOrEmpty(brandNameInput.Text))
                {

                    ScrollToElement(brandNameInput);

                    // Вставить путь к изображению в элемент
                    ClearAndEnterText(brandNameInput, brandName);
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки номера 
        public void ArticulInput(string articulName)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement articulNameInput = _wait.Until(e => e.FindElement(By.Name("autoPartsOemNumber")));

                if (string.IsNullOrEmpty(articulNameInput.Text))
                {

                    ScrollToElement(articulNameInput);

                    ClearAndEnterText(articulNameInput, articulName);
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки цены 
        public void PriceInput(string price)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement priceInput = _wait.Until(e => e.FindElement(By.Name("price")));

                ScrollToElement(priceInput);
                ClearAndEnterText(priceInput, price);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения кнопки выбора состояния (новое или б/у)
        public void Сondition()
        {
            try
            {
                IWebElement stateButton = _wait.Until(e => e.FindElement(By.XPath("//label[text()='Новый']")));

                // Выполнить клик на элементе с использованием JavaScript
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
                jsExecutor.ExecuteScript("arguments[0].click();", stateButton);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки описания 
        public void DescriptionTextInput(string description)    
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement descriptionTextInput = _wait.Until(e => e.FindElement(By.Name("text")));

                ScrollToElement(descriptionTextInput);

                ClearAndEnterText(descriptionTextInput, description);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"ОШибка {ex.ToString()} в методе DescriptionTextInput");
            }
        }

        // Метод получения кнопки наличия или под заказ
        public void GoodPresentState()
        {
            try
            {

                IWebElement presentPartBtn = _wait.Until(e => e.FindElement(By.XPath("//label[text()='В наличии']")));

                // Выполнить клик на элементе с использованием JavaScript
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
                jsExecutor.ExecuteScript("arguments[0].click();", presentPartBtn);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод публицкации объявления
        public bool ClickPublishButton()
        {
            try
            {
                // Нахождение и клик по элементу по ID
                IWebElement bulletinPublicationFree = _wait.Until(e => e.FindElement(By.Id("bulletin_publication_free")));

                ScrollToElement(bulletinPublicationFree);


                // Выполнить клик на элементе с использованием JavaScript
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
                jsExecutor.ExecuteScript("arguments[0].click();", bulletinPublicationFree);
                //bulletinPublicationFree.Click();

                return true;
            }
            catch (Exception)
            {
                //TODO добавить логирование
                //MessageBox.Show("Ошибка при клике на кнопку 'Опубликовать': " + ex.Message);
                CheckAndFillRequiredFields(); // Если попали сюда, надо проверить, все ли поля заполнены
                return false;
            }
        }

        // Метод ввода текста в Input, сначала отчищаем, потом вводим
        private static void ClearAndEnterText(IWebElement element, string text)
        {
            Random random = new Random();

            element.Clear(); // Очищаем элемент перед вводом текста

            foreach (char letter in text)
            {
                if (letter == '\b')
                {
                    // Если символ является символом backspace, удаляем следующий символ
                    element.SendKeys(Keys.Delete);
                }
                else
                {
                    // Вводим символ
                    element.SendKeys(letter.ToString());
                }

                Thread.Sleep(random.Next(10, 20));  // Добавляем небольшую паузу между вводом каждого символа
            }

            element.Submit();
            Thread.Sleep(random.Next(100, 200));
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

        // Метод проверки на валидность заполненной формы
        public void CheckAndFillRequiredFields()
        {
            IList<IWebElement> liElements = null!;

            try
            {
                //liElements = _driver.FindElements(By.CssSelector("ul.bulletin_adding__completeness_watcher__fields li"));
                liElements = _wait.Until(e => e.FindElements(By.CssSelector("ul.bulletin_adding__completeness_watcher__fields li")));
            }
            catch (Exception)
            {
                return;
            }

            foreach (IWebElement liElement in liElements)
            {
                string dataRequired = string.Empty;
                string dataName = string.Empty;

                try
                {
                    dataRequired = liElement.GetAttribute("data-required");
                }
                catch (Exception) { }

                try
                {
                    dataName = liElement.GetAttribute("data-name");
                }
                catch (Exception) { }

                if (dataRequired == "1")
                {
                    // Вызываем соответствующий метод заполнения поля на основе data-name
                    switch (dataName)
                    {
                        case "condition":
                            Сondition();
                            Thread.Sleep(500);
                            break;
                        case "goodPresentState":
                            GoodPresentState();
                            Thread.Sleep(500);
                            break;
                            // Добавьте другие case для других полей
                    }
                }
            }

        }

        // Инициализация драйвера       
        private async Task InitializeDriver(string channelName)
        {            
            try
            {                
                List<Profile> profiles = await ProfileManager.GetProfiles();

                foreach (Profile profile in profiles)
                {
                    if (profile.Name != channelName || profile == null) continue;

                    _driver = await adsPower.InitializeDriver(profile.UserId);
                }
            }
            catch(Exception) { }
        }
    }
}

