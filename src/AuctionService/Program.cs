using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // Creates a WebApplicationBuilder instance to configure the web application, using command-line arguments

// Add services to the container. // Comment indicating the section for adding services to the DI container

builder.Services.AddControllers(); // Adds support for MVC controllers to the services collection, enabling controller-based routing
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
  opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")); // Configures the AuctionDbContext to use PostgreSQL with the connection string from configuration
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build(); // Builds the WebApplication from the builder, finalizing the service configuration and middleware pipeline

app.UseHttpsRedirection(); // Adds middleware to redirect HTTP requests to HTTPS for secure communication

app.UseAuthorization(); // Adds authorization middleware to the request pipeline to enforce access control based on user permissions

app.MapControllers(); // Maps the routes defined in controllers to the application, enabling endpoint routing for HTTP requests

try
{
  DbInitializer.InitDb(app); // Initializes the database by applying migrations and seeding initial data
}
catch (Exception ex)
{
  Console.WriteLine($"Error during DB initialization: {ex.Message}"); // Logs any exceptions that occur during database initialization
}
app.Run(); // Starts the web application and begins listening for incoming HTTP requests on the configured port
