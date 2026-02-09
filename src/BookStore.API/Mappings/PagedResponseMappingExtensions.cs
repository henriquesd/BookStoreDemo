using BookStore.API.Dtos;

namespace BookStore.API.Mappings
{
    public static class PagedResponseMappingExtensions
    {
        public static PagedResponseDto<TDto>? ToDto<TModel, TDto>(
            this PagedResponse<TModel> pagedResponse,
            Func<TModel, TDto> mappingFunction)
        {
            if (pagedResponse == null)
            {
                return null;
            }

            return new PagedResponseDto<TDto>
            {
                Data = pagedResponse.Data?.Select(mappingFunction).ToList(),
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalRecords = pagedResponse.TotalRecords,
                TotalPages = pagedResponse.TotalPages
            };
        }
    }
}
