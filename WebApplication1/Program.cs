using dotenv.net;
using SFSCtinstallerAPI;
using Microsoft.Extensions.DependencyInjection;
namespace SFSCtinstallerAPI {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            if (!LoadDotEnv()) {
                Console.WriteLine("enviroment variable is not config");
                return;
            }
            Console.WriteLine("Program is loading");

            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", policy => {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
        public static bool LoadDotEnv() {
            try {
                DotEnv.Load();
                Global.GITHUB_REPO_NAME = Environment.GetEnvironmentVariable("GITHUB_REPO_NAME");
                Global.GITHUB_REPO_OWNER = Environment.GetEnvironmentVariable("GITHUB_REPO_OWNER");
                Global.GITHUB_TOKEN = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
                if (String.IsNullOrEmpty(Global.GITHUB_TOKEN)
                    || String.IsNullOrEmpty(Global.GITHUB_REPO_OWNER)
                    || String.IsNullOrEmpty(Global.GITHUB_REPO_NAME)) {
                    return false;
                }
                return true;
            } catch (Exception) {
                return false;
                throw;
            }
        }
    }
}
