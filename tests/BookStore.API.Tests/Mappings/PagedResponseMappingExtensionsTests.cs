using AutoFixture;
using BookStore.API.Mappings;
using BookStore.API.Tests.Helpers;
using BookStore.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BookStore.API.Tests.Mappings
{
    public class PagedResponseMappingExtensionsTests
    {
        private readonly Fixture _fixture;

        public PagedResponseMappingExtensionsTests()
        {
            _fixture = FixtureFactory.Create();
        }

        [Fact]
        public void PagedResponseBook_ToDto_ShouldMapCorrectly()
        {
            // Arrange
            var books = _fixture
                .Build<Book>()
                .With(b => b.Category, _fixture.Create<Category>())
                .CreateMany(5)
                .ToList();

            var pagedResponse = new PagedResponse<Book>(books, 1, 10, 5);

            // Act
            var dto = pagedResponse.ToDto(b => b.ToDto());

            // Assert
            dto.Should().NotBeNull();
            dto.HasValue.Should().BeTrue();
            dto.Value.Data.Should().HaveCount(5);
            dto.Value.PageNumber.Should().Be(1);
            dto.Value.PageSize.Should().Be(10);
            dto.Value.TotalRecords.Should().Be(5);
            dto.Value.TotalPages.Should().Be(pagedResponse.TotalPages);
            dto.Value.Data.First().Id.Should().Be(books.First().Id);
        }

        [Fact]
        public void PagedResponseCategory_ToDto_ShouldMapCorrectly()
        {
            // Arrange
            var categories = _fixture.CreateMany<Category>(3).ToList();
            var pagedResponse = new PagedResponse<Category>(categories, 2, 5, 3);

            // Act
            var dto = pagedResponse.ToDto(c => c.ToDto());

            // Assert
            dto.Should().NotBeNull();
            dto.HasValue.Should().BeTrue();
            dto.Value.Data.Should().HaveCount(3);
            dto.Value.PageNumber.Should().Be(2);
            dto.Value.PageSize.Should().Be(5);
            dto.Value.TotalRecords.Should().Be(3);
            dto.Value.TotalPages.Should().Be(pagedResponse.TotalPages);
            dto.Value.Data.First().Id.Should().Be(categories.First().Id);
        }

        [Fact]
        public void PagedResponse_ToDto_WithNullData_ShouldMapCorrectly()
        {
            // Arrange
            var pagedResponse = new PagedResponse<Book>(null, 1, 10, 0);

            // Act
            var dto = pagedResponse.ToDto(b => b.ToDto());

            // Assert
            dto.Should().NotBeNull();
            dto.HasValue.Should().BeTrue();
            dto.Value.Data.Should().BeNull();
            dto.Value.PageNumber.Should().Be(1);
            dto.Value.PageSize.Should().Be(10);
            dto.Value.TotalRecords.Should().Be(0);
        }

        [Fact]
        public void PagedResponse_ToDto_WithEmptyList_ShouldMapCorrectly()
        {
            // Arrange
            var books = new List<Book>();
            var pagedResponse = new PagedResponse<Book>(books, 1, 10, 0);

            // Act
            var dto = pagedResponse.ToDto(b => b.ToDto());

            // Assert
            dto.Should().NotBeNull();
            dto.HasValue.Should().BeTrue();
            dto.Value.Data.Should().BeEmpty();
            dto.Value.PageNumber.Should().Be(1);
            dto.Value.PageSize.Should().Be(10);
            dto.Value.TotalRecords.Should().Be(0);
        }
    }
}
