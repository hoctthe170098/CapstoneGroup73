using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using StudyFlow.Infrastructure.Data;
using StudyFlow.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
builder.Services.AddSwaggerGen();
builder.Services.AddWebServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Cấu hình kích thước request lớn (cho multipart/form-data)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100MB
});

// Cấu hình Kestrel để chấp nhận request lớn
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 100_000_000; // 100MB
});

var app = builder.Build();
// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI(settings =>
{
    settings.SwaggerEndpoint("/api/specification.json", "StudyFlowProject API");
    settings.RoutePrefix = "swagger";
});

// Redirect root to Swagger
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

await app.InitialiseDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Quan trọng: Thứ tự middleware chuẩn
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("Authorization")); // Cho phép client đọc header Authorization

app.UseHttpsRedirection();
app.UseAuthentication();  // Đặt trước UseAuthorization
app.UseAuthorization();   // Đặt trước các middleware cần xác thực
app.UseStaticFiles();     // Đặt sau auth để file tĩnh không cần xác thực
app.MapRazorPages();
app.UseExceptionHandler(); // Chỉ gọi một lần
app.MapEndpoints();

app.Run();
public partial class Program { }
