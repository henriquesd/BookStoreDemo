using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Dtos.Book
{
    public class BookEditDto
    {
        [Required(ErrorMessage = "The field {0} is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be a valid category ID")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(150, ErrorMessage = "The field {0} must be between {2} and {1} characters", MinimumLength = 2)]
        public string Author { get; set; } = null!;

        [StringLength(350, ErrorMessage = "The field {0} must be at most {1} characters")]
        public string? Description { get; set; }

        [Range(0.01, 999999999.99, ErrorMessage = "The field {0} must be greater than zero")]
        public decimal Value { get; set; }

        public DateTime PublishDate { get; set; }
    }
}
