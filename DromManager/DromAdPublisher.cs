using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Security.Policy;
using System.Threading;

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

        //public void ClickSubjectField()
        //{
        //    try
        //    {
        //        // Нахождение элемента по имени и клик по нему
        //        IWebElement subjectInput = _driver.FindElement(By.Name("subject"));
        //        subjectInput.Click();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Ошибка при клике на поле 'subject': " + ex.Message);
        //    }
        //}

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

                Thread.Sleep(random.Next(100, 350));  // Добавляем небольшую паузу между вводом каждого символа
            }

            element.Submit();
            Thread.Sleep(random.Next(300, 700));
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
                bulletinTypeVariant.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при клике на элемент 'bulletin-type__variant-title': " + ex.Message);
            }
        }

        // ---------------------- Метод вставки картинок
        public void InsertImage()
        {
            string imagePath = @"C:\Users\FedoTT\Desktop\0321-re.jpg";
            try
            {                
                // Найти элемент <input type="file>
                IWebElement fileInput = _driver.FindElement(By.Name("up[]"));                

                // Вставить путь к изображению в элемент
                fileInput.SendKeys(imagePath);
              
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вставке изображения: " + ex.Message);
            }           
        }
        // ----------------------


        // Метод публицкации объявления
        public void ClickPublishButton()
        {
            try
            {
                // Нахождение и клик по элементу по ID
                IWebElement bulletinPublicationFree = _driver.FindElement(By.Id("bulletin_publication_free"));
                bulletinPublicationFree.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при клике на кнопку 'Опубликовать': " + ex.Message);
            }
        }

        public void PublishAd(string adTitle)
        {
            OpenGoodsPage();
            SetWindowSize();
           
            //ClickSubjectField();
            TitleInput(adTitle);
            PressEnterKey();
            ClickDirControlVariant();
            ClickBulletinTypeVariant();
            InsertImage();
            //ClickPublishButton();

        }       
       
    }
}

