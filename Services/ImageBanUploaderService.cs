using Newtonsoft.Json.Linq;

namespace DromAutoTrader.Services
{
    public class ImageBanUploaderService
    {
        private const string ApiBaseUrl = "https://api.imageban.ru/v1";
        private const string SecretKey = "y67rw8ci8LUAW9VEdxs8LWAHKF1nnGrLIUy"; // Замените на ваш Secret Key

        public static async Task<string> UploadImageFromFile(string imagePath, string albumId = null)
        {
            using (var httpClient = new HttpClient())
            {
                // Set Authorization header for authorized user access
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {SecretKey}");

                // Prepare form data
                var formData = new MultipartFormDataContent();

                try
                {
                    // Load image file
                    byte[] imageData = File.ReadAllBytes(imagePath);
                    formData.Add(new ByteArrayContent(imageData), "image", Path.GetFileName(imagePath));

                    // Add optional parameters
                    if (!string.IsNullOrEmpty(albumId))
                        formData.Add(new StringContent(albumId), "album");

                    // Send POST request
                    var response = await httpClient.PostAsync(ApiBaseUrl, formData);

                    // Handle response
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseData);

                        // Parse responseData to get the link to the uploaded image
                        var imageUrl = ParseImageUrl(responseData);
                        return imageUrl;
                    }
                    else
                    {
                        // Handle error response
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        return null;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private static string ParseImageUrl(string responseData)
        {
            // Parse JSON response to get the link to the uploaded image
            var json = JObject.Parse(responseData);
            var data = json["data"] as JObject;

            if (data != null)
            {
                var imageUrl = data?["link"]?.ToString();
                return imageUrl;
            }
            else
            {
                Console.WriteLine("Invalid JSON response format.");
                return null;
            }
        }
    }
}
