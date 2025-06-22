using GameService.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Bind connection string
var connStr = builder.Configuration.GetConnectionString("GameStore");

// Register DbContext with MySQL
builder.Services.AddDbContext<GameDbContext>(opts =>
    opts.UseMySQL(connStr, mySqlOpts =>
    {
        // any MySQL-specific options, e.g. CommandTimeout:
        mySqlOpts.CommandTimeout(60);
    })
);

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddDbContextCheck<GameDbContext>(
        name: "mysql-readiness",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "ready" }
    );


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGameService, GameService.Infrastructure.Services.GameService>();

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    db.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("ready")
});

app.Run();
