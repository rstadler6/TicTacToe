using Microsoft.AspNetCore.Mvc;
using TicTacToeService;

namespace TicTacToeApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AIController : ControllerBase
    {
        private readonly ILogger<AIController> _logger;

        public AIController(ILogger<AIController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "MakeMove")]
        public void MakeMove([FromBody]int field)
        {
            //TicTacToeService.TicTacToeService.MakeMove(field);
        }
    }
}