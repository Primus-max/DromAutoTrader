﻿using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Web;

namespace DromAutoTrader.ImageServices
{
    public class MxgroupImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://new.mxgroup.ru/#login-client";

        protected override string SearchPageUrl => "https://new.mxgroup.ru/b/search/n/";

        protected override string UserName => "krasikov98975rus@gmail.com";

        protected override string Password => "H2A8AMZ757";

        public override string ServiceName => "new.mxgroup.ru";
        #endregion

        public MxgroupImageService()
        {
            InitializeDriver();
        }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса       
        protected override void GoTo()
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(LoginPageUrl);
        }

        protected override void Authorization()
        {
            try
            {
                try
                {
                    IWebElement logInput = _driver.FindElement(By.Name("username"));
                    Actions builder = new Actions(_driver);

                    builder.MoveToElement(logInput)
                           .Click()
                           .SendKeys(UserName)
                           .Build()
                           .Perform();

                    Thread.Sleep(500);
                }
                catch (Exception) { }

                try
                {
                    IWebElement passInput = _driver.FindElement(By.Name("password"));

                    Thread.Sleep(200);

                    passInput.SendKeys(Password);
                }
                catch (Exception) { }

                try
                {
                    IWebElement sumbitBtn = _driver.FindElement(By.ClassName("btn"));

                    sumbitBtn.Click();

                    Thread.Sleep(200);
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                // TODO сделать логирование
                string message = $"Произошла ошибка в методе Authorization: {ex.Message}";
                Console.WriteLine(message);
            }
        }

        protected override void SetArticulInSearchInput()
        {
            throw new NotImplementedException();
        }

        protected override bool IsNotMatchingArticul()
        {
            throw new NotImplementedException();
        }

        protected override void OpenSearchedCard()
        {
            throw new NotImplementedException();
        }

        protected override bool IsImagesVisible()
        {
            throw new NotImplementedException();
        }

        protected override Task<List<string>> GetImagesAsync()
        {
            throw new NotImplementedException();
        }
        protected override void SpecificRunAsync(string brandName, string articul)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Специфичные методы класса       

        // Метод для формирования Url поискового запроса
        public string BuildUrl()
        {
            var uri = new Uri(SearchPageUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["search"] = Articul;
            query["brand"] = Brand;

            // Построение нового URL с обновленными параметрами
            string newUrl = uri.GetLeftPart(UriPartial.Path) + "?" + query.ToString();

            return newUrl;
        }

        // Инициализация драйвера
        private void InitializeDriver()
        {
            UndetectDriver webDriver = new();
            _driver = webDriver.GetDriver();
        }
        #endregion

    }
}
