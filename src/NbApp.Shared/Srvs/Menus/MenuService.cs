namespace NbApp.Srvs.Menus
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Herf { get; set; }
        public object Tag { get; set; }

        public static T Create<T>(string id, string parentId, string title, string icon, string herf) where T : MenuItem, new()
        {
            return new T()
            {
                Id = id,
                ParentId = parentId,
                Title = title,
                Icon = icon,
                Herf = herf
            };
        }
    }
}