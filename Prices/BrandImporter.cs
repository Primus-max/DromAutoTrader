using DromAutoTrader.Prices;

/// <summary>
/// Класс для импорта брендов из прайс-листа в базу данных.
/// </summary>
public class BrandImporter
{
    private AppContext _db = null!;

    /// <summary>
    /// Инициализирует экземпляр класса <see cref="BrandImporter"/> и базу данных.
    /// </summary>
    public BrandImporter()
    {
        InitializeDatabase(); // Инициализация базы данных при создании экземпляра BrandImporter.
    }

    /// <summary>
    /// Импортирует бренды из переданного прайс-листа в базу данных.
    /// </summary>
    /// <param name="prices">Прайс-лист, содержащий информацию о брендах.</param>
    public void ImportBrandsFromPrices(PriceList prices)
    {
        foreach (var priceItem in prices)
        {
            if (!string.IsNullOrEmpty(priceItem.Brand))
            {
                // Проверка на дубликаты в базе данных
                bool isDuplicate = _db.Brands.Any(b => b.Name == priceItem.Brand);

                if (!isDuplicate)
                {
                    // Создание нового объекта Brand и добавление его в базу данных
                    var brand = new Brand { Name = priceItem.Brand };
                    _db.Brands.Add(brand);
                    _db.SaveChanges(); // Сохранение изменений в базе данных.
                }
                else
                {
                    // Обработка дубликата (по вашему желанию).
                }
            }
        }
    }

    private void InitializeDatabase()
    {
        try
        {
            // Экземпляр базы данных
            _db = new AppContext();
            // Гарантируем, что база данных создана
            _db.Database.EnsureCreated();
            // Загружаем данные о поставщиках из БД (по вашей логике).
        }
        catch (Exception ex)
        {
            // TODO: Добавить запись логов
            //Console.WriteLine($"Не удалось инициализировать базу данных: {ex.Message}");
        }
    }
}
