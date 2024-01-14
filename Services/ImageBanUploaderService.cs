using Newtonsoft.Json.Linq;

namespace DromAutoTrader.Services
{
    public class ImageBanUploaderService
    {
        private const string ApiBaseUrl = "https://api.imageban.ru/v1";
        private const string SecretKey = "73ITNPONgReAaANkK2xMBahDS5bAko8e0w6"; // Замените на ваш Secret Key
        private HttpClient _httpClient;

        public ImageBanUploaderService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {SecretKey}");
        }

        /// <summary>
        /// Метод создания альбома
        /// </summary>
        /// <param name="albumName"></param>
        /// <returns>Возвращает string id альбома</returns>
        public async Task<string> CreateAlbum(string albumName)
        {

            var formData = new MultipartFormDataContent();           

            try
            {

                // Add optional parameters
                if (!string.IsNullOrEmpty(albumName))
                    formData.Add(new StringContent(albumName), "album_name");

                // Send POST request
                var response = await _httpClient.PostAsync("https://api.imageban.ru/v1/album", formData);

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseData);

                    // Parse responseData to get the link to the uploaded image
                    JObject? albumIdObj = ParseImageData(responseData);
                    string albumId = albumIdObj["id"]?.ToString();
                    return albumId;
                }
                else
                {
                    // Handle error response
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить фото {ex.Message}");
                return null!;
            }

        }


        /// <summary>
        /// Метод загрузки изображения, в ответе получаем ссылку на изображение
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public async Task<Photo> UploadImageFromFile(string imagePath, string albumId = null!)
        {

            // Prepare form data
            var formData = new MultipartFormDataContent();
            Photo photo = new ();


            try
            {
                // Load image file
                byte[] imageData = File.ReadAllBytes(imagePath);
                formData.Add(new ByteArrayContent(imageData), "image", Path.GetFileName(imagePath));

                // Add optional parameters
                if (!string.IsNullOrEmpty(albumId))
                    formData.Add(new StringContent(albumId), "album");

                // Send POST request
                var response = await _httpClient.PostAsync("", formData);

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseData);

                    // Parse responseData to get the link to the uploaded image
                    JObject? imageUrl = ParseImageData(responseData);

                    photo.Link = imageUrl["link"]?.ToString();
                    photo.Name = imageUrl["name"]?.ToString();
                    photo.PhotoId = imageUrl["id"]?.ToString();

                    return photo;
                }
                else
                {
                    // Handle error response
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить фото {ex.Message}");
                return null!;
            }

        }

        // Вспомогательный метод получения данных из поля data
        private JObject ParseImageData(string responseData)
        {
            try
            {
                var json = JObject.Parse(responseData);
                var data = json["data"] as JObject;

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON response: {ex.Message}");
                return null;
            }
        }
    }
}
