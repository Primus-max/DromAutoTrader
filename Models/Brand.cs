using DromAutoTrader.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromAutoTrader.Models
{
    class Brand:IBaseModel
    {
        public int Id { get; set; } // Уникальный идентификатор поставщика
        public string? Name { get; set; }  // Название поставщика    
    }
}
