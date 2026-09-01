using IDS.Base;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using IDS.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace HQ.SMR.WebSockets
{


    /// <summary>
    /// WebSocket 客户端信息
    /// </summary>
    public class WebSocketClientInfo
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }  // 对应传入的 id 参数
        public WebSocket WebSocket { get; set; }
        public DateTime ConnectedTime { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public bool IsAlive => WebSocket?.State == WebSocketState.Open;
    }

    /// <summary>
    /// WebSocket 断开事件参数
    /// </summary>
    //public class WebSocketCloseEventArgs : EventArgs
    //{
    //    public string ConnectionId { get; set; }
    //    public string UserId { get; set; }
    //    public string Reason { get; set; }
    //    public string Detail { get; set; }
    //    public DateTime CloseTime { get; set; }
    //    public int? CloseCode { get; set; }
    //}

    /// <summary>
    /// 登录 WebSocket 服务
    /// </summary>
    public class RackAlarmWsService
    {
        private readonly ILogger<RackAlarmWsService> _logger;

        // 存储所有连接：ConnectionId -> ClientInfo
        private static readonly ConcurrentDictionary<string, WebSocketClientInfo> _connections = new();

        // 存储用户连接映射：UserId -> ConnectionId（用于快速查找）
        private static readonly ConcurrentDictionary<string, string> _userConnectionMap = new();

        // 事件定义
        public event EventHandler<WebSocketCloseEventArgs> OnClientDisconnected;
        public event EventHandler<WebSocketClientInfo> OnClientConnected;
        public event EventHandler<(string ConnectionId, string Message)> OnMessageReceived;

        public RackAlarmWsService(ILogger<RackAlarmWsService> logger)
        {
            _logger = logger;
            //注册消息发送事件
            IdsMessageHandler<object>.OnSendMessage += NoticeMessageEvent;
        }

        public IdsResult<object> NoticeMessageEvent(MessageContent<object> message) {
            BroadcastAsync(message);
            return IdsResult<object>.ok();
        }
        /// <summary>
        /// 处理 WebSocket 连接（由 Controller 调用）
        /// </summary>
        public async Task HandleWebSocketConnection(WebSocket webSocket, string userId)
        {
            string connectionId = Guid.NewGuid().ToString("N");
            var clientInfo = new WebSocketClientInfo
            {
                ConnectionId = connectionId,
                UserId = userId ?? "anonymous",
                WebSocket = webSocket,
                ConnectedTime = DateTime.Now,
                LastHeartbeat = DateTime.Now
            };

            try
            {
                // 保存连接
                _connections.TryAdd(connectionId, clientInfo);
                if (!string.IsNullOrEmpty(userId))
                {
                    _userConnectionMap.TryAdd(userId, connectionId);
                }

                _logger.LogInformation($"🟢 WebSocket 连接建立: ConnectionId={connectionId}, UserId={userId}");

                // 触发连接事件
                OnClientConnected?.Invoke(this, clientInfo);

                // 发送连接成功消息
                await SendMessageAsync(webSocket, new { type = "connected", connectionId, userId });

                // 开始接收消息（递归方式，无 while 循环）
                await ReceiveMessagesAsync(clientInfo);

                _logger.LogInformation($"🔴 WebSocket 连接断开: ConnectionId={connectionId}, UserId={userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ WebSocket 处理异常: ConnectionId={connectionId}");
                await DisconnectClientAsync(connectionId, "连接异常", ex.Message);
            }
            finally
            {
                // 清理资源
                await DisconnectClientAsync(connectionId, "连接关闭", "清理资源");
            }
        }

        /// <summary>
        /// 接收消息（递归替代 while 循环）
        /// </summary>
        private async Task ReceiveMessagesAsync(WebSocketClientInfo clientInfo)
        {
            if (clientInfo.WebSocket.State != WebSocketState.Open)
                return;

            try
            {
                var buffer = new byte[4096];
                var segment = new ArraySegment<byte>(buffer);

                // 异步接收消息
                var result = await clientInfo.WebSocket.ReceiveAsync(segment, CancellationToken.None);

                // 处理关闭消息
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await clientInfo.WebSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed by client",
                        CancellationToken.None
                    );
                    await DisconnectClientAsync(clientInfo.ConnectionId, "客户端关闭", "Normal closure");
                    return;
                }

                // 解析消息
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // 处理心跳
                if (message == "ping")
                {
                    clientInfo.LastHeartbeat = DateTime.Now;
                    await SendMessageAsync(clientInfo.WebSocket, "pong");
                }
                else
                {
                    // 触发消息事件
                    OnMessageReceived?.Invoke(this, (clientInfo.ConnectionId, message));
                    _logger.LogDebug($"📨 收到消息: ConnectionId={clientInfo.ConnectionId}, Message={message}");

                    // 处理业务消息（如登录请求等）
                    await ProcessBusinessMessageAsync(clientInfo, message);
                }

                // 继续接收下一条消息（递归）
                if (clientInfo.WebSocket.State == WebSocketState.Open)
                {
                    await ReceiveMessagesAsync(clientInfo);
                }
            }
            catch (WebSocketException ex)
            {
                _logger.LogWarning($"⚠️ WebSocket 异常: ConnectionId={clientInfo.ConnectionId}, Error={ex.Message}");
                await DisconnectClientAsync(clientInfo.ConnectionId, "WebSocket异常", ex.Message);
            }
            catch (OperationCanceledException)
            {
                // 主动取消
                _logger.LogInformation($"接收已取消: ConnectionId={clientInfo.ConnectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 接收消息异常: ConnectionId={clientInfo.ConnectionId}");
                await DisconnectClientAsync(clientInfo.ConnectionId, "接收异常", ex.Message);
            }
        }

        /// <summary>
        /// 处理业务消息
        /// </summary>
        private async Task ProcessBusinessMessageAsync(WebSocketClientInfo clientInfo, string message)
        {
            try
            {
                // 这里可以根据消息内容处理不同的业务逻辑
                // 例如：登录、订阅、请求数据等

                // 示例：如果消息是 JSON 格式
                // var jsonMessage = JsonSerializer.Deserialize<dynamic>(message);
                // string messageType = jsonMessage?.type;

                // 根据消息类型处理
                // switch (messageType)
                // {
                //     case "login":
                //         await HandleLoginAsync(clientInfo, message);
                //         break;
                //     case "heartbeat":
                //         clientInfo.LastHeartbeat = DateTime.Now;
                //         break;
                //     default:
                //         // 未知消息类型
                //         break;
                // }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理业务消息失败: {message}");
            }
        }

        /// <summary>
        /// 发送消息给指定客户端
        /// </summary>
        public async Task SendMessageAsync(string connectionId, object message)
        {
            if (!_connections.TryGetValue(connectionId, out var clientInfo))
            {
                _logger.LogWarning($"❌ 客户端不存在: {connectionId}");
                return;
            }

            if (clientInfo.WebSocket.State != WebSocketState.Open)
            {
                await DisconnectClientAsync(connectionId, "WebSocket 未连接", "State not open");
                return;
            }

            try
            {
                await SendMessageAsync(clientInfo.WebSocket, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ 发送消息失败: {connectionId}");
                await DisconnectClientAsync(connectionId, "发送失败", ex.Message);
            }
        }

        /// <summary>
        /// 发送消息给指定用户
        /// </summary>
        public async Task SendMessageToUserAsync(string userId, object message)
        {
            if (_userConnectionMap.TryGetValue(userId, out var connectionId))
            {
                await SendMessageAsync(connectionId, message);
            }
            else
            {
                _logger.LogWarning($"❌ 用户不在线: {userId}");
            }
        }

        /// <summary>
        /// 广播消息给所有客户端
        /// </summary>
        public async Task BroadcastAsync(object message)
        {
            var tasks = new List<Task>();
            foreach (var connectionId in _connections.Keys)
            {
                tasks.Add(SendMessageAsync(connectionId, message));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 广播消息给指定用户列表
        /// </summary>
        public async Task BroadcastToUsersAsync(IEnumerable<string> userIds, object message)
        {
            var tasks = new List<Task>();
            foreach (var userId in userIds)
            {
                tasks.Add(SendMessageToUserAsync(userId, message));
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 发送消息（内部方法）
        /// </summary>
        private async Task SendMessageAsync(WebSocket webSocket, object message)
        {
            string json = JsonConvert.SerializeObject(message);//System.Text.Json.JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public async Task DisconnectClientAsync(string connectionId, string reason, string detail = "")
        {
            if (!_connections.TryRemove(connectionId, out var clientInfo))
                return;

            try
            {
                // 移除用户映射
                if (!string.IsNullOrEmpty(clientInfo.UserId))
                {
                    _userConnectionMap.TryRemove(clientInfo.UserId, out _);
                }

                // 关闭 WebSocket
                if (clientInfo.WebSocket.State == WebSocketState.Open)
                {
                    await clientInfo.WebSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        reason,
                        CancellationToken.None
                    );
                }

                clientInfo.WebSocket?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"关闭 WebSocket 异常: {connectionId}");
            }
            finally
            {
                _logger.LogInformation($"🔴 客户端断开: ConnectionId={connectionId}, UserId={clientInfo.UserId}, Reason={reason}");

                // 🔥 触发断开事件（通知订阅者）
                var args = new WebSocketCloseEventArgs
                {
                    ConnectionId = connectionId,
                    UserId = clientInfo.UserId,
                    Reason = reason,
                    Detail = detail,
                    CloseTime = DateTime.Now,
                    CloseCode = 1000
                };
                OnClientDisconnected?.Invoke(this, args);

                // 执行清理逻辑（可选）
                await PerformCleanupAsync(clientInfo);
            }
        }

        /// <summary>
        /// 执行清理逻辑（断开后的处理）
        /// </summary>
        private async Task PerformCleanupAsync(WebSocketClientInfo clientInfo)
        {
            _logger.LogInformation($"执行清理逻辑: UserId={clientInfo.UserId}");

            // 🔥 在这里执行你的清理逻辑：
            // 1. 更新数据库用户状态为离线
            // 2. 释放相关资源
            // 3. 通知其他服务
            // 4. 记录日志等

            // 示例：更新在线状态
            // await _userService.UpdateOnlineStatusAsync(clientInfo.UserId, false);

            await Task.CompletedTask;
        }

        /// <summary>
        /// 关闭所有连接
        /// </summary>
        public async Task CloseAllConnectionsAsync(string reason = "服务端关闭")
        {
            var connectionIds = _connections.Keys.ToList();
            foreach (var connectionId in connectionIds)
            {
                await DisconnectClientAsync(connectionId, reason);
            }
            _connections.Clear();
            _userConnectionMap.Clear();
            _logger.LogInformation("所有 WebSocket 连接已关闭");
        }

        /// <summary>
        /// 获取所有在线连接
        /// </summary>
        public List<WebSocketClientInfo> GetOnlineClients()
        {
            return _connections.Values.ToList();
        }

        /// <summary>
        /// 获取指定用户的连接信息
        /// </summary>
        public WebSocketClientInfo GetClientInfo(string userId)
        {
            if (_userConnectionMap.TryGetValue(userId, out var connectionId))
            {
                return _connections.GetValueOrDefault(connectionId);
            }
            return null;
        }

        /// <summary>
        /// 获取在线用户数量
        /// </summary>
        public int GetOnlineCount() => _connections.Count;

        /// <summary>
        /// 检查用户是否在线
        /// </summary>
        public bool IsUserOnline(string userId)
        {
            return _userConnectionMap.ContainsKey(userId) &&
                   _connections.TryGetValue(_userConnectionMap[userId], out var info) &&
                   info.IsAlive;
        }
    }
}
