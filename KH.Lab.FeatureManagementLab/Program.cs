using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 從環境變數或密碼管理器獲取 Azure App Configuration 連接字符串
string? appConfigConnectionString = builder.Configuration["AppConfig:ConnectionString"];
if (string.IsNullOrEmpty(appConfigConnectionString))
{
    throw new InvalidOperationException("AppConfig:ConnectionString is not configured properly.");
}

// 添加 Azure App Configuration 並啟用動態配置刷新
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(appConfigConnectionString)
           .UseFeatureFlags(featureOptions =>
           {
               featureOptions.SetRefreshInterval(TimeSpan.FromSeconds(30)); // 每 30 秒刷新一次功能標誌狀態
           });
});

// 設置 Feature Management
builder.Services.AddFeatureManagement();

// 配置控制器
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
