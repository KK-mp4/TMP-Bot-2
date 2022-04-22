namespace TMP_Bot_2.Modules
{
    using System.Threading.Tasks;
    using Discord.Commands;

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
            await this.ReplyAsync(
                "**!!tmp2 ping** Checks if bot is online\n" +
                "**!!tmp2 help** Displays all commands\n" +
                "**!!tmp2 add** Adds new video to the list\n" +
                "**!!tmp2 list** Outputs full video list");
        }

        [Command("add")]
        [Alias("+", "new")]
        [Summary("Adds new video to the list")]
        public async Task AddAsync()
        {
            await this.ReplyAsync("Adding...");
        }

        [Command("list")]
        [Alias("array", "out")]
        [Summary("Outputs full video list")]
        public async Task ListAsync()
        {
            await this.ReplyAsync("Full videl list...");
        }
    }
}
