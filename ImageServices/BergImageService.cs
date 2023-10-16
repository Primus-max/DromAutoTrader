using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    /// <summary>
    /// Класс для получения изображений деталей брэндов с сайта https://berg.ru/ 
    /// </summary>
    class BergImageService : IWebsite
    {
        public string WebSiteName => "berg.ru";
        public string? Brand { get; set; }
        public string? Articul { get; set; }
        public List<string>? BrandImages { get; set; }

        private string? _userName = string.Empty;
        private string? _password = string.Empty;

        private IWebDriver _driver = null!;


        public BergImageService(string brandName, string articul)
        {
            Brand = brandName;
            Articul = articul;

            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();
        }

        #region Методы
        public void Authorization(string username, string password)
        {
            try
            {
                IWebElement logInput = _driver.FindElement(By.Id("username"));

                logInput.SendKeys(username);
            }
            catch (Exception) { }

            try
            {
                IWebElement passInput = _driver.FindElement(By.Id("password"));

                passInput.SendKeys(password);
            }
            catch (Exception) { }
        }


        public IWebElement GetSearchInput()
        {
            throw new NotImplementedException();
        }

        public void SetArticulInSearchInput(string articul)
        {
            throw new NotImplementedException();
        }

        // Метод отчистки полей и вставки текста
        private static void ClearAndEnterText(IWebElement element, string text)
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

                Thread.Sleep(random.Next(50, 150));  // Добавляем небольшую паузу между вводом каждого символа
            }
            Thread.Sleep(random.Next(300, 700));
        }
        #endregion

    }
}
