using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DromAutoTrader.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        [InverseProperty("Albums")]
        public List<Photo> Photos { get; set; }
    }
}
