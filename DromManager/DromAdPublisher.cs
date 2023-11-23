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
            _wait = new(_driver, TimeSpan.FromSeconds(10));





            using var context = new AppContext();
            var adInfos = context.AdPublishingInfo.ToList();

            foreach (var adInfo in adInfos)
            {
                if (adInfo.DromeId == null || adInfo.DromeId == 0)
                {
                    string url = $"https://baza.drom.ru/personal/actual/bulletins?find={adInfo.Artikul}";
                    try
                    {
                        _driver.Navigate().GoToUrl(url);
                    }
                    catch (Exception)
                    {
                    }

                    int idElmet = ExtractId();

                    adInfo.DromeId = idElmet;

                    try
                    {
                        context.Update(adInfo);
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {

                    }
                }
            }













            // Если установлен этот флаг, значит нужно обновить объявление. Убираем в архив, добавляем новое
            if (adPublishingInfo.PriceBuy == "2")
            {
                isPublited = await EditAdInfo(adPublishingInfo);
                return isPublited; // Выхожу после редактирования. Дальше нам не надо
            }

            await Task.Delay(200);
            OpenGoodsPage();

            await Task.Delay(200);
            SetWindowSize();

            await Task.Delay(200);
            CloseAllTabsExceptCurrent();
            await Task.Delay(200);
            // Устанавливаю заголовок объявления
            TitleInput(adPublishingInfo.KatalogName);
            await Task.Delay(1000);

            ClickDirControlVariant();
            await Task.Delay(200);
            ClickBulletinTypeVariant();
            List<string> ImagesPaths = adPublishingInfo.ImagesPath.Split(";").ToList();

            // Вставляю изображение
            foreach (var imagePath in ImagesPaths)
            {
                await Task.Delay(200);
                string absolutePath = Path.Combine(Environment.CurrentDirectory, imagePath);
                InsertImage(absolutePath);
            }

            await Task.Delay(200);
            // Бренд для публикации
            BrandInput(adPublishingInfo?.Brand);

            await Task.Delay(200);
            // Артикул для публикации
            ArticulInput(adPublishingInfo?.Artikul);
            await Task.Delay(200);
            // Цена для публикации
            PriceInput(adPublishingInfo?.OutputPrice?.ToString());
            await Task.Delay(200);
            DescriptionTextInput(adPublishingInfo?.Description);
            await Task.Delay(200);
            // Кнопка наличие или под заказ
            GoodPresentState();
            await Task.Delay(200);
            // Проверяю заполненность полей
            CheckAndFillRequiredFields();

            // Публикую
            await Task.Delay(200);

            isPublited = ClickPublishButton();

            // Если объявление разместил, то записываю Id
            if (isPublited)
                WriteDromeId(adPublishingInfo);

            return isPublited;
        }




        public int ExtractId()
        {
            try
            {
                var findedElement = _driver.FindElements(By.CssSelector("div.bull-item.bull-item_inline"))[0];
                var checkbox = findedElement.FindElement(By.CssSelector("input[type='checkbox']"));

                // Получаем значение атрибута "name" из элемента checkbox
                var nameAttributeValue = checkbox.GetAttribute("name");

                // Используем регулярное выражение для извлечения числовой части из строки
                var match = Regex.Match(nameAttributeValue, @"\[(\d+)\]");

                if (match.Success)
                {
                    // Преобразуем значение, найденное в регулярном выражении, в int
                    return int.Parse(match.Groups[1].Value);
                }
                else
                {
                    // Если не удалось найти значение, возвращаем 0 или выбрасываем исключение
                    throw new InvalidOperationException("Не удалось извлечь значение id.");
                }
            }
            catch (Exception)
            {
                // Обработка исключений
                throw;
            }
        }















        // Объединяющтй метод для редактирования
        private async Task<bool> EditAdInfo(AdPublishingInfo adPublishingInfo)
        {
            bool isPublited = false;

            OpenEditePage(adPublishingInfo);

            await Task.Delay(200);
            CloseAllTabsExceptCurrent();

            await Task.Delay(200);
            // Цена для публикации
            PriceInput(adPublishingInfo?.OutputPrice?.ToString());

            // Проверяю заполненность полей
            CheckAndFillRequiredFields();

            isPublited = ClickPublishButton();

            // Если объявление разместил, то записываю Id
            if (isPublited)
            {
                WriteDromeId(adPublishingInfo);
            }

            return isPublited;
        }

        // Метод записи DromeId
        private void WriteDromeId(AdPublishingInfo adPublishingInfo)
        {
            using var context = new AppContext();
            var existingAdInfo = context.AdPublishingInfo.Find(adPublishingInfo.Id);

            string postedAdUrl = _driver.Url;
            int dromeId = GetDromeId(postedAdUrl);
            adPublishingInfo.DromeId = dromeId;

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        // Открываю ссылку для редактирования объявления
        private void OpenEditePage(AdPublishingInfo adPublishing)
        {
            string editeUrl = "";
            string fullUrl = editeUrl + adPublishing.DromeId.ToString();

            try
            {
                _driver.Navigate().GoToUrl(fullUrl);
            }
            catch (Exception)
            {
            }
        }

        // Получаю ID из ссылки
        private int GetDromeId(string url)
        {
            // Используем регулярное выражение для поиска числовых значений после последнего "/"
            Match match = Regex.Match(url, @"/(\d+).html");

            if (match.Success)
            {
                // Преобразуем найденное значение в int
                if (int.TryParse(match.Groups[1].Value, out int id))
                {
                    return id;
                }
            }

            // Если не удалось извлечь ID, возвращаем значение по умолчанию (например, -1)
            return -1;
        }

        // Метод открытия страницы с размещением объявления
        public void OpenGoodsPage()
        {
            try
            {
                // Открытие URL
                _driver.Navigate().GoToUrl(gooodsUrl);
            }
            catch (Exception)
            {
                //MessageBox.Show($"ОШибка {ex.ToString()} в методе OpenGoodsPage");
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
            {
                //MessageBox.Show("Ошибка при вводе текста в поле 'subject': " + ex.Message);
            }
        }

        // Метод открытия разделов
        public void ClickDirControlVariant()
        {
            try
            {
                // Нахождение и клик по элементу по CSS селектору
                IWebElement dirControlVariant = _wait.Until(e => e.FindElement(By.CssSelector("label.button.dir_control__variant[data-action='accept-dir'][data-value='14']")));
                ScrollToElement(dirControlVariant);

                dirControlVariant.Click();
            }
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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

                    brandNameInput.Clear();
                    brandNameInput.SendKeys(brandName);
                    // Вставить путь к изображению в элемент
                    //ClearAndEnterText(brandNameInput, brandName);
                }

            }
            catch (Exception)
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

                    articulNameInput.Clear();
                    articulNameInput.SendKeys(articulName);
                    //ClearAndEnterText(articulNameInput, articulName);
                }

            }
            catch (Exception)
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

                priceInput.Clear();
                priceInput.SendKeys(price);
                //ClearAndEnterText(priceInput, price);

            }
            catch (Exception)
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
            catch (Exception)
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


                descriptionTextInput.Clear();
                descriptionTextInput.SendKeys(description);
                //ClearAndEnterText(descriptionTextInput, description);
            }
            catch (Exception)
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
            catch (Exception)
            {
                //MessageBox.Show("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод публицкации объявления
        public bool ClickPublishButton()
        {
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(5));
            try
            {
                // Нахождение и клик по элементу по ID
                IWebElement bulletinPublicationFree = wait.Until(e => e.FindElement(By.Id("bulletin_publication_free")));

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
        //private static void ClearAndEnterText(IWebElement element, string text)
        //{
        //    Random random = new Random();

        //    element.Clear(); // Очищаем элемент перед вводом текста

        //    foreach (char letter in text)
        //    {
        //        if (letter == '\b')
        //        {
        //            // Если символ является символом backspace, удаляем следующий символ
        //            element.SendKeys(Keys.Delete);
        //        }
        //        else
        //        {
        //            // Вводим символ
        //            element.SendKeys(letter.ToString());
        //        }

        //        Thread.Sleep(random.Next(10, 20));  // Добавляем небольшую паузу между вводом каждого символа
        //    }

        //    element.Submit();
        //    Thread.Sleep(random.Next(100, 200));
        //}

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
            catch (Exception) { }
        }
    }
}

