using AutoFixture;
using AutoMapper;
using BookStore.API.Tests.Helpers;

namespace BookStore.API.Tests.Mappers
{
    public abstract class ProfileTestsBase<TProfile1, TProfile2>
      where TProfile1 : Profile, new()
      where TProfile2 : Profile, new()
    {
        protected readonly Fixture _fixture;
        protected readonly IMapper _mapper;

        protected ProfileTestsBase(Fixture fixture = null)
        {
            _fixture = fixture ?? FixtureFactory.Create();
            _mapper = new MapperConfiguration(x =>
            {
                x.AddProfile<TProfile1>();
                x.AddProfile<TProfile2>();
            }).CreateMapper();
        }
    }

    public abstract class ProfileTestsBase<T>
        where T : Profile, new()
    {
        protected readonly Fixture _fixture;
        protected readonly IMapper _mapper;

        protected ProfileTestsBase(Fixture fixture = null)
        {
            _fixture = fixture ?? FixtureFactory.Create();
            _mapper = new MapperConfiguration(x => x.AddProfile<T>()).CreateMapper();
        }
    }
}
