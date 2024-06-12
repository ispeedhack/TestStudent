using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TestCreator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        protected JsonSerializerSettings JsonSettings;

        public BaseApiController()
        {
            JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        } 
}
}
