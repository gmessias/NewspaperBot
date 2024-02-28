using System.Net.Http.Headers;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace NewspaperBot.Commands;

public class NewsCommand : ModuleBase<SocketCommandContext>
{
    private readonly HttpClient _httpClient;
    //private const string NewsApiKey = "cba4bdbd69af456bab0baf5bdc31d92a";
    private readonly IConfiguration _configuration;

    public NewsCommand(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "NewspaperBot");
        _configuration = configuration;
    }
    
    [Command("news")]
    [Summary("Returns the top 10 news")]
    public async Task ExecuteAsync([Remainder][Summary("A command")] string command)
    {
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
                await ReplyAsync($"{article["title"]}: {article["url"]}");
            }
        }
    }
}