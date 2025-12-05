using HtmlAgilityPack;

namespace CinemaNotifier.Worker;

public class MovieService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;
    private const string BaseUrl = "https://www.sinemalar.com";

    public MovieService(ILogger<MovieService> logger)
    {
        _httpClient = new HttpClient();
        // Mimic a real browser to avoid some blocking
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _logger = logger;
    }

    public async Task<List<string>> GetMoviesInElazigAsync()
    {
        var foundMovies = new HashSet<string>();
        try
        {
            // Target specific cinema
            var cinemaUrl = "https://www.sinemalar.com/sinemasalonu/2231/elazig-cinepoint-elysium-park";
            _logger.LogInformation($"Sinema kontrol ediliyor: {cinemaUrl}");

            try 
            {
                var cinemaHtml = await _httpClient.GetStringAsync(cinemaUrl);
                var cinemaDoc = new HtmlDocument();
                cinemaDoc.LoadHtml(cinemaHtml);

                var movieNodes = cinemaDoc.DocumentNode.SelectNodes("//a[contains(@href, '/film/')]");
                if (movieNodes != null)
                {
                    foreach (var mNode in movieNodes)
                    {
                        var title = mNode.InnerText.Trim();
                        // Filter strict: Title > 2 chars, not "Seans", and perhaps ensure it's not a generic link
                        if (!string.IsNullOrWhiteSpace(title) && title.Length > 2 && !title.Contains("Seans", StringComparison.OrdinalIgnoreCase))
                        {
                            title = System.Net.WebUtility.HtmlDecode(title);
                            foundMovies.Add(title);
                            _logger.LogInformation($"Film bulundu: {title}");
                        }
                    }
                }
            }
            catch (Exception cx)
            {
                _logger.LogError(cx, $"Sinema taranırken hata: {cinemaUrl}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filmler taranırken hata oluştu.");
        }

        return foundMovies.ToList();
    }
}
