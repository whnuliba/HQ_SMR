using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Api.Controller
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task Broadcast(string message)
        {
            //"ReceiveMessage" 是客户端注册的接收消息的回调函数的名字
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public async Task SendMessage(string message)
        {
            var connectionId = Context.ConnectionId;
            await Clients.All.SendAsync("ReceiveMessage", connectionId, message);
        }
    }
}
