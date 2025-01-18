using StatsService.Services;
using StatsService.Models;
using StatsService.Models.DTO;
using StatsService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure HttpClient
builder.Services.AddHttpClient("URLStatsService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5240/"); 
});

builder.Services.AddScoped<URLStatsService>();
builder.Services.AddScoped<IBitlyClickCountService, BitlyClickCountService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// RecurringJob.AddOrUpdate(
//     "FetchBitlyClickCounts",
//     () => serviceProvider.GetRequiredService<IBitlyClickCountService>().GetClickCountForBitlink(bitlinkUrl), 
//     cronExpression: "0 */1 * * *"
//     ); // Every hour (adjust as needed)    

app.Run();
