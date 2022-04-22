namespace TMP_Bot_2
{
    using System.Threading.Tasks;
    using AngleSharp;

    class Parse
    {
        public static async Task<string> NameAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);

            using var doc = await context.OpenAsync(url);
            var title = doc.Title;

            return title;
        }

        public static async Task<string> DateAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            using var context = BrowsingContext.New(config);

            using var doc = await context.OpenAsync(url);

            var date = doc.QuerySelectorAll("van-icon-videodetails_like");
            return date[0].TextContent;
        }
    }
}
