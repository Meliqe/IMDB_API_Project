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
builder.Services.AddScoped<FilmRepository>();
builder.Services.AddScoped<FilmServices>();
builder.Services.AddScoped<AdminServices>();
builder.Services.AddScoped<AdminRepository>();

//jwt doğrulama mekanizması
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options => //token doğrulama parametreleri
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //tokenı oluşturan uygulama doğru mu
        ValidateAudience = true,//tokenın hedef kitlesi doğru mu
        ValidateLifetime = true, //tokenın süresi dolmuş mu 
        ValidateIssuerSigningKey = true, //token bir secret key ile imzalanmış mı
        ValidIssuer = jwtSettings["Issuer"], 
        ValidAudience = jwtSettings["Audience"], 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" 
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context => //Token doğrulama işlemi başarısız olduğunda tetiklenir. 
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context => //Token doğrulama işlemi başarılı olduğunda tetiklenir.
        {
            Console.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
});
// Veritabanı bağlantı ayarlarını doğru şekilde yapılandırın
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// FilmRepository'yi başlatın
var filmRepository = new FilmRepository(configuration);

// GetAllFilms fonksiyonunu çağırın ve sonuçları yazdırın

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