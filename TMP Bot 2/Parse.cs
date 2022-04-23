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
                var date = document.QuerySelector(".video-data").Text();
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
    }
}
