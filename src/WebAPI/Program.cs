using Infrastructure;
using Serilog.Events;
using Serilog;
using WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// Ref: https://blog.miniasp.com/post/2021/11/29/How-to-use-Serilog-with-NET-6
// 此處設定避免 Host 啟用發生錯誤時沒有紀錄到 log
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{

    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;

    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Application 層相依注入
    builder.Services.AddApplicationServices();

    // Infrastructure 層相依注入
    builder.Services.AddInfrastuctureServices(configuration);

    // WebAPI 層相依注入
    builder.Services.AddWebAPIServices(configuration);

    // 使用 Serilog 作為 logger 套件
    builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {   
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    await app.InitialiseDatabaseAsync();

    // 紀錄每個 Request 請求的 Log
    app.UseSerilogRequestLogging();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseMiddleware<TransactionMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Web API 應用程式啟動發生錯誤");
}
finally
{
    Log.CloseAndFlush();
}