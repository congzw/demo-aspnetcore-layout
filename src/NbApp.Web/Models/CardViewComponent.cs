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

        //recommend naming the view file Default.cshtml
        //Foo => "Views/Shared/Components/Foo/Default.cshtml"

        //Open the _ViewImports.cshtml file and add the following line to the existing code:
        //@addTagHelper *, DemoLayout.Web
        //Then we can use tag as:
        //<vc:card header="A1" body="AAA"></vc:card>

        public async Task<IViewComponentResult> InvokeAsync(string header, string body)
        {
            await Task.CompletedTask;
            return View(new CardVo { Header = header, Body = body });
        }
    }
}