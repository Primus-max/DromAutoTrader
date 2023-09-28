namespace DromAutoTrader.Models
{
    public class Supplier
    {
        public int Id { get; set; } // Уникальный идентификатор поставщика
        public string Name { get; set; } // Название поставщика
        public string Address { get; set; } // Адрес поставщика
        public string ContactEmail { get; set; } // Контактный адрес электронной почты
        public string PhoneNumber { get; set; } // Номер телефона поставщика
    }

}
