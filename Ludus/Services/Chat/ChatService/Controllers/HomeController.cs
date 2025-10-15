using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers
{
    [ApiController]
    [Route("/")] 
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ContentResult Get()
        {
            return new ContentResult
            {
                Content = "<html><body><h1>ChatService is running</h1></body></html>",
                ContentType = "text/html"
            };
        }
    }
}
