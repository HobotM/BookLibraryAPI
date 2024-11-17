using System.ComponentModel.DataAnnotations;

namespace BookLibraryAPI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]  // The Title field is required
        [MaxLength(100)]  // Maximum length of 100 characters
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string Author { get; set; }

        [MaxLength(30)]
        public string Genre { get; set; }

        public int Year { get; set; }
    }
}
