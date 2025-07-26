using Microsoft.AspNetCore.Mvc;
using bgbahasajerman_DataAccessLibrary.DataAccess;

namespace bgbahasajerman_DataAccessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockController : ControllerBase
    {
        private readonly IMockRepository _mockRepository;

        public MockController(IMockRepository mockRepository)
        {
            _mockRepository = mockRepository;
        }

        [HttpGet("greeting")]
        public async Task<ActionResult<string>> GetGreeting()
        {
            var result = await _mockRepository.GetGreetingAsync();
            return Ok(result);
        }
    }
}