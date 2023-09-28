using DromAutoTrader.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AppContext = DromAutoTrader.Data.Connection.AppContext;

namespace DromAutoTrader.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        #region Приватные поля
        private string? _title = string.Empty;
        private AppContext _db = null!;
        private ObservableCollection<Supplier> _suppliers = null!;
        private Supplier _selectedSupplier = null!;
        #endregion

        #region Публичный поля
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public AppContext Db
        {
            get => _db;
            set => Set(ref _db, value);
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set => Set(ref _suppliers, value);
        }

        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set => Set(ref _selectedSupplier, value);
        }
        #endregion

        #region Команды
        public ICommand AddSupplierCommand { get; } = null!;

        private bool CanAddSupplierCommandExecute(object p) => true;

        private void OnAddSupplierCommandExecuted(object sender)
        {
            
            
        }
        #endregion


        public MainWindowViewModel()
        {
            // Инициализация базы данных
            InitializeDatabase();

            #region Инициализация команд
            AddSupplierCommand = new LambdaCommand(OnAddSupplierCommandExecuted, CanAddSupplierCommandExecute);
            #endregion
        }


        #region Методы
        // Метод инициализации базы данных
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


        #region Поставщики



        #endregion


        #endregion
    }
}
