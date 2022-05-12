namespace TMP_Bot_2
{
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    /// <summary>
    /// Class that handles all the parsing.
    /// </summary>
    internal class Parse
    {
        public static async Task<string> YTTitleAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                string title = document.Title;
                title = title.Replace(" - YouTube", string.Empty);
                return title;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> YTDateAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var dateDoc = document.QuerySelector("#info-strings");        // //*[@id="info-strings"]/yt-formatted-string
                string date = dateDoc.Text();
                return date;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> YTUserAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var usernameDoc = document.QuerySelector("#text");     // //*[@id="text"]/a
                string username = usernameDoc.Text().Trim();
                return username;
            }
            catch
            {
                return "Null";
            }
        }

        public static string YTThumbnail(string url)
        {
            // making url into https://img.youtube.com/vi/[videoID]/0.jpg
            url = url.Replace("https://youtu.be/", string.Empty);
            url += "/0.jpg";
            string img_url = "https://img.youtube.com/vi/";
            img_url += url;

            return img_url;
        }

        public static async Task<string> BiliTitleAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var title = document.Title;
                return title;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> BiliDateAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var date2 = document.QuerySelector(".video-data");

                string date = date2.Text();
                date = date.Remove(0, date.Length - 19);
                date = date.Remove(date.Length - 9, 9);

                return date;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> BiliUserAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var username = document.QuerySelector(".username");
                string usernamestring = username.Text().Trim();
                return usernamestring;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> BiliThumbnailAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var thumbnail = document.QuerySelector("meta[itemprop='image']").GetAttribute("content");
                return thumbnail;
            }
            catch
            {
                return "Null";
            }
        }
    }
}
