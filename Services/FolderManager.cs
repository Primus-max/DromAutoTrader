using System.IO;


namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс по рабоде с директориями (провекра, создание)
    /// </summary>
    public class FolderManager
    {
        private string? _rootPathDirectory = "brand_images";

        /// <summary>
        /// Метод проверки существования или создания директорий: 
        /// 1. корневая директория - brand_images
        /// 2. директория в brand_images/ с брэндом - brandName
        /// 3. директория в  brand_images/brandName с именем артикула - articulName
        /// </summary>
        /// <param name="rootPath">Путь к корневой директории.</param>
        /// <param name="brand">Название бренда.</param>
        /// <param name="articul">Название артикула.</param>
        public void EnsureFolderStructure( string brand, string articul)
        {
            string brandImagesPath = Path.Combine(_rootPathDirectory);

            if (!Directory.Exists(brandImagesPath))
            {
                Directory.CreateDirectory(brandImagesPath);
            }

            string brandPath = Path.Combine(brandImagesPath, brand);

            if (!Directory.Exists(brandPath))
            {
                Directory.CreateDirectory(brandPath);
            }

            string articulPath = Path.Combine(brandPath, articul);

            if (!Directory.Exists(articulPath))
            {
                Directory.CreateDirectory(articulPath);
            }
        }

        /// <summary>
        /// Метод имеет две перегрузки, возвращает значение bool и директорию к изображениям, в зависимости от флага
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="articul"></param>
        /// <param name="folderPath"></param>
        /// <param name="returnPath"></param>
        /// <returns>bool</returns>
        /// <returns>filePath</returns>
        public bool ArticulFolderContainsFiles(string brand, string articul, out string folderPath)
        {
            string articulPath = Path.Combine(_rootPathDirectory, brand, articul);

            if (Directory.Exists(articulPath))
            {
                string[] files = Directory.GetFiles(articulPath);
                folderPath = articulPath;

                if (files.Length > 0)
                {
                    return true;
                }
            }

            folderPath = null;
            return false;
        }

        public bool ArticulFolderContainsFiles(string brand, string articul)
        {
            string folderPath; // В этой версии метода не нужно передавать folderPath

            return ArticulFolderContainsFiles(brand, articul, out folderPath);
        }


    }
}
