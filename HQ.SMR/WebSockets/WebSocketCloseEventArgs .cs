namespace HQ.SMR.WebSockets
{
    using System;

    /// <summary>
    /// WebSocket 断开连接事件参数
    /// </summary>
    public class WebSocketCloseEventArgs : EventArgs
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// 断开原因（如：服务器关闭、连接异常、超时等）
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 详细信息（如：异常消息、错误码等）
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 断开时间
        /// </summary>
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// 关闭状态码（可选）
        /// </summary>
        public int? CloseCode { get; set; }

        /// <summary>
        /// 是否需要自动重连
        /// </summary>
        public bool AutoReconnect { get; set; } = true;
    }
}
