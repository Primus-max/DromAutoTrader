using System.IO;


namespace DromAutoTrader.Services
{
    /// <summary>
    /// Класс по рабоде с директориями (провекра, создание)
    /// </summary>
    public class FolderManager
    {
        /// <summary>
        /// Метод проверки существования или создания директорий: 
        /// 1. корневая директория - brand_images
        /// 2. директория в brand_images/ с брэндом - brandName
        /// 3. директория в  brand_images/brandName с именем артикула - articulName
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="brand"></param>
        /// <param name="articul"></param>
        public void EnsureFolderStructure(string rootPath, string brand, string articul)
        {
            string brandImagesPath = Path.Combine(rootPath, "brand_images");

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
        /// Метод проверки наличия файлов в директории с имненем артикула:  
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="brand"></param>
        /// <param name="articul"></param>
        /// <returns>bool IsFolderContainsFiles</returns>        
        public bool ArticulFolderContainsFiles(string rootPath, string brand, string articul)
        {
            string articulPath = Path.Combine(rootPath, "brand_images", brand, articul);

            if (Directory.Exists(articulPath))
            {
                string[] files = Directory.GetFiles(articulPath);
                return files.Length > 0;
            }

            return false;
        }
    }
}
