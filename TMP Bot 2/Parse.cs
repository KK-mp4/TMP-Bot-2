namespace TMP_Bot_2
{
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;

    class Parse
    {
        public static async Task<string> TitleAsync(string url)
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

        public static async Task<string> DateAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                string date = document.QuerySelector(".video-data").Text();

                date = date.Remove(0, date.Length - 19);
                date = date.Remove(date.Length - 9, 9);

                return date;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> UserAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);
            using var document = await context.OpenAsync(url);

            try
            {
                var username = document.QuerySelector(".username").Text().Trim();
                return username;
            }
            catch
            {
                return "Null";
            }
        }

        public static async Task<string> ThumbnailAsync(string url)
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
