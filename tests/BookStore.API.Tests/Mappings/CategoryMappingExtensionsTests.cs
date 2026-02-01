using AutoFixture;
using BookStore.API.Dtos.Category;
using BookStore.API.Mappings;
using BookStore.API.Tests.Helpers;
using BookStore.Domain.Models;
using FluentAssertions;
using Xunit;

namespace BookStore.API.Tests.Mappings
{
    public class CategoryMappingExtensionsTests
    {
        private readonly Fixture _fixture;

        public CategoryMappingExtensionsTests()
        {
            _fixture = FixtureFactory.Create();
        }

        public class ToModel : CategoryMappingExtensionsTests
        {
            [Fact]
            public void CategoryAddDto_ToModel_ShouldMapCorrectly()
            {
                // Arrange
                var dto = _fixture.Create<CategoryAddDto>();

                // Act
                var model = dto.ToModel();

                // Assert
                model.Should().NotBeNull();
                model.Name.Should().Be(dto.Name);
            }

            [Fact]
            public void CategoryAddDto_ToModel_WithNull_ShouldReturnNull()
            {
                // Arrange
                CategoryAddDto dto = null;

                // Act
                var model = dto.ToModel();

                // Assert
                model.Should().BeNull();
            }

            [Fact]
            public void CategoryEditDto_ToModel_ShouldMapCorrectly()
            {
                // Arrange
                var dto = _fixture.Create<CategoryEditDto>();

                // Act
                var model = dto.ToModel();

                // Assert
                model.Should().NotBeNull();
                model.Id.Should().Be(dto.Id);
                model.Name.Should().Be(dto.Name);
            }

            [Fact]
            public void CategoryEditDto_ToModel_WithNull_ShouldReturnNull()
            {
                // Arrange
                CategoryEditDto dto = null;

                // Act
                var model = dto.ToModel();

                // Assert
                model.Should().BeNull();
            }
        }

        public class ToDto : CategoryMappingExtensionsTests
        {
            [Fact]
            public void Category_ToDto_ShouldMapCorrectly()
            {
                // Arrange
                var model = _fixture.Create<Category>();

                // Act
                var dto = model.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Id.Should().Be(model.Id);
                dto.Name.Should().Be(model.Name);
            }

            [Fact]
            public void Category_ToDto_WithNull_ShouldReturnNull()
            {
                // Arrange
                Category model = null;

                // Act
                var dto = model.ToDto();

                // Assert
                dto.Should().BeNull();
            }

            [Fact]
            public void CategoryList_ToDto_ShouldMapCorrectly()
            {
                // Arrange
                var models = _fixture.CreateMany<Category>(3).ToList();

                // Act
                var dtos = models.ToDto().ToList();

                // Assert
                dtos.Should().NotBeNull();
                dtos.Should().HaveCount(3);
                dtos[0].Id.Should().Be(models[0].Id);
                dtos[0].Name.Should().Be(models[0].Name);
                dtos[1].Id.Should().Be(models[1].Id);
                dtos[1].Name.Should().Be(models[1].Name);
                dtos[2].Id.Should().Be(models[2].Id);
                dtos[2].Name.Should().Be(models[2].Name);
            }

            [Fact]
            public void CategoryList_ToDto_WithNull_ShouldReturnNull()
            {
                // Arrange
                IEnumerable<Category> models = null;

                // Act
                var dtos = models.ToDto();

                // Assert
                dtos.Should().BeNull();
            }

            [Fact]
            public void OperationResultCategory_ToDto_ShouldMapCorrectly()
            {
                // Arrange
                var category = _fixture.Create<Category>();
                var operationResult = new OperationResult<Category>(category, true, "Success");

                // Act
                var dto = operationResult.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Success.Should().BeTrue();
                dto.Message.Should().Be("Success");
                dto.Payload.Should().NotBeNull();
                dto.Payload.Id.Should().Be(category.Id);
                dto.Payload.Name.Should().Be(category.Name);
            }

            [Fact]
            public void OperationResultCategory_ToDto_WithNullPayload_ShouldMapCorrectly()
            {
                // Arrange
                var operationResult = new OperationResult<Category>(false, "Error message");

                // Act
                var dto = operationResult.ToDto();

                // Assert
                dto.Should().NotBeNull();
                dto.Success.Should().BeFalse();
                dto.Message.Should().Be("Error message");
                dto.Payload.Should().BeNull();
            }

            [Fact]
            public void OperationResultCategory_ToDto_WithNull_ShouldReturnNull()
            {
                // Arrange
                OperationResult<Category> operationResult = null;

                // Act
                var dto = operationResult.ToDto();

                // Assert
                dto.Should().BeNull();
            }
        }
    }
}
