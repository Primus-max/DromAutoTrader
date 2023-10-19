using OpenQA.Selenium;

namespace DromAutoTrader.ImageServices.Interfaces
{
    /// <summary>
    /// Интерфейс описыват класс сайта для парсинга картинок брэндов
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Название сайта / сервиса
        /// </summary>
        string ServiceName { get; }

        ///// <summary>
        ///// Название брэнда для которого получаем изображение(я)
        ///// </summary>
        //string? _brand { get; set; }

        ///// <summary>
        ///// Артикул товара
        ///// </summary>
        //string? _articul { get; set; }

        /// <summary>
        /// Список (локальных) ссылок на картинки полученных с сайта (сначала сохраняем в локальное хранилище и их же возвращаем) 
        /// </summary>
        List<string> BrandImages { get; set; }

        /// <summary>
        /// Асинхронный метод (входная точка) для запсука парсинга на сайте.
        /// </summary>
        /// <param name="brandName">Название бренда для поиска изображений.</param>
        /// <param name="articul">Артикул товара для поиска.</param>
        /// <returns>Задача, представляющая выполнение операции.</returns>
        Task RunAsync(string brandName, string articul);
    }
}
