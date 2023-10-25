namespace NbApp.Web.Models
{
    public class ContentLinkModel
    {
        public string FileFullPath { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LanguageCss { get; set; }
        public string Link { get; set; }

        public static ContentLinkModel Create(string fileFullPath, string title, string content, string LanguageCss, string link)
        {
            return new ContentLinkModel()
            {
                FileFullPath = fileFullPath,
                Title = title,
                Content = content,
                LanguageCss = LanguageCss,
                Link = link
            };
        }
    }
}