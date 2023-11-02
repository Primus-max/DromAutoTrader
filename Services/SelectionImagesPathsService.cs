using DromAutoTrader.Data;
using DromAutoTrader.ImageServices;
using DromAutoTrader.ImageServices.Base;
using DromAutoTrader.ImageServices.Interfaces;
using DromAutoTrader.Models;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;

namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс для получения путей к изображениям 
    /// </summary>
    public class SelectionImagesPathsService

    {
        private AppContext _db = null!;
        // Словарь хранит соответствия между ImageService (выбранные для брэнда) и классы для парсинга этих сервисов
        Dictionary<int, Type> imageServiceTypes = new Dictionary<int, Type>
        {
                { 1, typeof(BergImageService) },
                { 2, typeof(UnicomImageService) },
                { 3, typeof(LynxautoImageService) },
                { 4, typeof(LuzarImageService) },
                { 5, typeof(StarvoltImageService) },
                { 6, typeof(IrkRosskoImageService) },
                { 7, typeof(TmpartsImageService) },
        };
        public SelectionImagesPathsService()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// Метод для получения изображений из локального хранилища или из удалённого
        /// </summary>
        /// <param name="Brand">Марка</param>
        /// <param name="Articul">Артикул</param>
        /// <returns>Список путей к изображениям (List&lt;string&gt;)</returns>
        public async Task<List<string>> SelectPaths(string Brand, string Articul)
        {
            // Получаем изображения локально
            List<string> imagesPaths = SelectLocalPaths(Brand, Articul);

            // Если локально не получили пробуем получить удалённо (скачиваем)
            if (imagesPaths.Count == 0)
            {
                List<int> imageServices = GetImageServicesForBrand(Brand);

                foreach (var imageService in imageServices)
                {
                    List<string> serviceImages = await RunImageServiceAsync(Brand, Articul, imageService);

                    await Task.Delay(500); // Задерживаюсь чтобы изображение докачалось

                    if (serviceImages.Count > 0)
                    {
                        return serviceImages; // Если получили изображения, возвращаем их
                    }
                }

                // Если не получили изображений из сервисов, используем DefaultImage из Brand
                string defaultImage = GetDefaultImageForBrand(Brand);

                if (!string.IsNullOrEmpty(defaultImage))
                {
                    imagesPaths.Add(defaultImage);
                }
            }

            return imagesPaths;
        }

        // Получаю изображение по умолчанию, если больше нигде не получили
        private string GetDefaultImageForBrand(string brandName)
        {
            try
            {
                // Пытаемся получить Brand по имени
                var brand = _db.Brands.FirstOrDefault(b => b.Name == brandName);

                if (brand != null)
                {
                    // Если Brand найден, возвращаем значение DefaultImage
                    return brand.DefaultImage;
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и логирование ошибки
                // Console.WriteLine($"Ошибка при получении DefaultImage для бренда: {ex.Message}");
            }

            // Если не удалось получить DefaultImage, возвращаем пустую строку или другое значение по умолчанию
            return string.Empty;
        }

        // Метод получения адресов картинок из локального хранилища
        private List<string> SelectLocalPaths(string Brand, string Articul)
        {
            List<string> imagePaths = new List<string>();
            string baseDirectory = "brand_images"; // Замените на реальный путь

            if (Directory.Exists(baseDirectory))
            {
                string brandDirectory = Path.Combine(baseDirectory, Brand, Articul);

                if (Directory.Exists(brandDirectory))
                {
                    string[] imageFiles = Directory.GetFiles(brandDirectory);

                    imagePaths.AddRange(imageFiles);
                }
            }

            return imagePaths;
        }

        // Метод получения списка сервисов картинок для бренда, возвращает ImageServiceId
        private List<int> GetImageServicesForBrand(string brandName)
        {
            List<int> imageServiceIds = new List<int>();
            try
            {
                // 1. Получаем BrandId по имени бренда
                var brandId = _db.Brands
                    .Where(b => b.Name == brandName)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                if (brandId != null)
                {
                    // 2. Получаем ImageServiceId, принадлежащие бренду
                    imageServiceIds = _db.BrandImageServiceMappings
                        .Where(mapping => mapping.BrandId == brandId)
                        .Select(mapping => mapping.ImageServiceId ?? 0) // Преобразуем Nullable<int> в int
                        .ToList();

                    return imageServiceIds;
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и логирование ошибки
                // Console.WriteLine($"Ошибка при получении ImageService для бренда: {ex.Message}");
                // В случае ошибки, можно вернуть пустой список или null
            }
            return imageServiceIds;
        }

        // Скачивам картинки с сайтов
        private async Task<List<string>> RunImageServiceAsync(string brand, string articul, int imageServiceUrl)
        {
            List<string> downLoadedImagesPaths = new List<string>();            

            // Проверяем, есть ли URL-адрес сервиса в словаре
            if (imageServiceTypes.TryGetValue(imageServiceUrl, out Type imageServiceType))
            {
                // Создаем экземпляр ImageService
                var imageService = Activator.CreateInstance(imageServiceType) as IImageService;

                if (imageService != null)
                {
                    // Выполнение RunAsync для найденного сервиса
                    await imageService.RunAsync(brand, articul);

                    // Получение результатов
                    downLoadedImagesPaths = imageService.BrandImages;
                }
            }

            return downLoadedImagesPaths;
        }

        private void InitializeDatabase()
        {
            try
            {
                _db = AppContextFactory.GetInstance();
                _db.Channels
                    .Include(c => c.PriceIncreases)
                    .Include(c => c.Brands)
                    .Load();
                _db.Brands
                    .Include(b => b.ImageServices)
                    .Load();
                _db.BrandImageServiceMappings
                    .Include(mapping => mapping.ImageServiceId)
                    .Load();               
            }
            catch (Exception ex)
            {
                // Добавьте логирование ошибки для более детального анализа
                // Вместо вывода сообщения в консоль, рекомендуется использовать библиотеки логирования, такие как Serilog, NLog, или другие.
                // Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }

    }
}
