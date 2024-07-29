using DromAutoTrader.ImageServices.Interfaces;
using OpenQA.Selenium;
using System.Threading;

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
        /// <summary>
        /// Метод (точка входа) запускает парсинг. Определён в базовом классе, наследники не перезаписывают.
        /// Использует методы которые обязательны для наследников. Возвращает список скачанных изображений (локальные дареса)
        /// <see cref="GoTo"/> - метод перехода на сайт
        /// <see cref="Authorization"/> - метод авторизации
        /// <see cref="SetArticulInSearchInput"/> - метод вставки артикула в поле поиска
        /// <see cref="IsNotMatchingArticul"/> - реализация этого метод может отличаться в наследниках. Возвращает bool.
        /// <see cref="OpenSearchedCard"/> - Метод перехода в карточку где находятся изображений
        /// <see cref="IsImagesVisible"/> - Метод ожидания появления изображений
        /// <see cref="GetImagesAsync"/> - Метод скачивания изображений (имеет две перегрузки)
        /// </summary>
        /// <param name="brandName"></param>
        /// <param name="articul"></param>
        /// <returns></returns>
        public async Task RunAsync(string brandName, string articul)
        {
            Brand = brandName;
            Articul = articul;

            GoTo();

            // Закрывает окно с предложением получения уведомлений
            await Task.Delay(500);
            //ClosePermissionRequestPopup();

            Authorization();


            SetArticulInSearchInput();

            if (IsNotMatchingArticul())
            {
               await CloseDriverAsync();
                return;
            }


            OpenSearchedCard();

            if (IsImagesVisible())
            {
                BrandImages = await GetImagesAsync();
            }
            else
            {
                BrandImages = null;
            }

           await CloseDriverAsync();

        }
       
        #endregion

        #region Абстратные метод
        protected abstract void GoTo();
        protected abstract void Authorization();
        protected abstract void SetArticulInSearchInput();
        protected abstract bool IsNotMatchingArticul();
        protected abstract void OpenSearchedCard();
        protected abstract bool IsImagesVisible();
        protected abstract Task<List<string>> GetImagesAsync();

        protected abstract Task CloseDriverAsync();
        #endregion
    }

}
