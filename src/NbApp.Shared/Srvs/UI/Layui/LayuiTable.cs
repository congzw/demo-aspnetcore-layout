using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace NbApp.Srvs.UI.Layui
{
    public class LayuiTable
    {
        public static LayuiTable Create()
        {
            return new LayuiTable();
        }

        public List<LayuiTableColumn> cols { get; set; } = new List<LayuiTableColumn>();
        public List<object> data { get; set; } = new List<object>();
        public int limit { get; set; } = 10;
        public List<int> limits { get; set; } = new List<int>() { 10, 20, 50, 100, 1000 };
    }

    public class LayuiTableColumn
    {
        public static List<LayuiTableColumn> CreateForType(Type theType)
        {
            var items = new List<LayuiTableColumn>();

            var properties = theType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                var item = new LayuiTableColumn();
                item.field = property.Name;

                item.title = property.Name;
                var desc = property.GetCustomAttribute<DescriptionAttribute>()?.Description;
                if (!string.IsNullOrWhiteSpace(desc))
                {
                    item.title = desc;
                }

                items.Add(item);
            }

            return items;
        }

        public static List<LayuiTableColumn> CreateForDict(IDictionary<string, object> dict)
        {
            var items = new List<LayuiTableColumn>();
            foreach (var dictItem in dict)
            {
                var item = new LayuiTableColumn();
                item.field = dictItem.Key;
                item.title = dictItem.Key;
                items.Add(item);
            }

            return items;
        }

        public string field { get; set; }
        public string title { get; set; }
        public bool sort { get; set; } = true;
        public int? width { get; set; }
    }

    public static class LayuiTableExts
    {
        public static LayuiTable WithColumns(this LayuiTable table, List<LayuiTableColumn> cols)
        {
            if (cols != null)
            {
                table.cols = cols;
            }
            return table;
        }
        public static LayuiTable WithData(this LayuiTable table, List<object> data)
        {
            if (data != null)
            {
                table.data = data;
            }
            return table;
        }
        public static LayuiTable WithPager(this LayuiTable table, int pageSize)
        {
            table.limit = pageSize;
            return table;
        }
    }

    #region how to use

    public class DemoTableData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        public static LayuiTable CreateDemoTable()
        {
            var cols = LayuiTableColumn.CreateForType(typeof(DemoTableData));
            var data = Seed(123);

            var table = LayuiTable.Create()
                .WithColumns(cols)
                .WithData(data)
                .WithPager(10);

            return table;
        }

        private static List<object> Seed(int seedCount)
        {
            var list = new List<object>();

            for (int i = 1; i <= seedCount; i++)
            {
                var index = i.ToString("000");
                list.Add(new DemoTableData() { Id = index, Title = "user_" + index });
            }

            return list;
        }
    }

    #endregion
}
