// Program.cs
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// --- 1) Load Key Vault into configuration (works in ANY environment) ---
string? vaultUrl =
    builder.Configuration["KeyVault:Url"] ??
    builder.Configuration["KeyVault__Url"]; // supports env-var style

if (!string.IsNullOrWhiteSpace(vaultUrl))
{
    // DefaultAzureCredential order (on App Service):
    //  - Uses AZURE_CLIENT_ID if you attached a User-Assigned MI
    //  - Otherwise uses the System-Assigned MI
    //  - Falls back to other credentials locally (Developer CLI, Visual Studio, az login)
    builder.Configuration.AddAzureKeyVault(new Uri(vaultUrl), new DefaultAzureCredential());
}

// --- 2) Services ---
builder.Services.AddRazorPages();
builder.Services.AddSession();

// If you later add a DB, you can read a connection string like this:
// var conn = builder.Configuration.GetConnectionString("DefaultConnection")
//           ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
//           ?? builder.Configuration["DatabaseConnectionString"]; // if you kept it as a KV secret only
// builder.Services.AddDbContext<YourDbContext>(opt => opt.UseSqlServer(conn));

var app = builder.Build();

// --- 3) Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

// --- 4) Routes / Endpoints ---
app.MapRazorPages();

// Keep your redirect to the Game page
app.MapGet("/", () => Results.Redirect("/Game"));

// Simple liveness probe
app.MapGet("/healthz", () => Results.Ok("OK"));

// Key Vault diagnostics endpoint (remove in production if you prefer)
app.MapGet("/kv-check", (IConfiguration cfg) =>
{
    // Try read a couple of known secrets you created
    var maxAttempts = cfg["MaxAttemptsPerGame"];   // from KV secret
    var scoreMult   = cfg["ScoreMultiplier"];      // from KV secret
    var kvUrlSeen   = cfg["KeyVault:Url"] ?? cfg["KeyVault__Url"];

    // Mask values (don’t print secrets in plain text)
    string Mask(string? v) => string.IsNullOrEmpty(v) ? "(null)" : $"len={v.Length}";

    return Results.Ok(new
    {
        VaultUrlInConfig = kvUrlSeen ?? "(not set)",
        MaxAttemptsPerGame = Mask(maxAttempts),
        ScoreMultiplier     = Mask(scoreMult),
        LoadedFromKeyVault  = !string.IsNullOrEmpty(maxAttempts) || !string.IsNullOrEmpty(scoreMult)
    });
});

app.Run();
