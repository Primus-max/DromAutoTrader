using System.IO;
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

        /// <summary>
        /// Асинхронный метод для скачивания изображений из удалённых источников
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="downloadDirectory"></param>
        /// <param name="imageUrls"></param>
        /// <returns>Результат сохраняет в downloadDirectory</returns>
        public async Task DownloadImagesAsync()
        {
            try
            {
                for (int i = 0; i < _imageUrls.Count; i++)
                {
                    string imageUrl = _imageUrls[i];
                    string fileName = $"{_articul}_{i:00}.jpg";
                    string localFilePath = Path.Combine(_downloadDirectory, fileName);

                    using (HttpClient client = new HttpClient())
                    {
                        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
                        File.WriteAllBytes(localFilePath, imageBytes);
                    }

                    Console.WriteLine($"Изображение {fileName} успешно скачано и сохранено в {localFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}

