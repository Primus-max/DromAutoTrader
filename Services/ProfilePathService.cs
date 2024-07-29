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
                    await DeleteDirectoryRecursiveAsync(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении директории: {ex.Message}");
                }
            }

            private async Task DeleteDirectoryRecursiveAsync(string path)
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    await DeleteDirectoryRecursiveAsync(directory);
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    await DeleteFileAsync(file);
                }

                await Task.Run(() => Directory.Delete(path, true));
            }

            private async Task DeleteFileAsync(string filePath)
            {
                const int maxAttempts = 5;
                int attempts = 0;

                while (attempts < maxAttempts)
                {
                    try
                    {
                        await Task.Run(() => File.Delete(filePath));
                        return;
                    }
                    catch (IOException)
                    {
                        await Task.Delay(500);
                        attempts++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при удалении файла {filePath}: {ex.Message}");
                        return;
                    }
                }
            }
        
    }

}
