using ConnectVibe.Api;
using ConnectVibe.Api.Data;
using ConnectVibe.Application;
using ConnectVibe.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddApplication();
    builder.Services.AddPresentation();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddDbContext<ConnectVibeApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectVibeApiContext") ?? throw new InvalidOperationException("Connection string 'ConnectVibeApiContext' not found.")));

}


var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConnectVibe API v1");
        });
    }
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseRouting();
    app.MapControllers();

    app.Run();
}


