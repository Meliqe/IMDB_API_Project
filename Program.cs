using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
var secretKey = Env.GetString("JWT_SECRET_KEY");
builder.Services.AddOpenApi();
builder.Services.AddControllers();
//Program.cs kısmında, gelen JWT token'ların doğrulanması için gerekli ayarları yapıyoruz.
var jwtSettings=builder.Configuration.GetSection("JwtSetting");

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
        ValidIssuer = jwtSettings["Issuer"], // appsettings.json'dan alınıyor
        ValidAudience = jwtSettings["Audience"], // appsettings.json'dan alınıyor
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

/*try
{ 
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Bağlantı kuruldu");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Bağlantı kurulamadı:"+ ex.Message);
}*/
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();


