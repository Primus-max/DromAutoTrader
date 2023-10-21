using AngleSharp.Html.Dom;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.Services;
using System.Threading;

namespace DromAutoTrader.ImageServices
{
    public class LuzarImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://lynxauto.info/";

        protected override string SearchPageUrl => "https://lynxauto.info/index.php?route=product/category/search";

        protected override string UserName => "";

        protected override string Password => "";

        public override string ServiceName => "lynxauto.info";
        #endregion

        #region Приватные поля
        private IHtmlDocument _document = null!;
        #endregion

        public LuzarImageService() { }


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
