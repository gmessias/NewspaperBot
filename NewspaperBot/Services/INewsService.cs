using Newtonsoft.Json.Linq;

namespace NewspaperBot.Services;

public interface INewsService
{
     Task<IEnumerable<JToken>> NewsHotCommand();
     Task<IEnumerable<JToken>> NewsTopCommand(string[] instructions);
     Task<IEnumerable<JToken>> NewsEveryCommand(string[] instructions);
}