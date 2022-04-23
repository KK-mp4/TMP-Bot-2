namespace TMP_Bot_2
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The entry point of the bot.
    /// </summary>
    internal class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        private static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            this.client = new DiscordSocketClient();
            this.commands = new CommandService();

            this.services = new ServiceCollection()
                .AddSingleton(this.client)
                .AddSingleton(this.commands)
                .BuildServiceProvider();

            string token = GetToken.Get();      // gets Discord bot token from gitignored file

            this.client.Log += this.Client_Log;

            await this.RegisterCommandsAsync();

            await this.client.LoginAsync(TokenType.Bot, token);

            await this.client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            this.client.MessageReceived += this.HandleCommandAsync;
            await this.commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(this.client, message);
            if (message.Source != MessageSource.User)
            {
                return;
            }

            int argPos = 0;
            if (message.HasStringPrefix("!!tmp ", ref argPos))
            {
                var result = await this.commands.ExecuteAsync(context, argPos, this.services);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}
