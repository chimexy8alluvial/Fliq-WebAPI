using Fliq.Api;
using Fliq.Api.Common.Middlewares;
using Fliq.Application;
using Fliq.Application.Common.Hubs;
using Fliq.Infrastructure;
using Fliq.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using NLog;
using Quartz;
using Quartz.Impl;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddApplication();
    builder.Services.AddPresentation();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddScoped<SuperAdminSeeder>();
    builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
    // Configure NLog
    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fliq", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference=new OpenApiReference{
                       Type=ReferenceType.SecurityScheme,
                       Id="Bearer"
                    }
                },
                  new string[]{}
            }
        });
    });
}

var app = builder.Build();
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var superAdminSeeder = services.GetRequiredService<SuperAdminSeeder>();
        await superAdminSeeder.SeedSuperAdmin(); //Seed super Admin
    }

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FliqDbContext>();
        CurrencySeeder.Seed(dbContext);
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fliq API v1");
    });
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseActivityTrackingMiddleware();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<GameHub>("/gamehub");
    app.MapHub<BlindDateHub>("/blinddatehub");

    app.Run();
}