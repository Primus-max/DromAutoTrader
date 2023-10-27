using System.IO;

namespace DromAutoTrader.Services
{
    public class SelectionImagesPaths
    {
        public SelectionImagesPaths()
        {
        }

        public List<string> SelectLocalPaths(string Brand, string Articul)
        {

            List<string> imagePaths = new List<string>();
            string baseDirectory = "brand_images"; // Замените на реальный путь

            if (Directory.Exists(baseDirectory))
            {
                string[] brandDirectories = Directory.GetDirectories(baseDirectory, Brand, SearchOption.AllDirectories);

                foreach (string brandDirectory in brandDirectories)
                {
                    if (Directory.Exists(brandDirectory))
                    {
                        string[] imageFiles = Directory.GetFiles(brandDirectory);

                        imagePaths.AddRange(imageFiles);
                    }
                }
            }

            return imagePaths;           
        }

        private List<string> SelectBrandImages(string Brand, string Articul)
        {
            List<string> imagesPaths = new List<string>();

            imagesPaths = SelectLocalPaths(Brand, Articul);

            if (imagesPaths.Count > 0)
            {

            }

            return imagesPaths;
        }
    }
}
