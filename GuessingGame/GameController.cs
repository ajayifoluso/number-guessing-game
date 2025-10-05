using Microsoft.AspNetCore.Mvc;

namespace GuessingGame.Controllers
{
    public class GameController : Controller
    {
        private readonly IConfiguration _config;
        
        public GameController(IConfiguration config)
        {
            _config = config;
        }
        
        public IActionResult Index()
        {
            var maxAttempts = int.Parse(_config["MaxAttemptsPerGame"] ?? "10");
            var scoreMultiplier = int.Parse(_config["ScoreMultiplier"] ?? "100");
            
            ViewBag.MaxAttempts = maxAttempts;
            ViewBag.ScoreMultiplier = scoreMultiplier;
            
            return View();
        }
        
        [HttpPost]
        public IActionResult MakeGuess([FromBody] GuessRequest request)
        {
            var maxAttempts = int.Parse(_config["MaxAttemptsPerGame"] ?? "10");
            var scoreMultiplier = int.Parse(_config["ScoreMultiplier"] ?? "100");
            
            if (request.Guess == request.Target)
            {
                var score = (maxAttempts - request.Attempts + 1) * scoreMultiplier;
                return Json(new { success = true, message = "Correct! You win!", score = score });
            }
            else if (request.Guess < request.Target)
            {
                return Json(new { success = false, message = "Too low! Try higher." });
            }
            else
            {
                return Json(new { success = false, message = "Too high! Try lower." });
            }
        }
    }
    
    public class GuessRequest
    {
        public int Guess { get; set; }
        public int Target { get; set; }
        public int Attempts { get; set; }
    }
}