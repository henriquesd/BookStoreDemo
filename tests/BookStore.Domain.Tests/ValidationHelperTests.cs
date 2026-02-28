using AutoFixture;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Domain.Services;
using BookStore.Domain.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace BookStore.Domain.Tests
{
    public class ValidationHelperTests
    {
        private readonly Fixture _fixture;

        public ValidationHelperTests()
        {
            _fixture = FixtureFactory.Create();
        }

        public class ValidateNotNull : ValidationHelperTests
        {
            [Fact]
            public void Should_ReturnSuccess_When_EntityIsNotNull()
            {
                // Arrange
                var entity = _fixture.Create<Book>();

                // Act
                var result = ValidationHelper.ValidateNotNull(entity, "Book");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
            }

            [Fact]
            public void Should_ReturnValidationError_When_EntityIsNull()
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateNotNull<Book>(null, "Book");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Book");
                result.Message.Should().Contain("cannot be null");
            }
        }

        public class ValidateRequiredString : ValidationHelperTests
        {
            [Theory]
            [InlineData("Valid String")]
            [InlineData("Another valid string")]
            [InlineData("A")]
            public void Should_ReturnSuccess_When_StringIsValid(string value)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateRequiredString<Book>(value, "Field");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            [InlineData("\t")]
            [InlineData("\n")]
            public void Should_ReturnValidationError_When_StringIsInvalid(string? value)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateRequiredString<Book>(value, "Field");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Field");
                result.Message.Should().Contain("is required");
            }
        }

        public class ValidateId : ValidationHelperTests
        {
            [Theory]
            [InlineData(1)]
            [InlineData(100)]
            [InlineData(int.MaxValue)]
            public void Should_ReturnSuccess_When_IdIsValid(int id)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateId<Book>(id, "book");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-100)]
            [InlineData(int.MinValue)]
            public void Should_ReturnValidationError_When_IdIsInvalid(int id)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateId<Book>(id, "book");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Invalid");
                result.Message.Should().Contain("book");
            }
        }

        public class ValidateIdForRemoval : ValidationHelperTests
        {
            [Theory]
            [InlineData(1)]
            [InlineData(100)]
            [InlineData(int.MaxValue)]
            public void Should_ReturnSuccess_When_IdIsValid(int id)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateIdForRemoval(id, "book");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
            }

            [Theory]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(-100)]
            [InlineData(int.MinValue)]
            public void Should_ReturnValidationError_When_IdIsInvalid(int id)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidateIdForRemoval(id, "book");

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Invalid");
                result.Message.Should().Contain("book");
            }
        }

        public class ValidatePagination : ValidationHelperTests
        {
            [Theory]
            [InlineData(1, 1)]
            [InlineData(1, 10)]
            [InlineData(1, 100)]
            [InlineData(10, 50)]
            [InlineData(int.MaxValue, 100)]
            public void Should_ReturnSuccess_When_PaginationParametersAreValid(int pageNumber, int pageSize)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Book>(pageNumber, pageSize);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.ErrorCode.Should().Be(OperationErrorCode.None);
            }

            [Theory]
            [InlineData(0, 10)]
            [InlineData(-1, 10)]
            [InlineData(-100, 10)]
            [InlineData(int.MinValue, 10)]
            public void Should_ReturnValidationError_When_PageNumberIsInvalid(int pageNumber, int pageSize)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Book>(pageNumber, pageSize);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Page number");
                result.Message.Should().Contain("greater than zero");
            }

            [Theory]
            [InlineData(1, 0)]
            [InlineData(1, -1)]
            [InlineData(1, -100)]
            [InlineData(1, int.MinValue)]
            public void Should_ReturnValidationError_When_PageSizeIsTooSmall(int pageNumber, int pageSize)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Book>(pageNumber, pageSize);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Page size");
                result.Message.Should().Contain("between 1 and 100");
            }

            [Theory]
            [InlineData(1, 101)]
            [InlineData(1, 200)]
            [InlineData(1, 1000)]
            [InlineData(1, int.MaxValue)]
            public void Should_ReturnValidationError_When_PageSizeIsTooLarge(int pageNumber, int pageSize)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Book>(pageNumber, pageSize);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
                result.Message.Should().Contain("Page size");
                result.Message.Should().Contain("between 1 and 100");
            }

            [Theory]
            [InlineData(0, 0)]
            [InlineData(-1, -1)]
            [InlineData(0, 101)]
            [InlineData(-1, 200)]
            public void Should_ReturnValidationError_When_BothParametersAreInvalid(int pageNumber, int pageSize)
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Book>(pageNumber, pageSize);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeFalse();
                result.ErrorCode.Should().Be(OperationErrorCode.ValidationError);
            }

            [Fact]
            public void Should_WorkForDifferentGenericTypes_Book()
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Book>(1, 10);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
            }

            [Fact]
            public void Should_WorkForDifferentGenericTypes_Category()
            {
                // Arrange & Act
                var result = ValidationHelper.ValidatePagination<Category>(1, 10);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
            }
        }
    }
}
