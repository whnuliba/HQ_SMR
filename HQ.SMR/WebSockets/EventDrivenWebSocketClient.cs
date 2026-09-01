namespace HQ.SMR.WebSockets
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class EventDrivenWebSocketClient
    {
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private readonly object _lock = new object();

        // 事件定义
        public event EventHandler OnConnected;
        public event EventHandler<WebSocketCloseEventArgs> OnDisconnected;
        public event EventHandler<string> OnMessageReceived;
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// 连接 WebSocket（非阻塞）
        /// </summary>
        public async Task ConnectAsync(string url)
        {
            try
            {
                _webSocket = new ClientWebSocket();
                _cts = new CancellationTokenSource();

                // 注册连接完成回调
                var connectTask = _webSocket.ConnectAsync(new Uri(url), _cts.Token);

                // 连接完成后触发事件
                await connectTask.ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Console.WriteLine("✅ WebSocket 连接成功");
                        OnConnected?.Invoke(this, EventArgs.Empty);

                        // 启动接收（非阻塞）
                        StartReceiving();
                    }
                    else if (task.IsFaulted)
                    {
                        OnError?.Invoke(this, task.Exception);
                    }
                }, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
                NotifyDisconnected("连接失败", ex.Message);
            }
        }

        /// <summary>
        /// 启动接收（使用异步回调，无 while 循环）
        /// </summary>
        private void StartReceiving()
        {
            // 使用异步回调方式接收消息
            ReceiveAsync();
        }

        private async void ReceiveAsync()
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
                return;

            try
            {
                var buffer = new byte[4096];
                var segment = new ArraySegment<byte>(buffer);

                // 使用 ReceiveAsync 的异步回调
                await ReceiveInternalAsync(segment);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
                NotifyDisconnected("接收异常", ex.Message);
            }
        }

        private async Task ReceiveInternalAsync(ArraySegment<byte> buffer)
        {
            try
            {
                // 异步接收消息
                var result = await _webSocket.ReceiveAsync(buffer, _cts.Token);

                // 检查关闭消息
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed by server",
                        CancellationToken.None
                    );
                    NotifyDisconnected("服务器关闭", "Normal closure");
                    return;
                }

                // 处理消息
                string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                OnMessageReceived?.Invoke(this, message);

                // 递归调用：继续接收下一条消息（取代 while 循环）
                if (_webSocket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
                {
                    await ReceiveInternalAsync(buffer);
                }
            }
            catch (WebSocketException ex)
            {
                OnError?.Invoke(this, ex);
                NotifyDisconnected("连接异常", ex.Message);
            }
            catch (OperationCanceledException)
            {
                // 主动取消
                Console.WriteLine("接收已取消");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
                NotifyDisconnected("接收异常", ex.Message);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public async Task SendMessageAsync(string message)
        {
            if (_webSocket?.State != WebSocketState.Open)
            {
                OnError?.Invoke(this, new InvalidOperationException("WebSocket 未连接"));
                return;
            }

            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    _cts.Token
                );
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public async Task CloseAsync()
        {
            try
            {
                if (_webSocket?.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Client closed",
                        CancellationToken.None
                    );
                }
                _cts?.Cancel();
                _webSocket?.Dispose();
                _cts?.Dispose();
                Console.WriteLine("WebSocket 已关闭");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
        }

        private void NotifyDisconnected(string reason, string detail = "")
        {
            var args = new WebSocketCloseEventArgs
            {
                Reason = reason,
                Detail = detail,
                CloseTime = DateTime.Now
            };
            OnDisconnected?.Invoke(this, args);
        }

        public bool IsConnected => _webSocket?.State == WebSocketState.Open;
    }

}
