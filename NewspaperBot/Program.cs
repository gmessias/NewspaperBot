﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewspaperBot.Services;

namespace NewspaperBot;

internal class Program
{
    private static void Main(string[] args) =>
        MainAsync(args).GetAwaiter().GetResult();

    private static async Task MainAsync(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(options =>
            {
                options.ClearProviders();
                options.AddConsole();
            })
            .AddSingleton<IConfiguration>(configuration)
            .AddScoped<IBot, Bot>()
            .AddSingleton<INewsService, NewsService>()
            .BuildServiceProvider();

        try
        {
            var bot = serviceProvider.GetRequiredService<IBot>();

            await bot.StartAsync(serviceProvider);

            do
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key != ConsoleKey.Q) continue;
                
                await bot.StopAsync();
                return;
            } while (true);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            Environment.Exit(-1);
        }
    }
}
