using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// �q�����ܼƩαK�X�޲z����� Azure App Configuration �s���r�Ŧ�
string? appConfigConnectionString = builder.Configuration["AppConfig:ConnectionString"];
if (string.IsNullOrEmpty(appConfigConnectionString))
{
    throw new InvalidOperationException("AppConfig:ConnectionString is not configured properly.");
}

// �K�[ Azure App Configuration �ñҥΰʺA�t�m��s
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(appConfigConnectionString)
           .UseFeatureFlags(featureOptions =>
           {
               featureOptions.SetRefreshInterval(TimeSpan.FromSeconds(30)); // �C 30 ���s�@���\��лx���A
           });
});

// �]�m Feature Management
builder.Services.AddFeatureManagement();

// �t�m���
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
