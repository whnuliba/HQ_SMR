using IDS.Ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HQ.SMR.WebSockets;
namespace HQ.SMR
{

    [Route("api/[controller]")]
   // [PropertiesAutowired]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly RackAlarmWsService _loginWsService;
        private readonly ILogger<WebSocketController> _logger;

        public WebSocketController(RackAlarmWsService loginWsService, ILogger<WebSocketController> logger)
        {
            _loginWsService = loginWsService;
            _logger = logger;
        }

        [Route("/log/{id?}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task Webscoket(string id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    // 订阅断开事件（用于接收断开通知）
                    _loginWsService.OnClientDisconnected += (sender, args) =>
                    {
                        _logger.LogInformation($"🔴 WebSocket 断开通知: UserId={args.UserId}, Reason={args.Reason}");

                        // 🔥 在这里执行你的清理逻辑
                        // 例如：更新数据库、释放资源、通知其他模块等
                        HandleClientDisconnect(args);
                    };

                    // 订阅连接事件
                    _loginWsService.OnClientConnected += (sender, clientInfo) =>
                    {
                        _logger.LogInformation($"🟢 WebSocket 连接成功: UserId={clientInfo.UserId}");
                    };

                    // 订阅消息事件（可选）
                    _loginWsService.OnMessageReceived += (sender, args) =>
                    {
                        _logger.LogDebug($"📨 收到消息: {args.Message}");
                    };

                    // 接受 WebSocket 连接
                    var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                    // 处理连接
                    await _loginWsService.HandleWebSocketConnection(webSocket, id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"WebSocket 处理失败: {id}");
                    HttpContext.Response.StatusCode = 500;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                await HttpContext.Response.WriteAsync("WebSocket request expected");
            }
        }

        /// <summary>
        /// 处理客户端断开（清理逻辑）
        /// </summary>
        private void HandleClientDisconnect(WebSocketCloseEventArgs args)
        {
            _logger.LogInformation($"执行断开清理逻辑: UserId={args.UserId}");

            // 🔥 在这里执行你的业务清理逻辑：
            // 1. 更新数据库用户状态为离线
            // 2. 释放用户资源
            // 3. 通知其他模块用户已离线
            // 4. 记录操作日志等

            // 示例：更新在线状态
            // await _userService.UpdateOnlineStatusAsync(args.UserId, false);

            // 示例：发送通知
            // await _notificationService.NotifyUserOfflineAsync(args.UserId);
        }
    }
}
