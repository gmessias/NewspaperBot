using Discord.Commands;
using NewspaperBot.Services;
using Newtonsoft.Json.Linq;

namespace NewspaperBot.Commands;

public class NewsCommand(INewsService newsService) : ModuleBase<SocketCommandContext>
{
    [Command("news")]
    [Summary("Returns the top 10 news")]
    public async Task ExecuteAsync([Remainder][Summary("A command")] string command)
    {
        if (string.IsNullOrEmpty(command)) await ReplyAsync("Please enter the type of news command");

        string[] commandSplit = command.Split(' ');
        string subcommand = commandSplit[0];

        bool haveArguments = commandSplit.Length > 1;
        
        switch (subcommand)
        {
            case "hot":
                var hotEnumerable = await newsService.NewsHotCommand();
                ResultHot(hotEnumerable);
                break;
            case "top":
                if (haveArguments)
                {
                    var topEnumerable = await newsService.NewsTopCommand(commandSplit);
                    ResultDefault(topEnumerable);
                }
                break;
            case "everything":
                if (haveArguments)
                {
                    var everyEnumerable = await newsService.NewsEverythingCommand(commandSplit);
                    ResultDefault(everyEnumerable);
                }
                break;
            default:
                await ReplyAsync("This type of command was not recognized");
                break;
        }
    }
    
    private async void ResultDefault(IEnumerable<JToken>? enumerable)
    {
        var articles = enumerable!.ToList();
        if (articles.Any())
        {
            ReplyAsyncArticles(articles);
        }
        else
        {
            await ReplyAsync("Unable to execute the command");
        }
    }

    private async void ResultHot(IEnumerable<JToken>? enumerable)
    {
        var articles = enumerable!.ToList();
        if (articles.Any())
        {
            ReplyAsyncArticles(articles);
        }
        else
        {
            await ReplyAsync("Unable to execute the 'news hot' command");
        }
    }

    private async void ReplyAsyncArticles(List<JToken> articles)
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