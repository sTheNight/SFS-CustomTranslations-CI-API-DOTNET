using dotenv.net;
using System.Net.Http.Headers;
using SFSCtinstallerAPI.Configuration;
using SFSCtinstallerAPI.Utils;
using System.Text;

static bool LoadDotEnv() {
    try {
        // 加载 .env 文件，如果存在的话
        DotEnv.Load();
        // 检查必要的环境变量是否存在
        return !(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_TOKEN")) ||
                 string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_REPO_OWNER")) ||
                 string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_REPO_NAME")));
    } catch (Exception ex) {
        Console.Error.WriteLine($"Error loading environment variables: {ex.Message}");
        return false;
    }
}

if (!LoadDotEnv()) {
    Console.Error.WriteLine("Error: Required GitHub environment variables are not configured. Application exit.");
    return;
}

var builder = WebApplication.CreateBuilder(args);
// 注册 DI 服务
builder.Services.Configure<GitHubSettings>(builder.Configuration);
builder.Services.AddHttpClient<ApiHelper>(client => {
    // 为 ApiHelper 配置 HttpClient
    var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    client.DefaultRequestHeaders.Add("User-Agent", "Github-REST-API-Test");
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 配置 CORS 规则
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();
// 配置 Swagger 中间件，虽然我没用上
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
// 使用 CORS 规则
app.UseCors("AllowAll");
app.MapControllers();

app.MapGet("/", () => {
    return Results.Content("Hello World!", "text/plain", Encoding.UTF8);
});

app.Run();