using System.Text;
using DotNetEnv;
using Imdb.Repositories;
using Imdb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// CORS Policy tanımı
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

Env.Load();
var secretKey = Env.GetString("JWT_SECRET_KEY");

// JWT Ayarları
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"], 
        ValidAudience = jwtSettings["Audience"], 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
// Veritabanı bağlantı ayarlarını doğru şekilde yapılandırın
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// FilmRepository'yi başlatın
var filmRepository = new FilmRepository(configuration);

// GetAllFilms fonksiyonunu çağırın ve sonuçları yazdırın
try
{
    var films = filmRepository.GetAllFilms();

    foreach (var film in films)
    {
        Console.WriteLine($"Film Adı: {film.FilmName}");
        Console.WriteLine($"Açıklama: {film.FilmDescription}");
        Console.WriteLine($"Türler: {string.Join(", ", film.Genres?.Select(g => g.GenreName) ?? new List<string>())}");
        Console.WriteLine($"Oyuncular: {string.Join(", ", film.Actors?.Select(a => a.ActorName) ?? new List<string>())}");
        Console.WriteLine($"Yayın Tarihi: {film.FilmReleaseDate?.ToString("yyyy-MM-dd")}");
        Console.WriteLine($"Süre: {film.FilmDuration} dk");
        Console.WriteLine($"Poster Yolu: {film.PosterPath}");
        Console.WriteLine("--------------------------");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Bir hata oluştu: {ex.Message}");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting(); // Routing middleware

// CORS middleware (Routing'den hemen sonra olmalı)
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();