using System.IO;

namespace DromAutoTrader.Services
{
    public class ProfilePathService
    {
        public string CreateTempPath(string originalPath)
        {
            string uniqueIdentifier = Guid.NewGuid().ToString("N"); // Генерируем уникальный идентификатор
            string tempPath = Path.Combine(originalPath, uniqueIdentifier); // Создаем путь к временной директории

            Directory.CreateDirectory(tempPath); // Создаем временную директорию

            return tempPath;
        }
    }
}
