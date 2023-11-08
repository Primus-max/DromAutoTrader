using System.IO;

namespace DromAutoTrader.Services
{
    public class ProfilePathService
    {
        private readonly string _originalProfilePath;

        public ProfilePathService(string originalProfilePath)
        {
            _originalProfilePath = originalProfilePath;
        }

        public string CreateTempProfile()
        {
            string uniqueIdentifier = Guid.NewGuid().ToString("N"); // Генерируем уникальный идентификатор
            string tempBasePath = Path.Combine(_originalProfilePath, ".."); // Указываем базовую директорию для временных профилей
            string tempPath = Path.Combine(tempBasePath, uniqueIdentifier); // Создаем уникальную директорию

            // Копируем основной профиль во временную директорию
            CopyProfile(_originalProfilePath, tempPath);

            return tempPath;
        }



        private void CopyProfile(string sourcePath, string destinationPath)
        {
            try
            {
                Directory.CreateDirectory(destinationPath);

                foreach (var file in Directory.EnumerateFiles(sourcePath))
                {
                    File.Copy(file, Path.Combine(destinationPath, Path.GetFileName(file)), true);
                }

                foreach (var dir in Directory.EnumerateDirectories(sourcePath))
                {
                    string destinationSubDir = Path.Combine(destinationPath, Path.GetFileName(dir));
                    CopyProfile(dir, destinationSubDir);
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок копирования
                Console.WriteLine($"Ошибка при копировании профиля: {ex.Message}");
            }
        }
    }

}
