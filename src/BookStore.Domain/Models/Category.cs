namespace BookStore.Domain.Models
{
    public class Category : Entity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; } = [];
    }
}