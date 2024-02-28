using Microsoft.Extensions.DependencyInjection;

namespace NewspaperBot;

public interface IBot
{
    Task StartAsync(ServiceProvider services);
    Task StopAsync();
}