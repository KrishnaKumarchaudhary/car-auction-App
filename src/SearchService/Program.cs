using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Modals;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
try
{
  await DbInitializer.InitDb(app);
}
catch (Exception ex)
{
  Console.WriteLine($"Error during DB initialization: {ex.Message}");
}
app.Run();
