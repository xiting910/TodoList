using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoList.API;

const string allowedOrigins = "http://localhost:5173";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy => _ = policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()
));
builder.Services.AddDbContext<TodoDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString(nameof(TodoList)))
);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.Run();
