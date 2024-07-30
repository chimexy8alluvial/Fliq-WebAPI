using ConnectVibe.Api.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDbContext<ConnectVibeApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectVibeApiContext") ?? throw new InvalidOperationException("Connection string 'ConnectVibeApiContext' not found.")));

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();

}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConnectVibe API v1");
    });
}
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();
