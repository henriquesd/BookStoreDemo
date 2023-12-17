using AutoMapper;
using BookStore.API.Dtos.Book;
using BookStore.Domain.Models;

namespace BookStore.API.Configuration.Mappers
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookAddDto>().ReverseMap();
            CreateMap<Book, BookEditDto>().ReverseMap();
            CreateMap<Book, BookResultDto>().ReverseMap();
        }
    }
}