namespace TMP_Bot_2.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    /// General module containing commands.
    /// </summary>
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping"), Alias("p", "test"), Summary("Checks if bot is online")]
        public async Task PingAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();
            await this.ReplyAsync("Pong!");
        }


        [Command("add", true), Alias("+", "new"), Summary("Adds new video to the list")]
        public async Task AddAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();

            string message = this.Context.Message.Content;
            string[] words = message.Split(' ');

            string username = await Parse.UserAsync(words[2]);
            string title = await Parse.TitleAsync(words[2]);
            string date = await Parse.DateAsync(words[2]);

            //await this.ReplyAsync($"Url: `{words[2]}`");
            //if (words.Length > 2)
            //{
            //    await this.ReplyAsync($"Comment: `{words[3]}`");
            //}

            //string date = await Parse.DateAsync(words[2]);
            await this.ReplyAsync($"**Added video:** {title} | {username}\n**Uploaded on a date:** {date}");
        }


        [Command("list"), Alias("array", "out"), Summary("Outputs full video list")]
        public async Task ListAsync()
        {
            await this.ReplyAsync("Video list...\n**[Not implemented]**");
        }


        [Command("pastebin"), Summary("Generates pastebin for TMP description")]
        public async Task PastebinAsync()
        {
            await this.ReplyAsync("**Generated pastebin: **\n**[Not implemented]**");
        }


        [Command("data"), Summary("Outputs full video data")]
        public async Task SataAsync()
        {
            await this.ReplyAsync("Current stored data:\n**[Not implemented]**");
        }


        [Command("list compact"), Summary("Outputs full video data in compact way")]
        public async Task ListCompactAsync()
        {
            await this.ReplyAsync("Compact video list...\n**[Not implemented]**");
        }


        [Command("help"), Alias("?", "asist"), Summary("Displays all commands")]
        public async Task HelpAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();

            var embed = new EmbedBuilder()
                .WithTitle("TMP Bot 2 Help")
                .WithColor(new Color(255, 102, 94))
                .AddField("Prefix: ", "!!tmp")
                .AddField("Commands", "\n" +
                "**add** Adds new video to the list\n" +
                "**data** Outputs full video data\n" +
                "**help** Displays all commands\n" +
                "**list** Outputs full video list\n" +
                "**list compact** Outputs full video data in compact way\n" +
                "**pastebin** Generates pastebin for TMP description\n" +
                "**ping** Checks if bot is online")
                .WithFooter("Developed by KK")
                .WithUrl("https://github.com/KK-mp4/TMP-Bot-2")
                .Build();
            await this.ReplyAsync(embed: embed);
        }
    }
}
