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

            var app = builder.Build();

            // Force the backend to listen on port 8000 (or $PORT if set) so it matches the frontend's API URL.
            // var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
            // app.Urls.Clear();
            // app.Urls.Add($"http://localhost:{port}");

            // Use the correct path for the frontend directory (solution root + /frontend)
            var frontendPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            if (Directory.Exists(frontendPath))
            {
                app.UseDefaultFiles(new DefaultFilesOptions
                {
                    FileProvider = new PhysicalFileProvider(frontendPath),
                    DefaultFileNames = new List<string> { "index.html" }
                });
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(frontendPath)
                });
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
