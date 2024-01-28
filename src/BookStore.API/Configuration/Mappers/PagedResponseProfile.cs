using AutoMapper;
using BookStore.API.Dtos;
using BookStore.Domain.Models;

namespace BookStore.API.Configuration.Mappers
{
    public class PagedResponseProfile : Profile
    {
        public PagedResponseProfile()
        {
            CreateMap(typeof(PagedResponse<>), typeof(PagedResponseDto<>));
        }
    }
}