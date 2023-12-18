using AutoMapper;
using BookStore.API.Dtos.Book;
using BookStore.Domain.Models;
using static BookStore.API.Dtos.PaginationDto;

namespace BookStore.API.Configuration.Mappers
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookAddDto>().ReverseMap();
            CreateMap<Book, BookEditDto>().ReverseMap();
            CreateMap<Book, BookResultDto>().ReverseMap();

            CreateMap<Pagination.PagedResponse<Book>, PagedResponseDto<BookResultDto>>()
               .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));
        }
    }
}