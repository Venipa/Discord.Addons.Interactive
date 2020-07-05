using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SampleApp
{
    public class Program
    {
        private static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task MainAsync()
        {
            var token = File.ReadAllText("token.ignore");

            _client = new DiscordSocketClient();

            _client.Log += log =>
            {
                Console.WriteLine(log.ToString());
                return Task.CompletedTask;
            };

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();

            _commands = new CommandService();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.MessageReceived += HandleCommandAsync;

            await Task.Delay(-1);
        }

        public async Task HandleCommandAsync(SocketMessage m)
        {
            if (!(m is SocketUserMessage msg)) return;
            if (msg.Author.IsBot) return;

            var argPos = 0;
            if (!(msg.HasStringPrefix("i~>", ref argPos))) return;

            var context = new SocketCommandContext(_client, msg);
            await _commands.ExecuteAsync(context, argPos, _services);
        }
    }
}