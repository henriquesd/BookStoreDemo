using AutoMapper;
using BookStore.API.Dtos;
using BookStore.API.Dtos.Category;
using BookStore.Domain.Models;

namespace BookStore.API.Configuration.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryAddDto>().ReverseMap();
            CreateMap<Category, CategoryEditDto>().ReverseMap();
            CreateMap<Category, CategoryResultDto>().ReverseMap();
        
            CreateMap<PagedResponse<Category>, PagedResponseDto<CategoryResultDto>>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));
        }
    }
}