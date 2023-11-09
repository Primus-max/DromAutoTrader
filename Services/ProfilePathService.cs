using System.IO;

namespace DromAutoTrader.Services
{
    //private readonly string _originalProfilePath;
    /// <summary>
    /// Класс по работе с директориями профилей для Selenium
    /// </summary>
    public class ProfilePathService
    {      
        public ProfilePathService()
        {
            //_originalProfilePath = originalProfilePath;
        }

        /// <summary>
        /// Метод создания копии профиля на сессию
        /// </summary>
        /// <param name="originalProfilePath"></param>
        /// <returns></returns>
        public string CreateTempProfile(string originalProfilePath)
        {
            string uniqueIdentifier = Guid.NewGuid().ToString("N"); // Генерируем уникальный идентификатор
            string tempBasePath = Path.Combine(originalProfilePath, ".."); // Указываем базовую директорию для временных профилей
            string tempPath = Path.Combine(tempBasePath, uniqueIdentifier); // Создаем уникальную директорию

            // Копируем основной профиль во временную директорию
            CopyProfile(originalProfilePath, tempPath);

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

        /// <summary>
        /// Асинхронный метод удаления директории профиля после его использования
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task DeleteDirectoryAsync(string path)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    await DeleteDirectoryAsync(directory); // Рекурсивно удаляем поддиректории асинхронно
                }

                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(file); // Попытка удаления файла
                    }
                    catch (IOException)
                    {
                        // Файл занят другим процессом, можно добавить логику повторного удаления
                        await Task.Delay(1000); // Подождать 1 секунду и повторить попытку
                        File.Delete(file); // Повторная попытка удаления файла
                    }
                }

                Directory.Delete(path, true); // Удаляем саму директорию
            }
            catch (Exception ex)
            {
                // Обработка ошибок удаления
                Console.WriteLine($"Ошибка при удалении директории: {ex.Message}");
            }
        }

    }

}
