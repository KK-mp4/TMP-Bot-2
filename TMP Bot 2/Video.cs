namespace TMP_Bot_2
{
    public class Video
    {
        public string Url { get; set; }

        public string Channel_name { get; set; }

        public string Video_title { get; set; }

        public string Upload_date { get; set; }

        public string? Comment { get; set; }

        public Video(string url, string channel_name, string video_title, string upload_date, string comment = null)
        {
            this.Url = url;
            this.Channel_name = channel_name;
            this.Video_title = video_title;
            this.Upload_date = upload_date;
            this.Comment = comment;
        }
    }
}
