using Markdig.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NbApp.Web.Models
{
    public class MyHtmlHelper
    {
        public static MyHtmlHelper Instance = new MyHtmlHelper();

        public async Task<string> GetPartialContent(IHtmlHelper helper, HtmlEncoder encoder, string partialName, object model = null)
        {
            using (var sw = new StringWriter())
            {
                var partialHtml = model == null ? await helper.PartialAsync(partialName) : await helper.PartialAsync(partialName, model);
                partialHtml.WriteTo(sw, encoder);
                return sw.ToString();
            }
        }
    }

    public static class HtmlHelperExtensions
    {
        public static Task<string> GetPartialContent(this IHtmlHelper helper, HtmlEncoder encoder, string partialName, object model = null)
        {
            return MyHtmlHelper.Instance.GetPartialContent(helper, encoder, partialName, model);
        }
    }
}