using DromAutoTrader.Data;
using Microsoft.Win32;

namespace DromAutoTrader.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        #region Приватные поля
        #region Базовые
        private string? _title = string.Empty;
        private AppContext _db = null!;
        private string? _pathFilePrice = string.Empty;
        #endregion
        

        #region Поставщики
        private ObservableCollection<Supplier> _suppliers = null!;
        private Supplier _selectedSupplier = null!;
        private string? _newSuplierName = "Имя поставщика";
        private DataGrid _supplierDataGrid = null!;
        #endregion

        #region Бренды
        private ObservableCollection<Brand> _brands = null!;
        #endregion

        #region Каналы
        private Channel _selectedChannel = null!;
        #endregion

        #endregion

        #region Публичный поля
        #region Базовые
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
        public string PathFilePrice
        {
            get => _pathFilePrice;
            set => Set(ref _pathFilePrice, value);
        }
        #endregion

        #region Поставщики
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
        public string? NewSuplierName
        {
            get => _newSuplierName;
            set => Set(ref _newSuplierName, value);
        }
        public DataGrid SupplierDataGrid
        {
            get => _supplierDataGrid;
            set => Set(ref _supplierDataGrid, value);
        }
        #endregion

        #region Бренды
        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set => Set(ref _brands, value);
        }
        #endregion
        #region Каналы
        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set => Set(ref _selectedChannel, value);
        }
        #endregion

        #endregion

        #region КОМАНДЫ

        #region Поставщики
        public ICommand AddSupplierCommand { get; } = null!;

        private bool CanAddSupplierCommandExecute(object p) => true;

        private void OnAddSupplierCommandExecuted(object sender)
        {

            AddNewSupplier();
        }

        public ICommand EnterKeyPressedCommand { get; } = null!;

        private bool CanEnterKeyPressedCommandExecute(object p) => true;

        private void OnEnterKeyPressedCommandExecuted(object sender)
        {
            // Записываю в базу
            SaveSupplierToDatabase(SelectedSupplier.Id, SelectedSupplier?.Name);
        }

        public ICommand EditeSuplierCommand { get; } = null!;

        private bool CanEditeSuplierCommandExecute(object p) => true;

        private void OnEditeSuplierCommandExecuted(object sender)
        {
            if (sender is Supplier selectedSupplier)
            {
                SaveSupplierToDatabase(selectedSupplier.Id, selectedSupplier.Name);
            }
        }

        public ICommand DeleteSuplierCommand { get; } = null!;

        private bool CanDeleteSuplierCommandExecute(object p) => true;

        private void OnDeleteSuplierCommandExecuted(object sender)
        {
            if (sender is Supplier selectedSupplier)
            {
                DeleteSupplierFromDatabase(selectedSupplier.Id);
            }
        }

        public ICommand SelectFilePriceCommand { get; } = null!;

        private bool CanSelectFilePriceCommandExecute(object p) => true;

        private void OnSelectFilePriceCommandExecuted(object sender)
        {
            SelectFilePrice();
        }
       
        #endregion

        #region Каналы

        #endregion

        #endregion


        public MainWindowViewModel()
        {
            // Инициализация базы данных
            InitializeDatabase();

            #region Инициализация команд

            #region Поставщики
            AddSupplierCommand = new LambdaCommand(OnAddSupplierCommandExecuted, CanAddSupplierCommandExecute);
            EnterKeyPressedCommand = new LambdaCommand(OnEnterKeyPressedCommandExecuted, CanEnterKeyPressedCommandExecute);
            EditeSuplierCommand = new LambdaCommand(OnEditeSuplierCommandExecuted, CanEditeSuplierCommandExecute);
            DeleteSuplierCommand = new LambdaCommand(OnDeleteSuplierCommandExecuted, CanDeleteSuplierCommandExecute);
            SelectFilePriceCommand = new LambdaCommand(OnSelectFilePriceCommandExecuted, CanSelectFilePriceCommandExecute);
            #endregion

            #endregion

            #region Инициализация источников данных

            #region Поставщики
            Suppliers = GetAllSuppliers();
            #endregion

            #region Бренды
            Brands = new ObservableCollection<Brand>(_db.Brands.ToList());
            #endregion

            #endregion
        }


        #region Методы

        #region Базовые
        // Метод выбора файла для парсинга
        private void SelectFilePrice()
        {
            // Создаем диалоговое окно выбора файла
            OpenFileDialog openFileDialog = new()
            {
                // Устанавливаем фильтры для типов файлов, которые вы хотите разрешить выбирать
                Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу
                PathFilePrice = openFileDialog.FileName;                
            }
        }

        // Метод инициализации базы данных
        private void InitializeDatabase()
        {
            try
            {
                // Экземпляр базы данных
                _db = AppContextFactory.GetInstance();
                // загружаем данные о поставщиках из БД
                _db.Suppliers.Load();
            }
            catch (Exception)
            {
                // TODO сделать запись логов
                //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
            }
        }

        #endregion

        #region Поставщики

        public void AddNewSupplier()
        {
            // Создаем нового поставщика с пустым именем
            var newSupplier = new Supplier { Id = Suppliers.Count + 1, };

            // Добавляем его в список поставщиков
            Suppliers.Add(newSupplier);

        }

        // Метод для получения всех поставщиков из базы данных
        public ObservableCollection<Supplier> GetAllSuppliers()
        {
            try
            {
                return new ObservableCollection<Supplier>(_db.Suppliers.ToList());
            }
            catch (Exception)
            {
                return new ObservableCollection<Supplier>(); // Вернуть пустой список или обработать ошибку иным способом
            }
        }

        // Метод добавления нового или обновления существующего поставщика
        private void SaveSupplierToDatabase(int id, string name)
        {
            var supplierToUpdate = _db.Suppliers.FirstOrDefault(s => s.Id == id);

            if (supplierToUpdate != null)
            {
                // Если поставщик с таким ID уже существует, обновляем его имя
                supplierToUpdate.Name = name;
            }
            else
            {
                // Если поставщик с таким ID не существует, создаем нового поставщика
                var newSupplier = new Supplier { Name = name };
                _db.Suppliers.Add(newSupplier);
            }

            try
            {
                _db.SaveChanges();
                
                MessageBox.Show($"Поставщик: {name} сохранен");
            }
            catch (Exception)
            {
                // Обработка ошибок сохранения в базе данных
                MessageBox.Show("Произошла ошибка при сохранении поставщика в базе данных.");
            }
        }

        // Метод удаления поставщика
        private void DeleteSupplierFromDatabase(int id)
        {
            var supplierToRemove = _db.Suppliers.FirstOrDefault(s => s.Id == id);
            if (supplierToRemove != null)
            {
                try
                {
                    _db.Suppliers.Remove(supplierToRemove);
                    _db.SaveChanges();

                    Suppliers = GetAllSuppliers();
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось удалить поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion


        #endregion
    }
}
