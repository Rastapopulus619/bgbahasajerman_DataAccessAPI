namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    public interface IMockRepository
    {
        Task<string> GetGreetingAsync();
    }
}