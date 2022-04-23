namespace TMP_Bot_2.Modules
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Newtonsoft.Json;

    /// <summary>
    /// General module containing commands.
    /// </summary>
    public class Commands : ModuleBase<SocketCommandContext>
    {
        public class Video
        {
            public string url { get; set; }
            public string channel_name { get; set; }
            public string video_title { get; set; }
            public string upload_date { get; set; }
            public string? comment { get; set; }

            public Video(string url, string channel_name, string video_title, string upload_date, string comment = null)
            {
                this.url = url;
                this.channel_name = channel_name;
                this.video_title = video_title;
                this.upload_date = upload_date;
                this.comment = comment;
            }
        }

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

            //cleaning url
            string message = this.Context.Message.Content;
            string[] words = message.Split(' ');
            words[2] = words[2].Replace("<", string.Empty);
            words[2] = words[2].Replace(">", string.Empty);
            words[2] = words[2].Replace("https://www.bilibili.com/video", "https://b23.tv");
            int queryPos = words[2].IndexOf('?');
            if (queryPos >= 0)
            {
                words[2] = words[2].Remove(queryPos, words[2].Length - queryPos);
            }

            string username = await Parse.UserAsync(words[2]);
            string title = await Parse.TitleAsync(words[2]);
            string date = await Parse.DateAsync(words[2]);

            await this.ReplyAsync($"**Added video:** {title}\t|\t{username} ({date})");

            //downloading thumbnail
            string thumbnail = await Parse.ThumbnailAsync(words[2]);
            System.Net.WebRequest request = System.Net.WebRequest.Create(thumbnail);
            System.Net.WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Bitmap img = new Bitmap(responseStream);
            Bitmap imgResized = new Bitmap(img, 188, 100);
            img.Dispose();
            string filename = words[2].Replace("https://b23.tv/", string.Empty);
            imgResized.Save(@$"..\..\..\Thumbnails\{filename}.jpg");
            imgResized.Dispose();

            //saving to json
            string path = @"..\..\..\TMP_List.json";
            if (File.Exists(path))
            {
                List<Video> vidList = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(path));
                if (vidList == null)
                {
                    vidList = new List<Video>();
                }

                Video newVid = new Video(words[2], username, title, date);
                if (words.Length > 3)
                {
                    string comment = words[3];
                    for (int i = 4; i < words.Length; i++)
                    {
                        comment += " " + words[i];
                    }
                    newVid.comment = comment;
                }
                vidList.Add(newVid);

                vidList.Sort((x, y) => x.upload_date.CompareTo(y.upload_date));

                File.WriteAllText(path, JsonConvert.SerializeObject(vidList, Formatting.Indented));
            }
        }


        [Command("list"), Alias("array", "out"), Summary("Outputs full video list")]
        public async Task ListAsync()
        {
            List<Video> videos = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(@"..\..\..\TMP_List.json"));

            if (videos != null)
            {
                await this.ReplyAsync("**Videos:**\n");
                for (int i = 0; i < videos.Count; i++)
                {
                    string filename = videos[i].url.Replace("https://b23.tv/", string.Empty);
                    await Context.Channel.SendFileAsync(@$"..\..\..\Thumbnails\{filename}.jpg", $"————————————————————————\n**{i + 1}:**\t{videos[i].video_title}\t |\t {videos[i].channel_name} ({videos[i].upload_date})\n{videos[i].comment}\n<{videos[i].url}>");
                }
            }
            else
                await this.ReplyAsync("List is empty");
        }


        [Command("pastebin"), Alias("txt", ".txt", "description"), Summary("Generates pastebin for TMP description")]
        public async Task PastebinAsync()
        {
            List<Video> videos = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(@"..\..\..\TMP_List.json"));

            if (videos != null)
            {
                using (StreamWriter writer = new StreamWriter(@"..\..\..\TMP_List.txt"))
                {
                    for (int i = 0; i < videos.Count; i++)
                        writer.WriteLine($"{videos[i].channel_name} - {videos[i].video_title}:\n{videos[i].url}");
                }
                await Context.Channel.SendFileAsync(@$"..\..\..\TMP_List.txt", "**Generated pastebin:**");
            }
            else
                await this.ReplyAsync("List is empty");
        }


        [Command("data"), Summary("Outputs full video data")]
        public async Task SataAsync()
        {
            await Context.Channel.SendFileAsync(@$"..\..\..\TMP_List.json", "Current stored data:");
        }


        [Command("list compact"), Summary("Outputs full video data in compact way")]
        public async Task ListCompactAsync()
        {
            List<Video> videos = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(@"..\..\..\TMP_List.json"));

            if (videos != null)
            {
                for (int i = 0; i < videos.Count; i++)
                {
                    await this.ReplyAsync($"**{i + 1}:**\t{videos[i].video_title}\t |\t {videos[i].channel_name} ({videos[i].upload_date})\n<{videos[i].url}>");
                }
            }
            else
                await this.ReplyAsync("List is empty");
        }


        [Command("help"), Alias("?", "asist"), Summary("Displays all commands")]
        public async Task HelpAsync()
        {
            await this.Context.Channel.TriggerTypingAsync();

            var embed = new EmbedBuilder()
                .WithTitle("TMP Bot 2 Help")
                .WithColor(new Discord.Color(255, 102, 94))
                .AddField("Prefix: ", "!!tmp")
                .AddField("Commands", "\n" +
                "**add**\t Adds new video to the list\n" +
                "**clear**\t Clears list (only owner can)\n" +
                "**data**\t Outputs full video data\n" +
                "**help**\t Displays all commands\n" +
                "**list**\t Outputs full video list\n" +
                "**list compact**\t Outputs full video data in compact way\n" +
                "**list count**\t Counts amount of stored videos\n" +
                "**pastebin**\t Generates pastebin for TMP description\n" +
                "**ping**\t Checks if bot is online\n" +
                "**remove**\t Removes video from the list\n" +
                "**thumbnail**\t Generates thumbnail for the episode")
                .WithFooter("Developed by KK")
                .WithUrl("https://github.com/KK-mp4/TMP-Bot-2")
                .Build();
            await this.ReplyAsync(embed: embed);
        }


        [Command("clear"), RequireOwner, Summary("Clears TMP_List.json")]
        public async Task ClearAsync()
        {
            using (StreamWriter writer = new StreamWriter(@"..\..\..\TMP_List.json"))
            {
                writer.WriteLine("");
            }
            await this.ReplyAsync("TMP List have been cleared.");
        }


        [Command("thumbnail"), RequireOwner, Summary("Generates thumbnail for the episode")]
        public async Task ThumbnailAsync()
        {
            await this.ReplyAsync("Thumbnail for episode: **16**");
        }


        [Command("remove", true), Alias("delete", "-"), Summary("Removes video from the list")]
        public async Task RemoveAsync()
        {
            //cleaning url
            string message = this.Context.Message.Content;
            string[] words = message.Split(' ');
            words[2] = words[2].Replace("<", string.Empty);
            words[2] = words[2].Replace(">", string.Empty);
            words[2] = words[2].Replace("https://www.bilibili.com/video", "https://b23.tv");
            int queryPos = words[2].IndexOf('?');
            if (queryPos >= 0)
            {
                words[2] = words[2].Remove(queryPos, words[2].Length - queryPos);
            }

            string path = @"..\..\..\TMP_List.json";
            if (File.Exists(path))
            {
                List<Video> vidList = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(path));

                if (vidList == null) return;

                for (int i = 0; i < vidList.Count; i++)
                {
                    if (words[2] == vidList[i].url)
                    {
                        vidList.RemoveAt(i);
                        await this.ReplyAsync($"Video with URL: **{words[2]}** got removed");
                        File.WriteAllText(path, JsonConvert.SerializeObject(vidList, Formatting.Indented));
                        return;
                    }
                }
                await this.ReplyAsync($"No video was removed");
            }
        }


        [Command("list count", true), RequireOwner, Summary("Counts amount of stored videos")]
        public async Task CountAsync()
        {
            string path = @"..\..\..\TMP_List.json";
            if (File.Exists(path))
            {
                List<Video> vidList = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(path));

                if (vidList == null) return;

                await this.ReplyAsync($"The list currently has **{vidList.Count}** videos stored");
            }
        }
    }
}
