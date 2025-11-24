using dotenv.net;
using SFSCtinstallerAPI;

if (!LoadDotEnv()) {
    Console.WriteLine("Error: Required environment variables are not configured.");
    return;
}

Console.WriteLine("Program is running");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
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

static bool LoadDotEnv() {
    try {
        DotEnv.Load();
        Global.GITHUB_REPO_NAME = Environment.GetEnvironmentVariable("GITHUB_REPO_NAME");
        Global.GITHUB_REPO_OWNER = Environment.GetEnvironmentVariable("GITHUB_REPO_OWNER");
        Global.GITHUB_TOKEN = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        if (string.IsNullOrEmpty(Global.GITHUB_TOKEN) ||
            string.IsNullOrEmpty(Global.GITHUB_REPO_OWNER) ||
            string.IsNullOrEmpty(Global.GITHUB_REPO_NAME)) {
            return false;
        }
        return true;
    } catch (Exception ex) {
        Console.WriteLine($"Error loading environment variables: {ex.Message}");
        return false;
    }
}