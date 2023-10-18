using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromAutoTrader.ImageServices.Interfaces
{
    /// <summary>
    /// Интерфейс описыват класс сайта для парсинга картинок брэндов
    /// </summary>
    public interface IWebsite
    {
        /// <summary>
        /// Название сайта
        /// </summary>
        string WebSiteName { get; }

        /// <summary>
        /// Название брэнда для которого получаем изображение(я)
        /// </summary>
        string? Brand { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        string? Articul { get; set; }

        /// <summary>
        /// Список (локальных) ссылок на картинки полученных с сайта (сначала сохраняем в локальное хранилище и их же возвращаем) 
        /// </summary>
        List<string> BrandImages { get; set; }
       
        /// <summary>
        /// Метод авторизации на сайте
        /// </summary>        
        /// <exception cref="NoSuchElementException"></exception>
        void Authorization();

        /// <summary>
        /// Метод получает и возвращает строку поиска по артикулу
        /// </summary>
        /// <returns></returns>
        IWebElement GetSearchInput();

        /// <summary>
        /// Метод ввода артикула в строку поиска и отправки запроса (перед вводом поле очищается)
        /// </summary>        
        void SetArticulInSearchInput();  
    }
}
