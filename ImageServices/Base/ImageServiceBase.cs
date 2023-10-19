using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DromAutoTrader.ImageServices.Base
{
    public abstract class ImageServiceBase : IImageService
    {
        #region Переопределяемые свойства
        protected abstract string LoginPageUrl { get; }
        protected abstract string SearchPageUrl { get; }
        protected abstract string UserName { get; }
        protected abstract string Password { get; }
        public abstract string ServiceName { get; }
        #endregion

        #region Непереопределяемые поля
        protected bool _isFirstRunning = true;
        protected string _imagesLocalPath = string.Empty;
        protected IWebDriver _driver = null!;
        
        #endregion

        #region Публичные поля
        protected string? Brand { get; set; }
        protected string? Articul { get; set; }
        public List<string>? BrandImages { get; set; }
        #endregion

        public ImageServiceBase()
        {
            BrandImages = new List<string>();
        }

        #region Общие методы для наследников
        public async Task RunAsync(string brandName, string articul)
        {            
            Brand = brandName;
            Articul = articul;
            

            if (_isFirstRunning)
            {
                _isFirstRunning = false;

                GoTo();

                // Закрывает окно с предложением получения уведомлений
                Thread.Sleep(500);
                //ClosePermissionRequestPopup();

                Authorization();
            }

            SetArticulInSearchInput();

            if (IsNotMatchingArticul())
                return;

            OpenSearchedCard();

            if (IsImagesVisible())
            {
                BrandImages = await GetImages();
            }
            else
            {
                BrandImages = null;
            }
        }

        // Метод вставик текста с предварительной очисткой инпута (настраивается рандомная задержка для эмитации поведения человека)
        public void ClearAndEnterText(IWebElement element, string text)
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

        // Метод ожидания полной загрузки страницы
        protected void WaitReadyStatePage()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            WebDriverWait wait = new(_driver, TimeSpan.FromSeconds(30));

            // Ожидаем, пока загрузится страница
            wait.Until(driver => (bool)js.ExecuteScript("return document.readyState == 'complete'"));
        }

        #endregion


        protected abstract void GoTo();
        protected abstract void Authorization();
        protected abstract void SetArticulInSearchInput();
        protected abstract bool IsNotMatchingArticul();
        protected abstract void OpenSearchedCard();
        protected abstract bool IsImagesVisible();
        protected abstract Task<List<string>> GetImages();
         

        protected abstract void SpecificRunAsync(string brandName, string articul);
    }

}
