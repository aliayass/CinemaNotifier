
namespace CinemaNotifier.Worker;

public class MinimalApiService : IHostedService
{
    private WebApplication? _app;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateBuilder();
        
        // Use the PORT environment variable if available (Render)
        var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
        builder.WebHost.UseUrls($"http://*:{port}");

        _app = builder.Build();

        _app.MapGet("/", () => "Bot çalışıyor ✔");
        
        return _app.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _app?.StopAsync(cancellationToken) ?? Task.CompletedTask;
}
