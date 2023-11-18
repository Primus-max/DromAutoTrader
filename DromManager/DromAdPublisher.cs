using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private CookieContainer _cookieContainer;
        private readonly HttpClient _httpClient;

        public DromAdPublisher(string channelName)
        {
            _channelName = channelName;
            adsPower = new BrowserManager();

            _cookieContainer = new CookieContainer(); // Инициализация CookieContainer
            _httpClient = new HttpClient(new HttpClientHandler { CookieContainer = _cookieContainer });
            _httpClient.BaseAddress = new Uri("https://baza.drom.ru/api/1.0/save/bulletin");

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

            if (adPublishingInfo.PriceBuy == "2")
            {
                // Здесь логика удаления в архив или просто изменение цены

            }


            OpenGoodsPage(); // Открываю страницу через Selenium
            CloseAllTabsExceptCurrent(); // Закрываю все вкладки кроме текущей
            SetCookies(); // Получаю/сохраняю куки 


            await UploadImagesAsync(adPublishingInfo);
            // Dictionary<string, object> adDictionary = ObjectToDictionary(adPublishingInfo);

            return isPublited;
        }

        // Метод открытия страницы с размещением объявления, нужна только для получения авторизованных кук
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

        // Метод получения кук и сохранения
        private void SetCookies()
        {
            // Получение куки из Selenium драйвера
            var cookies = _driver.Manage().Cookies.AllCookies;

            // Сохранение кук в CookieContainer
            foreach (var cookie in cookies)
            {
                _cookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
        }

        // Метод загрузки объявления
        public async Task<bool> UploadToDrom(AdPublishingInfo ad)
        {
            List<string> images = ad.ImagesPath.Split(";").ToList();

            var data = new
            {
                addingType = "bulletin",
                fields = GetData(ad),
                directoryId = "14",
                images = new
                {
                    isShowCompanyLogo = false,
                    images
                }
            };

            if (ad.DromId != null)
            {
                // Если есть DromId, то обновляем, иначе создаем
                data.id = ad.DromId;
            }

            var requestData = new { changeDescription = Newtonsoft.Json.JsonConvert.SerializeObject(data) };

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("data", requestData.ToString())
        });

            var response = await _client.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                ad.SetDromId(responseContent.id);

                if (responseContent.isDraft == true)
                {
                    Publish(ad);
                }
                return true;
            }
            return false;
        }


        // Метод загрузки изображений на сервер
        public async Task<List<string>> UploadImagesAsync(AdPublishingInfo ad)
        {
            List<string> imageIds = new();

            List<string> imagesPathsFromDb = new() { @"C:\Users\FedoTT\Desktop\plugin.jpg" };

            foreach (var imagePath in imagesPathsFromDb)
            {
                using (var formData = new MultipartFormDataContent())
                {
                    // Добавление изображения к данным формы
                    formData.Add(new ByteArrayContent(File.ReadAllBytes(imagePath)), "up[]", Path.GetFileName(imagePath));

                    // Отправка данных на сервер
                    var response = await _httpClient.PostAsync("https://baza.drom.ru/upload-image-jquery", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Обработка успешного ответа, например, получение идентификатора изображения
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var imageId = ParseImageIdFromResponse(responseContent);
                        if (!string.IsNullOrEmpty(imageId))
                        {
                            imageIds.Add(imageId);
                        }
                    }
                    else
                    {
                        // Обработка ошибки, например, вывод содержимого ответа
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error uploading image: {errorContent}");
                    }
                }
            }

            return imageIds;
        }

        private string ParseImageIdFromResponse(string responseContent)
        {
            // Реализуйте логику извлечения идентификатора из содержимого ответа сервера
            // Вероятно, вам нужно будет использовать регулярные выражения или другие методы обработки данных
            // Пример: предположим, что responseContent содержит JSON с полем "id"
            var responseJson = JObject.Parse(responseContent);
            return responseJson["id"]?.ToString();
        }


        public Dictionary<string, object> GetData(AdPublishingInfo ad)
        {
            if (ad is null) return new Dictionary<string, object>();
            var data = new Dictionary<string, object>();

            data["goodPresentState"] = "present";
            // data["autoPartsAuthenticity"] = "original"; // закомментировано, так как не используется
            data["condition"] = "new";

            var contacts = new Contacts();
            //data["contacts"] = contacts.GetObjectValues(ad);

            data["subject"] = ad.AdDescription;
            data["text"] = ad.AdDescription;

            // var price = new Price(ad.PriceSell, "RUB");
            data["price"] = ad.OutputPrice;

            data["manufacturer"] = ad.Brand;
            data["autoPartsOemNumber"] = ad.Artikul;
            data["cityId"] = 340; // Иркутск

            // var pickupAddress = new PickupAddress();
            // data["pickupAddress"] = pickupAddress.GetObjectValues(ad);

            var delivery = new Delivery();
            data["delivery"] = delivery.GetObjectValues();
            data["delivery.comment"] = "Наша компания осуществляет доставку по городу Иркутск совершенно бесплатно, оплата производится после получения товара на руки!, Оплата наличными,, безналичный расчет, или переводом на карту СБЕР ВТБ";
            data["guarantee"] = "Принимаем товар в оригинальной упаковке и без следов установки к обмену и возврату в течение 7 дней со дня покупки";

            var result = new Dictionary<string, object>
    {
        { "changeDescription", JsonConvert.SerializeObject(data) }
    };

            return result;
        }

        public class Price
        {
            public decimal Count { get; set; }
            public string Valute { get; set; }

            public Price(decimal count, string valute)
            {
                Count = count;
                Valute = valute;
            }

            public object[] GetObjectValues()
            {
                return new object[] { Count, Valute };
            }
        }

        public class Delivery
        {
            public string LocalPrice { get; set; } = "free";
            public string MinPostalPrice { get; set; } = "free";
            public string SelfDeliveryStatus { get; set; } = "on";
            public object PostProviderWeight { get; set; }
            public object PostProviderPrice { get; set; }

            public Dictionary<string, object> GetObjectValues()
            {
                return new Dictionary<string, object>
            {
            { "localPrice", LocalPrice },
            { "minPostalPrice", MinPostalPrice },
            { "selfDeliveryStatus", SelfDeliveryStatus },
            { "postProviderWeight", PostProviderWeight },
            { "postProviderPrice", PostProviderPrice }
        };
            }
        }

        //public class PickupAddress
        //{
        //    public string Address { get; set; } = "";
        //    public Dictionary<string, object> Coordinates { get; set; } = new Dictionary<string, object>();

        //    public Dictionary<string, object> GetObjectValues(AdPublishingInfo ad)
        //    {
        //        var channel = ad.Channel;
        //        Coordinates["latitude"] = channel.Latitude;
        //        Coordinates["longitude"] = channel.Longitude;

        //        return new Dictionary<string, object>
        //{
        //    { "address", channel.Address },
        //    { "coordinages", Coordinates }
        //};
        //    }
        //}

        public class Contacts
        {
            public string ContactInfo { get; set; } = "";
            public string Email { get; set; } = "";
            public bool IsEmailHidden { get; set; } = false;

            Dictionary<string, string> channelPhoneNumbers = new()
            {
                { "DoctorCar38", "79149057076" },
                {"AutoBot38", "79500777698" }
            };

            Dictionary<string, string> channelEmails = new()
            {
                {"DoctorCar38", "doctorcar38@mail.ru" },
                {"AutoBot38", "krasikov98975rus@gmail.com" }
            };

            public Dictionary<string, object> GetObjectValues(AdPublishingInfo ad)
            {
                if(ad is null) return new Dictionary<string, object>();

                return new Dictionary<string, object>
                {
                    { "contactInfo", channelPhoneNumbers[ad?.AdDescription] },
                    { "email", channelEmails[ad.AdDescription] },
                    { "is_email_hidden", IsEmailHidden }
                };
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
            catch (Exception) { }
        }
    }
}

