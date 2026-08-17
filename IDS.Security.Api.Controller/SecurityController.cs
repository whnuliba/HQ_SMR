//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace IDS.Security.Api.Controller
//{
//    [ApiController]
//    [Route("[security]/[index]")]
//    public class WebSocketController : ControllerBase
//    {
//        private readonly DataContext _context;
//        private readonly IHubContext<SignalRHub> _hubContext;
//​
//        public WebSocketController(DataContext context, [FromServices] IHubContext<ChatHub> hubContext)
//        {
//            _context = context;
//            _hubContext = hubContext;
//        }
//​
//        [HttpGet]
//        public async Task<ActionResult<List<Message>>> GetMessage()
//        {
//            var List = await _context.Message.ToListAsync();
//            return Ok(List);
//        }
//​
//        [HttpPost]
//        public async Task<IActionResult> AddMessage(Message msg)
//        {

//            if (ModelState.IsValid)
//            {
//                _context.Message.Add(msg);
//                await _context.SaveChangesAsync();
//                SignalR websocket 消息
//               await _hubContext.Clients.All.SendAsync("ReceiveMessage", msg);
//                返回成功消息
//                return Content("添加成功！", "text/plain");
//            }
//            else
//            {
//                如果模型状态无效，返回错误消息
//                return BadRequest("无效的请求数据。");
//            }
//        }
//    }
//}
