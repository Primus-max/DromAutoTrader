namespace DromAutoTrader.ImageServices
{
    public class UnicomImageService : ImageServiceBase
    {
        #region Перезапись абстрактных свойст
        protected override string LoginPageUrl => "https://uniqom.ru/#login";

        protected override string SearchPageUrl => "https://uniqom.ru/search?term=";

        protected override string UserName => "autobest038@gmail.com";

        protected override string Password => "dimonfutboll";

        public override string ServiceName => "https://uniqom.ru";
        #endregion

        #region Приватные поля         
        private string _responseContent = string.Empty; // Глобально для класса сохраняю ответ
        #endregion


        public UnicomImageService()
        {

        }

        //----------------------- Реализация метод RunAsync находится в базовом классе ----------------------- //


        #region Перезаписанные методы базового класса       

        // Метод перехода по ссылке
        protected override void GoTo()
        {
            Task.Run(async () => await GoToAsync()).Wait();
        }

        // Отправляем запрос на получение изображений
        protected async Task GoToAsync()
        {
            string baseUrl = "https://uniqom.ru/search/api/query";
            string rememberMeCookie = "QXBwXFNlY3VyaXR5XFVzZXI6UVhWMGIwSmxjM1F3TXpoQVoyMWhhV3d1WTI5dDoxNzMyNjI2NTYwOjU2NzFmNThlOWIyNDBkY2ViYWIyNzBkYjg3NjJhOWRmY2UzZDU2NTYyMmQyMzRkNGY3YzgyNDdhZDdkODQ5NDY%3D";

            using HttpClient client = new();
            client.BaseAddress = new Uri(baseUrl);

            // Set headers
            client.DefaultRequestHeaders.Add("access-control-allow-origin", "*");

            // Set cookies
            client.DefaultRequestHeaders.Add("Cookie", rememberMeCookie);

            // Perform the GET request with query parameters
            HttpResponseMessage response = await client.GetAsync($"?term={Articul}&page=1");

            // Handle the response as needed
            if (response.IsSuccessStatusCode)
            {
                _responseContent = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Handle error
            }
        }

        protected override void Authorization() { }

        protected override void SetArticulInSearchInput() { }

        // Метод проверяет если ничего не найдено
        protected override bool IsNotMatchingArticul()
        {
            try
            {
                // Замените на вашу структуру JSON
                dynamic decodedResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(_responseContent);

                if (decodedResponse.products != null && decodedResponse.products.Count > 0 &&
                    decodedResponse.products[0].thumbnail != null)
                {
                    string thumbnail = decodedResponse.products[0].thumbnail;

                    // Проверка, содержит ли thumbnail подстроку "no_image"
                    return thumbnail.Contains("no_image");
                }
            }
            catch (Exception)
            {
                // Обработка ошибки парсинга JSON
            }

            return false;
        }

        // Открываю карточку с изображениями (в данном классе реализация не требуется)
        protected override void OpenSearchedCard() { }

        // Метод проверки наличия изображения для дальнейшего получения
        protected override bool IsImagesVisible()
        {
            return true;
        }

        // Метод получения изображений
        protected override async Task<List<string>> GetImagesAsync()
        {
            List<string> downloadedImages = new List<string>();
            List<string> imageUrls = new List<string>();

            try
            {
                // Замените на вашу структуру JSON
                dynamic decodedResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(_responseContent);

                if (decodedResponse.products != null && decodedResponse.products.Count > 0 &&
                    decodedResponse.products[0].exact != null && decodedResponse.products[0].exact.items != null)
                {
                    var items = decodedResponse.products[0].exact.items;

                    foreach (var item in items)
                    {
                        if (item.photo != null)
                        {
                            string imageUrl = "https:" + item.photo.ToString();

                            // Проверка, что изображение не является "photo_thumbnail"
                            if (!imageUrl.Contains("photo_thumbnail"))
                            {
                                imageUrls.Add(imageUrl);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Обработка ошибки парсинга JSON
                return downloadedImages;
            }

            if (imageUrls.Count != 0)
            {
                downloadedImages = await ImagesProcessAsync(imageUrls);
            }

            return downloadedImages;
        }


        // Метод создания директории и скачивания изображений
        private async Task<List<string>> ImagesProcessAsync(List<string> images)
        {
            List<string> downloadedImages = new();

            // Проверяю создан ли путь для хранения картинок
            FolderManager folderManager = new();
            bool folderContainsFiles = folderManager.ArticulFolderContainsFiles(brand: Brand, articul: Articul, out _imagesLocalPath);

            await Task.Delay(1000);

            if (!folderContainsFiles)
            {
                // Скачиваю изображения
                ImageDownloader? downloader = new(Articul, _imagesLocalPath, images);
                downloadedImages = await downloader.DownloadImagesAsync();
            }

            return downloadedImages;
        }

        protected override async Task CloseDriverAsync() { }

        #endregion

    }
}