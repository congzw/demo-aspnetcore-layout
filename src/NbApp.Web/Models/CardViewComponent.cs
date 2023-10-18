using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NbApp.Web.Models
{
    public class CardVo
    {
        public string Header { get; set; }
        public string Body { get; set; }
    }

    public class CardViewComponent : ViewComponent
    {
        //search path:
        ///Views/{Controller Name}/Components/{View Component Name}/{View Name}
        ///Views/Shared/Components/{View Component Name}/{View Name}
        ///Pages/Shared/Components/{View Component Name}/{View Name}
        ///Areas/{Area Name}/Views/Shared/Components/{View Component Name}/{View Name}

        //We recommend naming the view file Default.cshtml
        //PriorityList => "Views/Shared/Components/PriorityList/Default.cshtml"

        public async Task<IViewComponentResult> InvokeAsync(string header, string body)
        {
            await Task.CompletedTask;
            return View(new CardVo { Header = header, Body = body });
        }
    }
}