using GameOfLife.Application.Services;
using GameOfLife.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<BoardContext>("GameOfLife");

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBoardService, BoardService>();

//add logging
builder.Services.AddLogging(logging =>
{
  logging.AddConsole();
  logging.AddFilter("Default", LogLevel.Information);
  logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning); //Reduce noise level from ASP.NET core
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  using (var scope = app.Services.CreateScope())
  {
    var dbContext = scope.ServiceProvider.GetRequiredService<BoardContext>();
    dbContext.Database.EnsureCreated();
  }
  app.UseSwagger();
  app.UseSwaggerUI();
}
else
{
  app.UseExceptionHandler("/error", createScopeForErrors: true);
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


