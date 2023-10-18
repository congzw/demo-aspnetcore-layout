using System.Collections.Generic;

namespace NbApp.Srvs.Menus
{
    public class MenuItem
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Herf { get; set; }

        public static MenuItem Create(string id, string parentId, string title, string icon, string herf)
        {
            return new MenuItem()
            {
                Id = id,
                ParentId = parentId,
                Title = title,
                Icon = icon,
                Herf = herf
            };
        }
    }

    public class MenuService
    {
        public List<MenuItem> GetMenus()
        {
            var items = new List<MenuItem>();
            //todo: read from source

            items.Add(MenuItem.Create("1", "", "1", "", "#"));
            items.Add(MenuItem.Create("2", "", "2", "", "#"));
            items.Add(MenuItem.Create("3", "", "3", "", "#"));

            items.Add(MenuItem.Create("1.1", "1", "1.1", "", "#"));
            items.Add(MenuItem.Create("1.2", "1", "1.2", "", "#"));
            items.Add(MenuItem.Create("1.3", "1", "1.3", "", "#"));

            items.Add(MenuItem.Create("1.1.1", "1.1", "1.1.1", "", "#"));
            items.Add(MenuItem.Create("1.1.2", "1.1", "1.1.2", "", "#"));
            items.Add(MenuItem.Create("1.1.3", "1.1", "1.1.3", "", "#"));

            return items;
        }
    }
}