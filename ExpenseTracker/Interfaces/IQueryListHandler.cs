namespace ExpenseTracker.Interfaces
{
    public interface IQueryListHandler <TResult>
    {
        Task<TResult> HandleListAsync();
    }
}
