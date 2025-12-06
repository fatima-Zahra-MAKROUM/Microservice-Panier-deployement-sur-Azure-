using CartMicroservice.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Ajout des contrôleurs API (REST)
builder.Services.AddControllers();

// Configuration de la connexion Redis (via appsettings.json ou variable d'environnement)
var redisConnection = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";

// Injection du client Redis comme singleton (une seule connexion partagée)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConnection);
    config.AbortOnConnectFail = false; // Ne plante pas l'app si Redis est down au démarrage
    return ConnectionMultiplexer.Connect(config);
});

// Service métier du panier
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// Active les routes des contrôleurs ([ApiController])
app.MapControllers();

// Test de connexion Redis au démarrage (pour logs clairs dans Docker/K8s)
try
{
    await app.Services.GetRequiredService<IConnectionMultiplexer>()
        .GetDatabase().PingAsync();
    Console.WriteLine("Redis connecté");
}
catch (Exception ex)
{
    Console.WriteLine($"Redis: {ex.Message}");
}

app.Run();