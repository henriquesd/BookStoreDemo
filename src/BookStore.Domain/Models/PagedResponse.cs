namespace BookStore.Domain.Models
{
    public record struct PagedResponse<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public int TotalRecords { get; init; }
        public List<T> Data { get; init; }

        public PagedResponse(List<T> data, int pageNumber, int totalItems, int pageSize)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalItems;
            TotalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
        }
    }
}