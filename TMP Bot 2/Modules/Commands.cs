namespace TMP_Bot_2.Modules
{
    using System;
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
        [Command("ping"), Alias("p", "test"), Summary("Checks if bot is online")]
        public async Task PingAsync()
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync("Pong!");
        }


        [Command("add", true), Alias("+", "new"), Summary("Adds new video to the list")]
        public async Task AddAsync()
        {
            await Context.Channel.TriggerTypingAsync();

            bool mediaType = true; // true is YouTube, false is Bilibili
            string username = "Null";
            string title = "Null";
            string date = "Null";

            //cleaning url
            string message = Context.Message.Content;
            string[] words = message.Split(' ');
            words[2] = words[2].Replace("<", string.Empty);
            words[2] = words[2].Replace(">", string.Empty);

            if (words[2].Contains("https://www.bilibili.com/") || words[2].Contains("https://b23.tv"))
            {
                mediaType = false;
            }

            if (mediaType == false)
            {
                words[2] = words[2].Replace("https://www.bilibili.com/video", "https://b23.tv");
                int queryPos = words[2].IndexOf('?');
                if (queryPos >= 0)
                {
                    words[2] = words[2].Remove(queryPos, words[2].Length - queryPos);
                }

                username = await Parse.BiliUserAsync(words[2]);
                title = await Parse.BiliTitleAsync(words[2]);
                date = await Parse.BiliDateAsync(words[2]);
            }
            else
            {
                words[2] = words[2].Replace("https://www.youtube.com/watch?v=", "https://youtu.be/");
                int queryPos = words[2].IndexOf('&');
                if (queryPos >= 0)
                {
                    words[2] = words[2].Remove(queryPos, words[2].Length - queryPos);
                }

                username = await Parse.YTUserAsync(words[2]);
                title = await Parse.YTTitleAsync(words[2]);
                date = await Parse.YTDateAsync(words[2]);
            }

            //saving to json
            string path = @"..\..\..\TMP_List.json";
            if (!File.Exists(path))
            {
                return;
            }

            List<Video> vidList = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(path));
            if (vidList == null)
            {
                vidList = new List<Video>();
            }

            // checks if this vid already in list
            for (int i = 0; i < vidList.Count; i++)
            {
                if (words[2] == vidList[i].Url)
                {
                    await ReplyAsync($"Video with URL: **<{words[2]}>** already in the list");
                    return;
                }
            }

            await ReplyAsync($"**Added video:** {title}\t|\t{username} ({date})");

            Video newVid = new Video(words[2], username, title, date);
            if (words.Length > 3)
            {
                string comment = words[3];
                for (int i = 4; i < words.Length; i++)
                {
                    comment += " " + words[i];
                }

                newVid.Comment = comment;
            }

            vidList.Add(newVid);

            vidList.Sort((x, y) => x.Upload_date.CompareTo(y.Upload_date));

            File.WriteAllText(path, JsonConvert.SerializeObject(vidList, Formatting.Indented));

            //downloading thumbnail
            if (mediaType == true)
            {
                string thumbnail = Parse.YTThumbnail(words[2]);
                System.Net.WebRequest request = System.Net.WebRequest.Create(thumbnail);
                System.Net.WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                Bitmap img = new Bitmap(responseStream);

                Rectangle rect = new Rectangle(0, 45, img.Width, 270);
                Bitmap imgCropped = img.Clone(rect, img.PixelFormat);
                img.Dispose();

                Bitmap imgResized = new Bitmap(imgCropped, 188, 100);
                imgCropped.Dispose();
                string filename = words[2].Replace("https://youtu.be/", string.Empty);
                imgResized.Save(@$"..\..\..\Thumbnails\{filename}.jpg");
                imgResized.Dispose();
            }
            else
            {
                string thumbnail = await Parse.BiliThumbnailAsync(words[2]);
                System.Net.WebRequest request = System.Net.WebRequest.Create(thumbnail);
                System.Net.WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                Bitmap img = new Bitmap(responseStream);

                Bitmap imgResized = new Bitmap(img, 188, 100);
                img.Dispose();
                string filename = words[2].Replace("https://b23.tv/", string.Empty);
                imgResized.Save(@$"..\..\..\Thumbnails\{filename}.jpg");
                imgResized.Dispose();
            }
        }


        [Command("list"), RequireOwner, Alias("array", "out"), Summary("Outputs full video list")]
        public async Task ListAsync()
        {
            List<Video> videos = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(@"..\..\..\TMP_List.json"));

            if (videos != null)
            {
                await ReplyAsync("**Videos:**\n");
                for (int i = 0; i < videos.Count; i++)
                {
                    string filename = videos[i].Url.Replace("https://b23.tv/", string.Empty);
                    filename = filename.Replace("https://youtu.be/", string.Empty);
                    if (videos[i].Comment != null)
                    {
                        await Context.Channel.SendFileAsync(@$"..\..\..\Thumbnails\{filename}.jpg", $"————————————————————————\n**{i + 1}:**\t{videos[i].Video_title}\t |\t {videos[i].Channel_name} ({videos[i].Upload_date})\n`{videos[i].Comment}`\n<{videos[i].Url}>");
                    }
                    else
                    {
                        await Context.Channel.SendFileAsync(@$"..\..\..\Thumbnails\{filename}.jpg", $"————————————————————————\n**{i + 1}:**\t{videos[i].Video_title}\t |\t {videos[i].Channel_name} ({videos[i].Upload_date})\n<{videos[i].Url}>");
                    }
                }
            }
            else
            {
                await ReplyAsync("List is empty");
            }
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
                    {
                        writer.WriteLine($"{videos[i].Channel_name} - {videos[i].Video_title}:\n{videos[i].Url}");
                    }
                }

                await Context.Channel.SendFileAsync(@$"..\..\..\TMP_List.txt", "**Generated pastebin:**");
            }
            else
            {
                await ReplyAsync("List is empty");
            }
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

            if (videos == null)
            {
                await ReplyAsync("List is empty");
                return;
            }

            string replyString1 = $"**{1}:**\t{videos[0].Video_title}\t |\t {videos[0].Channel_name} ({videos[0].Upload_date})\n<{videos[0].Url}>";
            string replyString2 = replyString1;
            for (int i = 1; i < videos.Count; i++)
            {
                replyString1 += $"\n**{i + 1}:**\t{videos[i].Video_title}\t |\t {videos[i].Channel_name} ({videos[i].Upload_date})\n<{videos[i].Url}>";
                if (replyString1.Length >= 2000)
                {
                    await ReplyAsync(replyString2);
                    replyString1 = $"**{i + 1}:**\t{videos[i].Video_title}\t |\t {videos[i].Channel_name} ({videos[i].Upload_date})\n<{videos[i].Url}>";
                }

                replyString2 = replyString1;
            }

            if (replyString2.Length > 0)
            {
                await ReplyAsync(replyString2);
            }
        }


        [Command("help"), Alias("?", "asist"), Summary("Displays all commands")]
        public async Task HelpAsync()
        {
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
            await ReplyAsync(embed: embed);
        }


        [Command("clear"), RequireOwner, Summary("Clears TMP_List.json")]
        public async Task ClearAsync()
        {
            using (StreamWriter writer = new StreamWriter(@"..\..\..\TMP_List.json"))
            {
                writer.WriteLine(string.Empty);
            }

            await ReplyAsync("TMP List have been cleared.");
        }


        [Command("thumbnail", true), Summary("Generates thumbnail for the episode")]
        public async Task ThumbnailAsync()
        {
            await Context.Channel.TriggerTypingAsync();
            string message = Context.Message.Content;
            string[] words = message.Split(' ');
            string episodeNumber = words[2];

            Bitmap thumbnail = Thumbnail.Generate(episodeNumber);
            thumbnail.Save(@$"..\..\..\Thumbnails\Thumbnail {episodeNumber}.jpg");
            await Context.Channel.SendFileAsync(@$"..\..\..\Thumbnails\Thumbnail {episodeNumber}.jpg", $"Thumbnail for episode: **{episodeNumber}**");
        }


        [Command("remove", true), Alias("delete", "-"), Summary("Removes video from the list")]
        public async Task RemoveAsync()
        {
            // cleaning url
            string message = Context.Message.Content;
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

                if (vidList == null)
                {
                    return;
                }

                for (int i = 0; i < vidList.Count; i++)
                {
                    if (words[2] == vidList[i].Url)
                    {
                        vidList.RemoveAt(i);
                        await ReplyAsync($"Video with URL: **<{words[2]}>** got removed");
                        File.WriteAllText(path, JsonConvert.SerializeObject(vidList, Formatting.Indented));
                        return;
                    }
                }

                await ReplyAsync($"No video was removed");
            }
        }


        [Command("list count"), Summary("Counts amount of stored videos")]
        public async Task CountAsync()
        {
            string path = @"..\..\..\TMP_List.json";
            if (File.Exists(path))
            {
                List<Video> vidList = JsonConvert.DeserializeObject<List<Video>>(File.ReadAllText(path));

                if (vidList == null)
                {
                    return;
                }

                await ReplyAsync($"The list currently has **{vidList.Count}** videos stored");
            }
        }
    }
}
