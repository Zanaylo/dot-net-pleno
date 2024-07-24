using Microsoft.AspNetCore.Mvc;

namespace StallosDotnetPleno.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StallosDotnetPlenoController : ControllerBase
    {
        private readonly ILogger<StallosDotnetPlenoController> _logger;

        public StallosDotnetPlenoController(ILogger<StallosDotnetPlenoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Bem-vindo ao teste para desenvoledor Pleno da Stallos Tecnologia";
        }
    }
}
