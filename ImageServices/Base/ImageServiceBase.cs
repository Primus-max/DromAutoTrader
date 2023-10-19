using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Services;
using OpenQA.Selenium;
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
        protected string? _brand = string.Empty;
        protected string? _articul = string.Empty;
        #endregion

        #region Публичные поля
        protected string? Brand { get; set; }
        protected string? Articul { get; set; }
        public List<string>? BrandImages { get; set; }
        #endregion

        public ImageServiceBase()
        {
         
        }

        #region Общие методы для наследников
        public async Task RunAsync(string brandName, string articul)
        {            
            _brand = brandName;
            _articul = articul;

            if (_isFirstRunning)
            {
                _isFirstRunning = false;

                GoTo(_brand, _articul);

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
        #endregion
        

        protected abstract void GoTo(string brand, string articul);
        protected abstract void Authorization();
        protected abstract void SetArticulInSearchInput();
        protected abstract bool IsNotMatchingArticul();
        protected abstract void OpenSearchedCard();
        protected abstract bool IsImagesVisible();
        protected abstract Task<List<string>> GetImages();
        

        protected abstract void SpecificRunAsync(string brandName, string articul);
    }

}
