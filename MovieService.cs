using HtmlAgilityPack;
using System.Text;
using System;
using System.Linq;

namespace CinemaNotifier.Worker;

public class MovieService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;
    private const string BaseUrl = "https://boxofficeturkiye.com";
    private readonly List<string> _followList = new()
    {
        "Jujutsu Kaisen: Execution"
    };

    public MovieService(ILogger<MovieService> logger)
    {
        _httpClient = new HttpClient();
        // Mimic a real browser to avoid some blocking
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        _logger = logger;
    }

    public async Task<List<string>> GetMoviesInElazigAsync()
    {
        var foundMovies = new List<string>();
        try
        {
            // Target specific cinema page on BoxOfficeTurkiye
            var cinemaUrl = "https://boxofficeturkiye.com/sinema/elazig-cinepoint-elysium-park--745";
            _logger.LogInformation($"Sinema kontrol ediliyor: {cinemaUrl}");

            try 
            {
                var cinemaHtml = await _httpClient.GetStringAsync(cinemaUrl);
                var cinemaDoc = new HtmlDocument();
                cinemaDoc.LoadHtml(cinemaHtml);

                var movieNodes = cinemaDoc.DocumentNode.SelectNodes("//div[contains(@class, 'c-toggle__movie-item')]");
                if (movieNodes != null)
                {
                    foreach (var movieNode in movieNodes)
                    {
                        var titleNode = movieNode.SelectSingleNode(".//div[contains(@class, 'c-session__hours')]//h3/a");
                        var genreNode = movieNode.SelectSingleNode(".//p[contains(@class, 'c-session__movie-class')]");
                        
                        var title = titleNode?.InnerText?.Trim();
                        if (string.IsNullOrWhiteSpace(title)) continue;
                        
                        title = System.Net.WebUtility.HtmlDecode(title);
                        
                        // Feature: Takip listesi filtresi
                        if (!_followList.Any(f => title.Contains(f, StringComparison.OrdinalIgnoreCase)))
                        {
                             continue;
                        }

                        var genre = genreNode?.InnerText?.Trim();
                        if (!string.IsNullOrWhiteSpace(genre))
                        {
                            genre = System.Net.WebUtility.HtmlDecode(genre);
                        }

                        // Sessions
                        var sessionTypeNodes = movieNode.SelectNodes(".//div[contains(@class, 'c-session__movie-type')]");
                        var sessionsBuilder = new StringBuilder();

                        if (sessionTypeNodes != null)
                        {
                            foreach (var typeNode in sessionTypeNodes)
                            {
                                var headerNode = typeNode.SelectSingleNode(".//div[contains(@class, 'c-session__movie-type-header')]");
                                var hourNodes = typeNode.SelectNodes(".//span[contains(@class, 'c-session__hour')]");
                                
                                var header = headerNode?.InnerText?.Trim();
                                if (!string.IsNullOrWhiteSpace(header))
                                {
                                    header = System.Net.WebUtility.HtmlDecode(header);
                                }

                                if (hourNodes != null)
                                {
                                    var hours = hourNodes.Select(h => h.InnerText.Trim()).Where(h => !string.IsNullOrWhiteSpace(h));
                                    if (hours.Any())
                                    {
                                        if (sessionsBuilder.Length > 0) sessionsBuilder.Append(" | ");
                                        sessionsBuilder.Append($"{header}: {string.Join(", ", hours)}");
                                    }
                                }
                            }
                        }

                        var movieInfo = $"🎬 {title}";
                        if (!string.IsNullOrWhiteSpace(genre))
                        {
                            movieInfo += $" ({genre})";
                        }
                        
                        if (sessionsBuilder.Length > 0)
                        {
                            movieInfo += $"\n   🕒 {sessionsBuilder}";
                        }
                        
                        foundMovies.Add(movieInfo);
                        _logger.LogInformation($"Film bulundu: {title}");
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

        return foundMovies;
    }
}
