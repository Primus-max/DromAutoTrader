using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DromAutoTrader.Models
{
    public class Photo
    {
        [Key]
        public int? Id { get; set; }
        public string? PhotoId { get; set; }
        public string? Name { get; set; }
        public string? Link { get; set; }

        [InverseProperty("Photos")]
        public List<Album>? Albums { get; set; }
    }
}
