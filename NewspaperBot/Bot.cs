using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NewspaperBot;

public class Bot : IBot
{
    private ServiceProvider? _serviceProvider;

    private readonly ILogger<Bot> _logger;
    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    public Bot(ILogger<Bot> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        DiscordSocketConfig config = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);
        _commands = new CommandService();
    }

    public async Task StartAsync(ServiceProvider services)
    {
        var discordToken = _configuration["DiscordToken"] ?? throw new Exception("Missing Discord token");

        _logger.LogInformation("Bot started successfully");

        _serviceProvider = services;

        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        _client.MessageReceived += HandleCommandAsync;
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("Shutting down");

        await _client.LogoutAsync();
        await _client.StopAsync();
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        if (arg is not SocketUserMessage message || message.Author.IsBot)
        {
            return;
        }
        
        _logger.LogInformation("{Time} - {Author}: {Content}", DateTime.Now.ToShortTimeString(), message.Author, message.Content);
        
        var position = 0;
        var messageIsCommand = message.HasCharPrefix('!', ref position);

        if (messageIsCommand)
        {
            await _commands.ExecuteAsync(
                new SocketCommandContext(_client, message),
                position,
                _serviceProvider);
        }
    }
}