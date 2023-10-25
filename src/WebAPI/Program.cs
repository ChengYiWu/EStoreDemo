using Infrastructure;
using Serilog.Events;
using Serilog;
using WebAPI.Infrastructure;

// Ref: https://blog.miniasp.com/post/2021/11/29/How-to-use-Serilog-with-NET-6
// ���B�]�w�קK Host �ҥεo�Ϳ��~�ɨS�������� log
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

    // Application �h�̪ۨ`�J
    builder.Services.AddApplicationServices();

    // Infrastructure �h�̪ۨ`�J
    builder.Services.AddInfrastuctureServices(configuration);

    // WebAPI �h�̪ۨ`�J
    builder.Services.AddWebAPIServices(configuration);

    // �ϥ� Serilog �@�� logger �M��
    builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // �Ȯɥ�����
        //await app.InitialiseDatabaseAsync();

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // �����C�� Request �ШD�� Log
    app.UseSerilogRequestLogging();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Web API ���ε{���Ұʵo�Ϳ��~");
}
finally
{
    Log.CloseAndFlush();
}