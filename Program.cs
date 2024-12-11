using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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

app.UseHttpsRedirection();
app.MapControllers();
app.Run();


