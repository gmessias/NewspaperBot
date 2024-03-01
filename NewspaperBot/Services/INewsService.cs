using Newtonsoft.Json.Linq;

namespace NewspaperBot.Services;

public interface INewsService
{
     Task<IEnumerable<JToken>> NewsHotCommand();
}