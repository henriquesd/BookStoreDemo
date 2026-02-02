namespace BookStore.Domain.Constants
{
    public static class ErrorMessages
    {
        public const string BookNotFound = "Book with ID {0} not found";
        public const string BookDuplicate = "A book with this name already exists";
        public const string BookDuplicateOnUpdate = "Another book with this name already exists";
        public const string BookAddError = "An error occurred while adding the book: {0}";
        public const string BookUpdateError = "An error occurred while updating the book: {0}";
        public const string BookRemoveError = "An error occurred while removing the book: {0}";

        public const string CategoryNotFound = "Category with ID {0} not found";
        public const string CategoryDuplicate = "This category name is already being used";
        public const string CategoryHasDependencies = "Cannot delete category with associated books";
        public const string CategoryAddError = "An error occurred while adding the category: {0}";
        public const string CategoryUpdateError = "An error occurred while updating the category: {0}";
        public const string CategoryRemoveError = "An error occurred while removing the category: {0}";

        public const string EntityCannotBeNull = "{0} cannot be null";
        public const string FieldRequired = "{0} is required";
        public const string InvalidId = "Invalid {0} ID";
    }
}
