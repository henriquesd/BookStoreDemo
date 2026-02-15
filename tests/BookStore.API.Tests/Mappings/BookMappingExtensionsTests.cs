using AutoFixture;
using BookStore.API.Dtos.Book;
using BookStore.API.Mappings;
using BookStore.API.Tests.Helpers;
using BookStore.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BookStore.API.Tests.Mappings
{
    public class BookMappingExtensionsTests
    {
        private readonly Fixture _fixture;

        public BookMappingExtensionsTests()
        {
            _fixture = FixtureFactory.Create();
        }

        public class ToModel : BookMappingExtensionsTests
        {
            [Fact]
            public void BookAddDto_ToModel_ShouldMapCorrectly()
            {
                // Arrange
                var dto = _fixture.Create<BookAddDto>();

                // Act
                var model = dto.ToModel();

                // Assert
                model.Should().NotBeNull();
                model.Name.Should().Be(dto.Name);
                model.Author.Should().Be(dto.Author);
                model.Description.Should().Be(dto.Description);
                model.Value.Should().Be(dto.Value);
                model.PublishDate.Should().Be(dto.PublishDate);
                model.CategoryId.Should().Be(dto.CategoryId);
            }

            [Fact]
            public void BookAddDto_ToModel_WithNull_ShouldThrowArgumentNullException()
            {
                // Arrange
                BookAddDto? dto = null;

                // Act
                var act = () => dto!.ToModel();

                // Assert
                act.Should().Throw<ArgumentNullException>().WithParameterName("dto");
            }

            [Fact]
            public void BookEditDto_ToModel_ShouldMapCorrectly()
            {
                // Arrange
                var dto = _fixture.Create<BookEditDto>();

                // Act
                var model = dto.ToModel();

                // Assert
                model.Should().NotBeNull();
                model.Id.Should().Be(dto.Id);
                model.Name.Should().Be(dto.Name);
                model.Author.Should().Be(dto.Author);
                model.Description.Should().Be(dto.Description);
                model.Value.Should().Be(dto.Value);
                model.PublishDate.Should().Be(dto.PublishDate);
                model.CategoryId.Should().Be(dto.CategoryId);
            }

            [Fact]
            public void BookEditDto_ToModel_WithNull_ShouldThrowArgumentNullException()
            {
                // Arrange
                BookEditDto? dto = null;

                // Act
                var act = () => dto!.ToModel();

                // Assert
                act.Should().Throw<ArgumentNullException>().WithParameterName("dto");
            }
        }

        public class ToDto : BookMappingExtensionsTests
        {
            [Fact]
            public void Book_ToDto_ShouldMapCorrectly()
            {
                // Arrange
                var model = _fixture.Build<Book>()
                    .With(b => b.Category, _fixture.Create<Category>())
                    .Create();

                // Act
                var dto = model.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Id.Should().Be(model.Id);
                dto.Name.Should().Be(model.Name);
                dto.Author.Should().Be(model.Author);
                dto.Description.Should().Be(model.Description);
                dto.Value.Should().Be(model.Value);
                dto.PublishDate.Should().Be(model.PublishDate);
                dto.CategoryId.Should().Be(model.CategoryId);
                dto.CategoryName.Should().Be(model.Category!.Name);
            }

            [Fact]
            public void Book_ToDto_WithNullCategory_ShouldMapCorrectly()
            {
                // Arrange
                var model = _fixture.Build<Book>()
                    .Without(b => b.Category)
                    .Create();

                // Act
                var dto = model.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Id.Should().Be(model.Id);
                dto.CategoryName.Should().BeNull();
            }

            [Fact]
            public void Book_ToDto_WithNull_ShouldThrowArgumentNullException()
            {
                // Arrange
                Book? model = null;

                // Act
                var act = () => model!.ToDto();

                // Assert
                act.Should().Throw<ArgumentNullException>().WithParameterName("model");
            }

            [Fact]
            public void BookList_ToDto_ShouldMapCorrectly()
            {
                // Arrange
                var models = _fixture.Build<Book>()
                    .With(b => b.Category, _fixture.Create<Category>())
                    .CreateMany(3)
                    .ToList();

                // Act
                var dtos = models.ToDto().ToList();

                // Assert
                dtos.Should().NotBeNull();
                dtos.Should().HaveCount(3);
                dtos[0].Id.Should().Be(models[0].Id);
                dtos[1].Id.Should().Be(models[1].Id);
                dtos[2].Id.Should().Be(models[2].Id);
            }

            [Fact]
            public void BookList_ToDto_WithNull_ShouldThrowArgumentNullException()
            {
                // Arrange
                IEnumerable<Book>? models = null;

                // Act
                var act = () => models!.ToDto();

                // Assert
                act.Should().Throw<ArgumentNullException>().WithParameterName("models");
            }

            [Fact]
            public void OperationResultBook_ToDto_ShouldMapCorrectly()
            {
                // Arrange
                var book = _fixture.Build<Book>()
                    .With(b => b.Category, _fixture.Create<Category>())
                    .Create();
                var operationResult = OperationResult<Book>.SuccessResult(book);

                // Act
                var dto = operationResult.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Success.Should().BeTrue();
                dto.Message.Should().Be("Success");
                dto.Payload.Should().NotBeNull();
                dto.Payload!.Id.Should().Be(book.Id);
                dto.Payload.Name.Should().Be(book.Name);
            }

            [Fact]
            public void OperationResultBook_ToDto_WithNullPayload_ShouldMapCorrectly()
            {
                // Arrange
                var operationResult = OperationResult<Book>.ValidationError("Error message");

                // Act
                var dto = operationResult.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Success.Should().BeFalse();
                dto.Message.Should().Be("Error message");
                dto.Payload.Should().BeNull();
            }

            [Fact]
            public void OperationResultBook_ToDto_WithNull_ShouldThrowArgumentNullException()
            {
                // Arrange
                OperationResult<Book>? operationResult = null;

                // Act
                var act = () => operationResult!.ToDto();

                // Assert
                act.Should().Throw<ArgumentNullException>().WithParameterName("operationResult");
            }
        }
    }
}
