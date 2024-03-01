using System.Net.Http.Headers;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using NewspaperBot.Services;
using Newtonsoft.Json.Linq;

namespace NewspaperBot.Commands;

public class NewsCommand : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly INewsService _newsService;

    public NewsCommand(IConfiguration configuration, INewsService newsService)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "NewspaperBot");
        _configuration = configuration;
        _newsService = newsService;
    }
    
    [Command("news")]
    [Summary("Returns the top 10 news")]
    public async Task ExecuteAsync([Remainder][Summary("A command")] string command)
    {
        await ReplyAsync(_newsService.Teste());
        
        if (string.IsNullOrEmpty(command) || command != "hot")
        {
            await ReplyAsync("Usage: !news hot");
            return;
        }

        var newsApiKey = _configuration["NewsApiKey"];
        if (string.IsNullOrEmpty(newsApiKey))
        {
            await ReplyAsync("Sorry, error in API Key");
            return;
        }
        
        var response = await _httpClient.GetStringAsync($"https://newsapi.org/v2/top-headlines?country=br&apiKey={newsApiKey}");
        var json = JObject.Parse(response);
        var articles = json["articles"]?.Take(10);

        if (articles != null)
        {
            foreach (var article in articles)
            {
                await ReplyAsync($"**Title**: {article["title"]} \n" +
                                 $"**Url**: [Click here]({article["url"]}) \n" +
                                 $"**Author**: {article["author"]} \n" +
                                 $"**Publish At**: {article["publishedAt"]}");
            }
        }
    }
}