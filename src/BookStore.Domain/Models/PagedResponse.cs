namespace BookStore.Domain.Models
{
    public readonly record struct PagedResponse<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public int TotalRecords { get; init; }
        public List<T> Data { get; init; }

        public PagedResponse(List<T> data, int pageNumber, int totalRecords, int pageSize)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize);
        }
    }
}