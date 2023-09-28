using DromAutoTrader.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AppContext = DromAutoTrader.Data.Connection.AppContext;

namespace DromAutoTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppContext _db = null!;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализируем базу данных
            InitializeDatabase();
        }

        // Метод для инициализации базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = new AppContext();
                // гарантируем, что база данных создана
                _db.Database.EnsureCreated();
                // загружаем данные из БД
                _db.Suppliers.Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }

        // Метод для получения всех поставщиков из базы данных
        public List<Supplier> GetAllSuppliers()
        {
            try
            {
                return _db.Suppliers.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось получить список поставщиков: {ex.Message}");
                return new List<Supplier>(); // Вернуть пустой список или обработать ошибку иным способом
            }
        }

        // Другие методы и события вашего MainWindow
    }
}
