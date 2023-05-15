using Serilog;
using TextToSql;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

var app = builder.Build();

var proxy = new OpenAIProxy(builder.Configuration);

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/text-to-sql", (SqlRequest data, CancellationToken token) =>
    proxy.SendRequest(data, token));

app.Run();