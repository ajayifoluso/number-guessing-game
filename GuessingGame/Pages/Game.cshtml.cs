using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class GameModel : PageModel
{
    private readonly IConfiguration _configuration;

    public GameModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public int? Guess { get; set; }
    
    public int Attempts { get; set; }
    public int MaxAttempts { get; set; }
    public int Score { get; set; }
    public string Message { get; set; } = "";
    public bool GameWon { get; set; }
    public bool GameOver { get; set; }
    public bool DatabaseConnected { get; set; }
    public bool KeyVaultConnected { get; set; }

    public void OnGet()
    {
        InitializeGame();
    }

    public IActionResult OnPost()
    {
        LoadGameState();
        
        if (Guess.HasValue)
        {
            ProcessGuess(Guess.Value);
        }
        
        return Page();
    }

    private void InitializeGame()
    {
        if (HttpContext.Session.GetInt32("Target") == null)
        {
            var random = new Random();
            HttpContext.Session.SetInt32("Target", random.Next(1, 101));
            HttpContext.Session.SetInt32("Attempts", 0);
            HttpContext.Session.SetInt32("Score", 1000);
        }
        
        LoadGameState();
    }

    private void LoadGameState()
    {
        MaxAttempts = int.Parse(_configuration["MaxAttemptsPerGame"] ?? "5");
        Attempts = HttpContext.Session.GetInt32("Attempts") ?? 0;
        Score = HttpContext.Session.GetInt32("Score") ?? 1000;
        
        // Check connections
        DatabaseConnected = !string.IsNullOrEmpty(_configuration["DatabaseConnectionString"]);
        KeyVaultConnected = !string.IsNullOrEmpty(_configuration["GameApiKey"]);
        
        GameOver = Attempts >= MaxAttempts;
        GameWon = HttpContext.Session.GetString("Won") == "true";
    }

    private void ProcessGuess(int guess)
    {
        if (GameOver || GameWon) return;

        var target = HttpContext.Session.GetInt32("Target") ?? 50;
        Attempts++;
        HttpContext.Session.SetInt32("Attempts", Attempts);

        var scoreMultiplier = int.Parse(_configuration["ScoreMultiplier"] ?? "50");
        Score = Math.Max(0, Score - (scoreMultiplier / 10));
        HttpContext.Session.SetInt32("Score", Score);

        if (guess == target)
        {
            Message = $"Congratulations! You guessed it! The number was {target}.";
            GameWon = true;
            HttpContext.Session.SetString("Won", "true");
        }
        else if (guess < target)
        {
            Message = "Too low! Try a higher number.";
        }
        else
        {
            Message = "Too high! Try a lower number.";
        }

        if (Attempts >= MaxAttempts && !GameWon)
        {
            Message = $"Game over! The number was {target}. Better luck next time!";
            GameOver = true;
        }
    }

    public IActionResult OnPostNewGame()
    {
        HttpContext.Session.Clear();
        return RedirectToPage();
    }
}