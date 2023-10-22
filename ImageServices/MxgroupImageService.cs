using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DromAutoTrader.ImageServices
{
    public class MxgroupImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://berg.ru/login";

        protected override string SearchPageUrl => "https://berg.ru/search/step2?search=AG19166&brand=TRIALLI&withRedirect=1";

        protected override string UserName => "autobest038";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "berg.ru";
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
            throw new NotImplementedException();
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
