using Microsoft.AspNetCore.Mvc;

namespace PropertyManager.WEB.Controllers
{
    // TODO: Refactor this controller. Delete method should be moved to UnitsController and use IUnitApiClient instead of HttpClient
    [ApiController]
    [Route("api-proxy")]
    public class ApiProxyController : ControllerBase
    {
        private readonly HttpClient _client;

        public ApiProxyController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        
    }
}
