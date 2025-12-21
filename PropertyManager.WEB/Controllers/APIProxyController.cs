using Microsoft.AspNetCore.Mvc;

namespace PropertyManager.WEB.Controllers
{
    [Route("api-proxy")]
    public class ApiProxyController : Controller
    {
        private readonly HttpClient _client;

        public ApiProxyController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        [HttpDelete("units/{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var response = await _client.DeleteAsync($"/api/units/{id}");
            return StatusCode((int)response.StatusCode);
        }
    }
}
