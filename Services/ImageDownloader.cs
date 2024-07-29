using System.Net;

namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс для скачивания изображений из удалённых источников
    /// </summary>
    class ImageDownloader
    {
        private string? _articul = string.Empty;
        private string? _downloadDirectory = string.Empty;
        private List<string> _imageUrls = null!;

        public ImageDownloader(string articul, string downloadDirectory, List<string> imageUrls)
        {
            _articul = articul;
            _downloadDirectory = downloadDirectory;
            _imageUrls = imageUrls;
        }

        /// <summary>
        /// Асинхронный метод для скачивания изображений из удалённых источников с использованием <see cref="HttpResponseMessage"/>.
        /// </summary>
        /// <param name="articul">Артикул</param>
        /// <param name="downloadDirectory">Директория для загрузки</param>
        /// <param name="imageUrls">Список URL изображений</param>
        /// <returns>Результат сохраняется в <paramref name="downloadDirectory" />. Возвращает список путей к скачанным изображениям.</returns>
        public async Task<List<string>> DownloadImagesAsync()
        {
            List<string> downloadedImagePaths = new List<string>();

            try
            {
                using HttpClient client = new HttpClient();
                for (int i = 0; i < _imageUrls.Count; i++)
                {
                    string imageUrl = _imageUrls[i];
                    string fileName = $"{_articul}_{i:00}.jpg";
                    string localFilePath = Path.Combine(_downloadDirectory, fileName);

                    //Uri uri = new Uri($"https:{imageUrl}"); // Явно указываем схему "https"

                    using HttpResponseMessage response = await client.GetAsync(imageUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        using Stream imageStream = await response.Content.ReadAsStreamAsync();
                        using FileStream fileStream = File.Create(localFilePath);
                        await imageStream.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        downloadedImagePaths.Add(localFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return downloadedImagePaths;
        }


        /// <summary>
        /// Асинхронный метод для скачивания изображений из удалённых источников с использованием 
        /// <see cref="HttpResponseMessage"/>
        /// и <see cref="CookieContainer"/> поученный от авторизованной сессии. 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="articul"></param>
        /// <param name="downloadDirectory"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public async Task<List<string>> DownloadImageWithCookiesAsync(IWebDriver driver)
        {
            List<string> downloadedImagePaths = new();
            try
            {
                string imageUrl = _imageUrls[0];
                string fileName = $"{_articul}_{0:00}.jpg";
                // Получение кук из WebDriver
                var seleniumCookies = driver.Manage().Cookies.AllCookies;

                using HttpClient client = new HttpClient();
                var cookieContainer = new CookieContainer();

                // Преобразовываем куки из Cookie (Selenium) в Cookie (HttpClient)
                foreach (var seleniumCookie in seleniumCookies)
                {
                    var httpClientCookie = new System.Net.Cookie(seleniumCookie.Name, seleniumCookie.Value, seleniumCookie.Path, seleniumCookie.Domain);
                    cookieContainer.Add(new Uri(imageUrl), httpClientCookie);
                }

                // Устанавливаем CookieContainer в HttpClient
                var clientHandler = new HttpClientHandler
                {
                    CookieContainer = cookieContainer
                };

                using (var httpClient = new HttpClient(clientHandler))
                {
                    using HttpResponseMessage response = await httpClient.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                        string localFilePath = Path.Combine(_downloadDirectory, fileName);

                        // Сохраняем скачанное изображение в файл
                        File.WriteAllBytes(localFilePath, imageBytes);

                        downloadedImagePaths.Add(localFilePath);
                        return downloadedImagePaths;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return downloadedImagePaths;
        }


        /// <summary>
        /// Синхронный метод для скачивания изображений из удалённых источников с использованием <see cref="WebClient"/>.
        /// </summary>
        /// <param name="articul">Артикул</param>
        /// <param name="downloadDirectory">Директория для загрузки</param>
        /// <param name="imageUrls">Список URL изображений</param>
        /// <returns>Результат сохраняется в <paramref name="downloadDirectory" />. Возвращает список путей к скачанным изображениям.</returns>
        public List<string> DownloadImages()
        {
            List<string> downloadedImagePaths = new List<string>();

            try
            {
                for (int i = 0; i < _imageUrls.Count; i++)
                {
                    string imageUrl = _imageUrls[i];
                    string fileName = $"{_articul}_{i:00}.jpg";
                    string localFilePath = Path.Combine(_downloadDirectory, fileName);

                    using WebClient client = new WebClient();
                    client.DownloadFile(imageUrl, localFilePath);
                    downloadedImagePaths.Add(localFilePath); // Добавляем путь к скачанному файлу в список
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            return downloadedImagePaths;
        }

    }
}

