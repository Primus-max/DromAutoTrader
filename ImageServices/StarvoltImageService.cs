using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DromAutoTrader.ImageServices
{
    public class StarvoltImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://luzar.ru/";

        protected override string SearchPageUrl => "https://luzar.ru/search/";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "luzar.ru";
        #endregion

        #region Приватные поля
        private IHtmlDocument _document = null!;
        #endregion

        public StarvoltImageService()
        {
            
        }


        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //

        
        #region Перезаписанные методы базового класса
        protected override void GoTo()
        {
            throw new NotImplementedException();
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
        // Асинхронный метода перехода на страницу поиска и поиск
        protected async Task GoToAsync(string url = null!)
        {
            try
            {
                using HttpClient httpClient = new();
                string? fullUrl = string.Empty;

                if (url != null)
                {
                    httpClient.BaseAddress = new Uri(LoginPageUrl);
                    fullUrl = new Uri(httpClient.BaseAddress, url).AbsoluteUri;
                }
                else
                {
                    fullUrl = $"{SearchPageUrl}?query={Articul}";
                }


                var response = await httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Получаю документ
                    var pageSource = await response.Content.ReadAsStringAsync();
                    var contextt = BrowsingContext.New(Configuration.Default);
                    var parser = contextt.GetService<IHtmlParser>();
                    _document = parser?.ParseDocument(pageSource);
                }
                else
                {
                    // TODO сделать логирование                    
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения, например, логирование
                Console.WriteLine($"Произошло исключение: {ex.Message}");
            }
        }

        // TODO вынести этот метод в базовый и сделать для всех
        // Метод создания директории и скачивания изображений
        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            Thread.Sleep(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }
        #endregion
    }
}
