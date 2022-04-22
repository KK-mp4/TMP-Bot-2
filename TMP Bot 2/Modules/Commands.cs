namespace TMP_Bot_2.Modules
{
    using System;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    /// General module containing commands.
    /// </summary>
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("p", "test")]
        [Summary("Checks if bot is online")]
        public async Task PingAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();
            await this.ReplyAsync("Pong!");
        }

        [Command("help")]
        [Alias("?", "h")]
        [Summary("Displays all commands")]
        public async Task HelpAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();

            var embed = new EmbedBuilder()
                .WithTitle("TMP Bot 2 Help")
                .WithColor(new Color(255, 102, 94))
                .AddField("Commands", "\n" +
                "**!!tmp2 ping** Checks if bot is online\n" +
                "**!!tmp2 help** Displays all commands\n" +
                "**!!tmp2 add** Adds new video to the list\n" +
                "**!!tmp2 list** Outputs full video list")
                .WithFooter("Developed by KK")
                .WithUrl("https://github.com/KK-mp4/TMP-Bot-2")
                .Build();
            await this.ReplyAsync(embed: embed);
        }

        [Command("add", true)]
        [Alias("+", "new")]
        [Summary("Adds new video to the list")]
        public async Task AddAsync()
        {
            string message = this.Context.Message.Content;
            string[] words = message.Split(' ');

            //await this.ReplyAsync($"Url: `{words[2]}`");
            //if (words.Length > 2)
            //{
            //    await this.ReplyAsync($"Comment: `{words[3]}`");
            //}

            string name = await Parse.NameAsync(words[2]);
            //string date = await Parse.DateAsync(words[2]);
            await this.ReplyAsync($"**Added video:** {name}");
            //await this.ReplyAsync($"Date: `{date}`");
        }

        [Command("list")]
        [Alias("array", "out")]
        [Summary("Outputs full video list")]
        public async Task ListAsync()
        {
            await this.ReplyAsync("Full video list...");
        }
    }
}
