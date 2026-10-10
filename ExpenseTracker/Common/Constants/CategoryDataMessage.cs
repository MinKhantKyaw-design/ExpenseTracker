namespace ExpenseTracker.Common.Constants
{
    public class CategoryDataMessage
    {
        // -------------- Success Messages --------------

        public const string CategoryCreatedSuccess = "Category created successfully.";

        public const string CategoryEditedSuccess = "Category updated successfully.";

        public const string CategoryDeletedSuccess = "Category deleted successfully.";

        // ------------------------------------------------

        // -------------- Error Messages --------------

        public const string CategoryNameRequiredError = "Category name is required.";

        public const string CategoryNameExistError = "A category with this name already exists.";

        public const string MiscellaneousCategoryDeleteError = "The Miscellaneous category cannot be deleted.";

        public const string DeleteHasExpenseError = "Cannot delete a category that has expenses.";

        // ------------------------------------------------
    }
}
