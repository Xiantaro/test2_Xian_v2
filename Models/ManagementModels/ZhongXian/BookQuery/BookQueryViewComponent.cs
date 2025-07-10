using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace test2.Models.ManagementModels.ZhongXian.BookQuery
{
    public class BookQueryViewComponent : ViewComponent
    {
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return  View();
        }
    }
}
