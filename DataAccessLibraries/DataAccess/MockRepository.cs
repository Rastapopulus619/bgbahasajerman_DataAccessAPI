namespace bgbahasajerman_DataAccessLibrary.DataAccess
{
    public class MockRepository : IMockRepository
    {
        public Task<string> GetGreetingAsync()
        {
            // Hard-coded value, simulating a query result
            return Task.FromResult("Hello from the mock repository!");
        }
    }
}