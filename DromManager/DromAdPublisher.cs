
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System;

namespace DromAutoTrader.DromManager
{
    public class DromAdPublisher
    {
        private string gooodsUrl = new("https://baza.drom.ru/adding?type=goods");
        private string archivedUrl = new("https://baza.drom.ru/personal/archived/bulletins");
        private IWebDriver _driver;

        public DromAdPublisher(IWebDriver driver)
        {
            // Инициализация драйвера Chrome
            _driver = driver;
        }

        // Метод входная точка
        public void PublishAd(string adTitle)
        {
            OpenGoodsPage();
            SetWindowSize();

            //ClickSubjectField();
            TitleInput(adTitle);
            PressEnterKey();
            ClickDirControlVariant();
            ClickBulletinTypeVariant();
            InsertImage(@"C:\Users\FedoTT\Desktop\0321-re.jpg");

            BrandInput("Brand");
            ArticulInput("Articul");
            PriceInput("3453.8");
            StateBtn();
            DescriptionTextInput("Новые запчасти");
            PresentPartBtn();
            StateBtn();
            ClickPublishButton();
            StateBtn();
            ClickPublishButton();
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
                Console.WriteLine("Ошибка при открытии веб-сайта: " + ex.Message);
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
                Console.WriteLine("Ошибка при прокрутке к элементу: " + ex.Message);
            }
        }

        // Метод получение input заголовка и ввода текста

        public void TitleInput(string text)
        {
            try
            {
                // Ввод текста в поле ввода
                IWebElement subjectInput = _driver.FindElement(By.Name("subject"));


                ClearAndEnterText(subjectInput, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при вводе текста в поле 'subject': " + ex.Message);
            }
        }

        // Метод нажатия Enter
        public void PressEnterKey()
        {
            try
            {
                // Нажатие клавиши Enter
                IWebElement subjectInput = _driver.FindElement(By.Name("subject"));
                subjectInput.SendKeys(Keys.Enter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при нажатии клавиши Enter: " + ex.Message);
            }
        }

        // Метод открытия разделов
        public void ClickDirControlVariant()
        {
            try
            {
                // Нахождение и клик по элементу по CSS селектору
                IWebElement dirControlVariant = _driver.FindElement(By.CssSelector(".dir_control__variant"));

                ScrollToElement(dirControlVariant);

                dirControlVariant.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при клике на элемент 'dir_control__variant': " + ex.Message);
            }
        }

        // Метод выбора раздела
        public void ClickBulletinTypeVariant()
        {
            try
            {
                // Нахождение и клик по элементу по CSS селектору
                IWebElement bulletinTypeVariant = _driver.FindElement(By.CssSelector(".choice-w-caption__variant:nth-child(1) .bulletin-type__variant-title"));

                ScrollToElement(bulletinTypeVariant);

                bulletinTypeVariant.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при клике на элемент 'bulletin-type__variant-title': " + ex.Message);
            }
        }

        // Метод вставки картинок
        public void InsertImage(string imgPath)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement fileInput = _driver.FindElement(By.Name("up[]"));

                ScrollToElement(fileInput);

                // Вставить путь к изображению в элемент
                fileInput.SendKeys(imgPath);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки имени брэнда
        public void BrandInput(string brandName)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement brandNameInput = _driver.FindElement(By.Name("manufacturer"));

                if (string.IsNullOrEmpty(brandNameInput.Text))
                {

                    ScrollToElement(brandNameInput);

                    // Вставить путь к изображению в элемент
                    ClearAndEnterText(brandNameInput, brandName);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки номера 
        public void ArticulInput(string articulName)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement articulNameInput = _driver.FindElement(By.Name("autoPartsOemNumber"));

                if (string.IsNullOrEmpty(articulNameInput.Text))
                {

                    ScrollToElement(articulNameInput);

                    ClearAndEnterText(articulNameInput, articulName);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки цены 
        public void PriceInput(string price)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement priceInput = _driver.FindElement(By.Name("price"));

                ScrollToElement(priceInput);
                ClearAndEnterText(priceInput, price);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения кнопки выбора состояния (новое или б/у)
        public void StateBtn()
        {
            try
            {
                IWebElement stateButton = _driver.FindElement(By.XPath("//label[text()='Новый']"));

                // Выполнить клик на элементе с использованием JavaScript
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
                jsExecutor.ExecuteScript("arguments[0].click();", stateButton);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения инпута и вставки описания 
        public void DescriptionTextInput(string description)
        {
            try
            {
                // Найти элемент <input type="file>
                IWebElement descriptionTextInput = _driver.FindElement(By.Name("text"));

                ScrollToElement(descriptionTextInput);

                ClearAndEnterText(descriptionTextInput, description);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод получения кнопки наличия или под заказ
        public void PresentPartBtn()
        {
            try
            {

                IWebElement presentPartBtn = _driver.FindElement(By.XPath("//label[text()='В наличии']"));

                // Выполнить клик на элементе с использованием JavaScript
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)_driver;
                jsExecutor.ExecuteScript("arguments[0].click();", presentPartBtn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }
        }

        // Метод публицкации объявления
        public void ClickPublishButton()
        {
            try
            {
                // Нахождение и клик по элементу по ID
                IWebElement bulletinPublicationFree = _driver.FindElement(By.Id("bulletin_publication_free"));

                ScrollToElement(bulletinPublicationFree);

                bulletinPublicationFree.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при клике на кнопку 'Опубликовать': " + ex.Message);
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
    }
}

