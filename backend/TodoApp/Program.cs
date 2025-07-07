using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using TodoApp.Application.Services;
using TodoApp.Core.Interfaces;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<ITodosRepository, TodosRepository>();
            builder.Services.AddSingleton<TodosService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Enable CORS for any origin (for cloud deployment)
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Use the correct path for the frontend directory (solution root + /frontend)
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(); // Enable CORS middleware
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
