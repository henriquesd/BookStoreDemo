﻿namespace BookStore.API.Dtos
{
    public readonly record struct PagedResponseDto<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public int TotalRecords { get; init; }
        public List<T> Data { get; init; }
    }
}