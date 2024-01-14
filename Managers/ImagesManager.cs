namespace DromAutoTrader.Managers
{
    public class ImagesManager
    {
        private readonly ImageBanUploaderService _imageBanUploader;

        public ImagesManager()
        {
            _imageBanUploader = new ImageBanUploaderService();
        }

        public async Task UploadImages(string brand, string articul, List<string> imagePaths)
        {
            using var appContext = new AppContext();
            // Проверяем наличие бренда в базе
            var existingAlbum = appContext.Albums.FirstOrDefault(a => a.Name == brand);

            // Если альбома для бренда нет, создаем новый
            if (existingAlbum == null)
            {
                // Создаем альбом на сервере ImageBan
                var albumId = await _imageBanUploader.CreateAlbum(brand);

                // Если не удалось создать альбом, выходим из метода
                if (albumId == null)
                {
                    Console.WriteLine($"Не удалось создать альбом для бренда {brand}");
                    return;
                }

                // Добавляем альбом в базу данных
                var newAlbum = new Album { Name = brand, AlbumId = albumId };
                appContext.Albums.Add(newAlbum);
                appContext.SaveChanges();
                existingAlbum = newAlbum;
            }


            // Теперь загружаем фото в ImageBan и добавляем ссылки в базу
            foreach (var imagePath in imagePaths)
            {
                // Загружаем изображение в ImageBan и получаем ссылку
                Photo image = await _imageBanUploader.UploadImageFromFile(imagePath, existingAlbum.AlbumId);

                // Добавляем ссылку в базу данных
                var newPhoto = new Photo { PhotoId = image.PhotoId, Name = articul, Link = image.Link };
                appContext.Photos.Add(newPhoto);

                // Устанавливаем связь между фото и альбомом
                //_dbContext.AlbumPhotos.Add(new AlbumPhoto { AlbumId = existingAlbum.Id, PhotoId = newPhoto.Id });
            }

            appContext.SaveChanges();
        }


        public List<string> GetImagesLinks(string brand, string articul)
        {

            using var appContext = new AppContext();

            // Ищем альбом для бренда
            var album = appContext.Albums.FirstOrDefault(a => a.Name == brand);

            if (album == null)
            {
                // Альбома нет, возвращаем пустой список
                return new List<string>();
            }

            // Ищем фотографии, связанные с альбомом и заданным артикулом
            var imageLinks = appContext.AlbumPhotos
                .Where(ap => ap.AlbumId == album.AlbumId && ap.Photo.Name.Contains(articul))
                .Select(ap => ap.Photo.Link)
                .ToList();

            return imageLinks;
        }

    }

}
