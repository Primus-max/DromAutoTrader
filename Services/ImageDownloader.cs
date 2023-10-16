using System.IO;
using System.Net.Http;

namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс для скачивания изображений из удалённых источников
    /// </summary>
    class ImageDownloader
    {
        public ImageDownloader() { }

        /// <summary>
        /// Асинхронный метод для скачивания изображений из удалённых источников
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="downloadDirectory"></param>
        /// <param name="imageUrls"></param>
        /// <returns>Результат сохраняет в downloadDirectory</returns>
        public async Task DownloadImagesAsync(string articul, string downloadDirectory, List<string> imageUrls)
        {
            try
            {
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    string imageUrl = imageUrls[i];
                    string fileName = $"{articul}_{i:00}.jpg";
                    string localFilePath = Path.Combine(downloadDirectory, fileName);

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

