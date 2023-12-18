namespace BookStore.Domain.Models
{
    public class Pagination
    {
        public class PagedResponse<T>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
            public int TotalRecords { get; set; }
            public List<T> Data { get; set; }

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
}