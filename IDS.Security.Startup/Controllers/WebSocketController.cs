using IDS.Ioc;
using IDS.Security.Api.Controller;
using Microsoft.AspNetCore.Mvc;

namespace IDS.Bpms.Api.Controllers
{
    //[ApiController]
    //[PropertiesAutowired]
    public class WebSocketController : ControllerBase
    {
        [Route("/security/{id?}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Anonymous]
        public async Task Webscoket(string id) {

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var service = (LoginWsService)ContainerUtils.AutofacServiceProvider.GetService(typeof(LoginWsService));
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await service.HandleWebSocketConnection(webSocket, id);
            }
        }
    }
}
