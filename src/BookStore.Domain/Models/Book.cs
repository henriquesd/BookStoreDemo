namespace BookStore.Domain.Models
{
    public class Book : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Value { get; set; }
        public DateTime PublishDate { get; set; }
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}