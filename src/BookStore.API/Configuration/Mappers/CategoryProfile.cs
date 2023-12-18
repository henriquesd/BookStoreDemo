using AutoMapper;
using BookStore.API.Dtos.Category;
using BookStore.Domain.Models;
using static BookStore.API.Dtos.PaginationDto;

namespace BookStore.API.Configuration.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryAddDto>().ReverseMap();
            CreateMap<Category, CategoryEditDto>().ReverseMap();
            CreateMap<Category, CategoryResultDto>().ReverseMap();
        
            CreateMap<Pagination.PagedResponse<Category>, PagedResponseDto<CategoryResultDto>>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));
        }
    }
}