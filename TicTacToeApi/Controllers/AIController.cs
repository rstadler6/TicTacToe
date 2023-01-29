using Microsoft.AspNetCore.Mvc;

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
        public void MakeMove([FromBody] MoveResponse move)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TicTacToe\\Moves\\" + move.Move;
            var file = System.IO.File.CreateText(path);
            file.Write(move.Field);
            file.Close();
        }
    }

    public class MoveResponse
    {
        public int Field { get; set; }
        public int Move { get; set; }
    }
}