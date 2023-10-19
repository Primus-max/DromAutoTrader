using OpenQA.Selenium;
using System.IO;
using System.Net;
using System.Net.Http;

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

        public async Task<List<string>> DownloadImagesAsync()
        {
            List<string> downloadedImagePaths = new();

            try
            {
                using (HttpClient client = new())
                {
                    for (int i = 0; i < _imageUrls.Count; i++)
                    {
                        string imageUrl = _imageUrls[i];
                        string fileName = $"{_articul}_{i:00}.jpg";
                        string localFilePath = Path.Combine(_downloadDirectory, fileName);

                        using HttpResponseMessage response = await client.GetAsync(imageUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            using Stream imageStream = await response.Content.ReadAsStreamAsync();
                            using FileStream fileStream = File.Create(localFilePath);
                            await imageStream.CopyToAsync(fileStream);
                            await fileStream.FlushAsync(); // Добавьте эту строку
                            downloadedImagePaths.Add(localFilePath);
                        }
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
        /// Синхронный метод для скачивания изображений из удалённых источников
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="downloadDirectory"></param>
        /// <param name="imageUrls"></param>
        /// <returns>Результат сохраняет в downloadDirectory</returns>
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

