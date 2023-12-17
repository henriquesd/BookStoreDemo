using AutoFixture;
using BookStore.API.Configuration.Mappers;
using BookStore.API.Dtos.Category;
using BookStore.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BookStore.API.Tests.Mappers
{
    public class CategoryProfileTests
    {
        public abstract class CategoryProfileTestsBase : ProfileTestsBase<CategoryProfile>
        {
        }

        public sealed class Model_To_Dto
        {
            public class Category_To_CategoryAddDto : CategoryProfileTestsBase
            {
                [Fact]
                public void ShouldMapCorrectly()
                {
                    // Arrange
                    var source = _fixture.Create<CategoryAddDto>();

                    // Act
                    var destination = _mapper.Map<Category>(source);

                    // Assert
                    destination.Should().NotBeNull();
                    destination.Name.Should().Be(source.Name);
                }
            }

            public class Category_To_CategoryEditDto : CategoryProfileTestsBase
            {
                [Fact]
                public void ShouldMapCorrectly()
                {
                    // Arrange
                    var source = _fixture.Create<CategoryEditDto>();

                    // Act
                    var destination = _mapper.Map<Category>(source);

                    // Assert
                    destination.Should().NotBeNull();
                    destination.Id.Should().Be(source.Id);
                    destination.Name.Should().Be(source.Name);
                }
            }

            public class Category_To_CategoryResultDto : CategoryProfileTestsBase
            {
                [Fact]
                public void ShouldMapCorrectly()
                {
                    // Arrange
                    var source = _fixture.Create<CategoryResultDto>();

                    // Act
                    var destination = _mapper.Map<Category>(source);

                    // Assert
                    destination.Should().NotBeNull();
                    destination.Id.Should().Be(source.Id);
                    destination.Name.Should().Be(source.Name);
                }
            }
        }
    }
}
