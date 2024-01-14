using System.ComponentModel.DataAnnotations;

namespace DromAutoTrader.Models
{
    public class AlbumPhoto
    {
        [Key]
        public int Id { get; set; }

        public string? AlbumId { get; set; }
        public Album? Album { get; set; }

        public string? PhotoId { get; set; }
        public Photo? Photo { get; set; }
    }
}
