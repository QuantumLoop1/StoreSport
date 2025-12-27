using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StoreSport.Pages
{
    public class CompletedModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        public void OnGet()
        {
        }
    }
}
