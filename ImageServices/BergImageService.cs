using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Services;
using OpenQA.Selenium;

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
            throw new NotImplementedException();
        }


        public IWebElement GetSearchInput()
        {
            throw new NotImplementedException();
        }

        public void SetArticulInSearchInput(string articul)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
