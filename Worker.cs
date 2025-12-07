namespace CinemaNotifier.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly MovieService _movieService;
    private readonly TelegramService _telegramService;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, MovieService movieService, TelegramService telegramService, IConfiguration configuration)
    {
        _logger = logger;
        _movieService = movieService;
        _telegramService = telegramService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initial notification
        await _telegramService.SendNotificationAsync("🎬 Sinema Elazığ Botu Başlatıldı! İlk kontrol yapılıyor...");

        // Run immediately once
        await CheckAndNotify(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var intervalMinutes = _configuration.GetValue<int?>("CheckIntervalMinutes") 
                                  ?? _configuration.GetValue<int?>("WorkerSettings:CheckIntervalMinutes") 
                                  ?? 10;
            var delay = TimeSpan.FromMinutes(intervalMinutes);

            _logger.LogInformation($"Bir sonraki kontrol zamanı: {DateTime.Now.Add(delay)}");
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            await CheckAndNotify(stoppingToken);
        }
    }

    private async Task CheckAndNotify(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Filmler kontrol ediliyor: {time}", DateTimeOffset.Now);

        var movies = await _movieService.GetMoviesInElazigAsync();

        if (movies.Any())
        {
            var message = $"📢 Elazığ'da Vizyondaki Filmler ({DateTime.Now:dd.MM.yyyy HH:mm}):\n\n" + string.Join("\n", movies.Select(m => $"• {m}"));
            await _telegramService.SendNotificationAsync(message);
        }
        else
        {
            _logger.LogInformation("Elazığ'da film bulunamadı.");
            // Notify user even if empty, so they verify it works
            await _telegramService.SendNotificationAsync($"ℹ️ Bilgi: Şu an Elazığ'da vizyonda herhangi bir film bulunamadı ({DateTime.Now:HH:mm}).");
        }
    }


}
