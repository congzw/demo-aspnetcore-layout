using System;
using System.Collections.Generic;
using System.Linq;

namespace NbApp.Web.Models
{
    public class FooModel
    {
        public static List<FooModel> Items = new List<FooModel>();

        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime UpdateAt { get; set; } = new DateTime(2000, 1, 1);

        public void AddOrUpdate(FooModel model)
        {
            var theOne = Items.FirstOrDefault(x => x.Id == model.Id);
            if (theOne == null)
            {
                theOne = new FooModel();
                theOne.Id = model.Id;
            }

            theOne.Title = model.Title;
            theOne.UpdateAt = DateTime.Now;
        }
    }

    public class FooRepo
    {
        public static FooRepo Instance = new FooRepo();

        public List<FooModel> Items = new List<FooModel>();

        public FooRepo()
        {
            for (int i = 0; i < 3; i++)
            {
                Items.Add(new FooModel { Id = i.ToString("00"), Title = $"Demo-{i:00}" });
            }
        }

        public void AddOrUpdate(FooModel model)
        {
            var theOne = Items.FirstOrDefault(x => x.Id == model.Id);
            if (theOne == null)
            {
                theOne = new FooModel();
                theOne.Id = model.Id;
            }

            theOne.Title = model.Title;
            theOne.UpdateAt = DateTime.Now;
        }
    }
}