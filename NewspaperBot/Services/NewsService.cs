using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace NewspaperBot.Services;

public class NewsService : INewsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public NewsService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "NewspaperBot");
        _configuration = configuration;
    }

    public async Task<IEnumerable<JToken>> NewsHotCommand()
    {
        var apiKey = _configuration["NewsApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return Enumerable.Empty<JToken>();
        }
        
        var response = await _httpClient.GetStringAsync($"https://newsapi.org/v2/top-headlines?country=br&apiKey={apiKey}");
        var json = JObject.Parse(response);
        var articles = json["articles"]?.Take(10);

        return articles ?? Enumerable.Empty<JToken>();
    }

    public async Task<IEnumerable<JToken>> NewsTopCommand(string[] instructions)
    {
        var apiKey = _configuration["NewsApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return Enumerable.Empty<JToken>();
        }
        
        var response = await _httpClient.GetStringAsync($"https://newsapi.org/v2/top-headlines?q={instructions[1]}&country=br&apiKey={apiKey}");
        var json = JObject.Parse(response);
        var articles = json["articles"]?.Take(4);

        return articles ?? Enumerable.Empty<JToken>();
    }
    
    public async Task<IEnumerable<JToken>> NewsEverythingCommand(string[] instructions)
    {
        var apiKey = _configuration["NewsApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return Enumerable.Empty<JToken>();
        }
        
        var response = await _httpClient.GetStringAsync($"https://newsapi.org/v2/everything?q={instructions[1]}&apiKey={apiKey}");
        var json = JObject.Parse(response);
        var articles = json["articles"]?.Take(6);

        return articles ?? Enumerable.Empty<JToken>();
    }
}