namespace ExpenseTracker.Common.Constants
{
    public class ExpenseDataMessage
    {

        // -------------- Success Messages --------------

        public const string ExpenseCreatedSuccess = "Expense created successfully.";

        public const string ExpenseEditedSuccess = "Expense updated successfully.";

        public const string ExpenseDeletedSuccess = "Expense deleted successfully.";

        // ------------------------------------------------

        // -------------- Error Messages --------------

        public const string UserDoesNotExistError = "User does not exist.";

        public const string CategoryDoesNotExistError = "Category does not exist.";

        public const string MiscellaneousCategoryDoesNotExistError = "Miscellaneous category does not exist.";

        public const string ExpenseLessThanZeroError = "Expense amount must be greater than 0.";

        public const string ExpenseDateRequireError = "Expense date is required.";

        public const string ExpenseDateInFutureError = "Expense date cannot be in the future.";

        // ------------------------------------------------
    }
}
