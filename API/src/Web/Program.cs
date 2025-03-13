using Microsoft.AspNetCore.Builder;
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
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI(settings =>
    {
        settings.SwaggerEndpoint("/api/specification.json", "StudyFlowProject API");
        settings.RoutePrefix = "swagger";
    });

    // Thêm middleware chuyển hướng
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger/index.html");
            return;
        }
        await next();
    });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();


app.UseExceptionHandler(options => { });
app.UseCors(
    options => options.
    WithOrigins("http://localhost:4200")
    .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.MapEndpoints();
app.UseExceptionHandler();
app.Run();

public partial class Program { }
