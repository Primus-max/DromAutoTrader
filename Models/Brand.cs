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
        public int Id { get; set; } 
        public string? Name { get; set; } = string.Empty; 
        public string? FindImageService { get; set; } = string.Empty;        
        public string? DefaultImage {  get; set; } = string.Empty;            
    }
}
