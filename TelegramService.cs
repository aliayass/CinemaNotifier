using Telegram.Bot;

namespace CinemaNotifier.Worker;

public class TelegramService
{
    private readonly TelegramBotClient _botClient;
    private readonly string _targetChatId;
    private readonly ILogger<TelegramService> _logger;

    public TelegramService(IConfiguration configuration, ILogger<TelegramService> logger)
    {
        _logger = logger;
        var token = configuration["Telegram:BotToken"];
        _targetChatId = configuration["Telegram:ChatId"] ?? configuration["Telegram:TargetChatId"];

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(_targetChatId))
        {
            _logger.LogError("Telegram yapılandırması eksik!");
            throw new ArgumentNullException("Telegram yapılandırması eksik!");
        }

        _botClient = new TelegramBotClient(token);
    }

    public async Task SendNotificationAsync(string message)
    {
        try
        {
            await _botClient.SendMessage(_targetChatId, message);
            _logger.LogInformation($"Telegram'a bildirim gönderildi: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Telegram bildirimi gönderilemedi.");
        }
    }
}
