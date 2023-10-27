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
        public async Task< List<string>> SelectPaths(string Brand, string Articul)
        {
            List<string> imagesPaths = new List<string>();

            imagesPaths = SelectLocalPaths(Brand, Articul);

            // Если локально не получили картинки, получаем с сайтов
            if (imagesPaths.Count == 0)
            {
                List<string> imageServices = GetImageServicesForBrand(Brand);

                foreach (var imageService in imageServices)
                {
                    imagesPaths = await RunImageServiceAsync( Brand,  Articul, imageService);
                }
            }

            return imagesPaths;
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

        // Метод получения списка сервисов картинок для бренда
        private List<string> GetImageServicesForBrand(string brandName)
        {
            List<string> imageServicesForBrand = new List<string>();
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
                    var imageServiceIds = _db.BrandImageServiceMappings
                        .Where(mapping => mapping.BrandId == brandId)
                        .Select(mapping => mapping.ImageServiceId)
                        .ToList();

                    if (imageServiceIds.Any())
                    {
                        // 3. Получаем имена ImageService по Id
                         imageServicesForBrand = _db.ImageServices
                            .Where(service => imageServiceIds.Contains(service.Id))
                            .Select(service => service.Name)
                            .ToList();

                        return imageServicesForBrand;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и логирование ошибки
                // Console.WriteLine($"Ошибка при получении ImageService для бренда: {ex.Message}");
               // return imageServicesForBrand;
            }
            return imageServicesForBrand;
        }

        // Скачивам картинки с сайтов
        public async Task<List<string>> RunImageServiceAsync(string brand, string articul, string imageServiceName)
        {
            List<string> downLoadedImagesPaths = new List<string>();

            Dictionary<string, Type> imageServiceTypes = new Dictionary<string, Type>
            {
                { "https://berg.ru/", typeof(BergImageService) },
                { "https://uniqom.ru/", typeof(UnicomImageService) },
                { "https://lynxauto.info/", typeof(LynxautoImageService) },
                { "https://luzar.ru/", typeof(LuzarImageService) },
                { "https://startvolt.com/", typeof(StarvoltImageService) },
                { "https://irk.rossko.ru/", typeof(IrkRosskoImageService) },
                { "https://tmparts.ru/", typeof(TmpartsImageService) },
            };


            // Проверяем, есть ли имя сервиса в словаре
            if (imageServiceTypes.TryGetValue(imageServiceName, out Type imageServiceType))
            {
                // Создаем экземпляр ImageService
                var imageService = Activator.CreateInstance(imageServiceType) as IImageService;

                if (imageService != null)
                {
                    // Выполнение RunAsync для найденного сервиса
                    await imageService.RunAsync(brand, articul);

                    // Получение результатов
                    downLoadedImagesPaths = imageService.BrandImages;
                    var serviceName = imageService.ServiceName;

                    return downLoadedImagesPaths;
                    // Здесь вы можете использовать brandImages и serviceName по вашим потребностям
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
