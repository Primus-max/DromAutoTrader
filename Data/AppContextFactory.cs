using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromAutoTrader.Data
{
    public class AppContextFactory
    {
        private static AppContext? _instance;

        public static AppContext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AppContext();
                _instance.Database.EnsureCreated(); // Проверка и создание базы данных при необходимости
            }

            return _instance;
        }
    }

}
