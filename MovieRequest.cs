using System.ComponentModel.DataAnnotations;

namespace CatalogoAzure
{    public class MovieRequest
    {
        [Required]
        public required string Id { get ; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Year { get; set; }
        [Required]
        public required string Video { get; set; }
        [Required]
        public required string Thumb { get; set; }
    }
}