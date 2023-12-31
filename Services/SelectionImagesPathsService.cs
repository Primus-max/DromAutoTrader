﻿using DromAutoTrader.ImageServices;
using DromAutoTrader.ImageServices.Interfaces;

namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс для получения путей к изображениям 
    /// </summary>
    public class SelectionImagesPathsService
    {
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
            List<string> localImagesPaths = SelectLocalPaths(Brand, Articul);

            // Проверяем локальные пути
            List<string> validLocalPaths = localImagesPaths
                .Where(path => !path.StartsWith("C:\\"))
                .ToList();

            // Если есть валидные локальные пути, возвращаем их
            if (validLocalPaths.Count > 0)
            {
                return validLocalPaths;
            }

            // Локальные пути не удовлетворяют условиям, пробуем удаленно
            List<int> imageServices = GetImageServicesForBrand(Brand);

            foreach (var imageService in imageServices)
            {
                List<string>? serviceImages = await RunImageServiceAsync(Brand, Articul, imageService);
                if (serviceImages == null) continue;

                await Task.Delay(500); // Задерживаюсь чтобы изображение докачалось

                if (serviceImages.Count > 0)
                {
                    return serviceImages; // Если получили изображения, возвращаем их
                }
                else
                {
                    if (localImagesPaths.Count > 0)
                        return localImagesPaths; // Если не скачали, то берём то, что есть локально
                }
            }

            // Если не получили изображений из сервисов, используем DefaultImage из Brand
            string defaultImage = GetDefaultImageForBrand(Brand);

            if (!string.IsNullOrEmpty(defaultImage))
            {
                return new List<string> { defaultImage };
            }

            // Если ничего не найдено, возвращаем пустой список
            return new List<string>();
        }


        // Получаю изображение по умолчанию, если больше нигде не получили
        private string GetDefaultImageForBrand(string brandName)
        {
            try
            {
                using var context = new AppContext();
                // Пытаемся получить Brand по имени
                var brand = context.Brands.FirstOrDefault(b => b.Name == brandName);

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
            string baseDirectory = "brand_images";

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
                using var context = new AppContext();
                // 1. Получаем BrandId по имени бренда
                var brandId = context.Brands
                    .Where(b => b.Name == brandName)
                    .Select(b => b.Id)
                    .FirstOrDefault();

                if (brandId != 0)
                {
                    // 2. Получаем ImageServiceId, принадлежащие бренду
                    imageServiceIds = context.BrandImageServiceMappings
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

                if (Activator.CreateInstance(imageServiceType) is IImageService imageService)
                {
                    // Выполнение RunAsync для найденного сервиса
                    await imageService.RunAsync(brand, articul);

                    // Получение результатов
                    downLoadedImagesPaths = imageService.BrandImages;
                }
            }

            return downLoadedImagesPaths;
        }



    }
}
