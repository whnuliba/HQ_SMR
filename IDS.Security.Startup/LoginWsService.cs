using IDS.Base;
using IDS.Security.IService;
using IDS.Security.IService.DTO;
using IDS.Security.Service;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace IDS.Bpms.Api
{
    public class LoginWsService
    {
        //public  List<WebSocket> _sockets = new();
        public static ConcurrentDictionary<string, WebSocket> sessionPools = new ConcurrentDictionary<string, WebSocket>();
        public IUserInfoService UserInfoService { get; set; }
        public LoginWsService(IUserInfoService userInfo) { 
           if(UserInfoService==null)
                UserInfoService = userInfo;
        }
        public async Task HandleWebSocketConnection(WebSocket socket,string clientId)
        {
            //_sockets.Add(socket);
            sessionPools.AddOrUpdate(clientId, socket, (k, v) => socket);
            var buffer = new byte[32];
            CancellationToken token = new CancellationToken();
            while (socket.State == WebSocketState.Open)
            {
                try {
                   

                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, token);
                        break;
                    }
                    string sessionKey = Encoding.UTF8.GetString(buffer);
                    UserSessionDto userSessionDto = new UserSessionDto();
                    userSessionDto.SessionKey = sessionKey;
                    string[] cIds = clientId.Split("_");
                    userSessionDto.UserName = cIds[0];
                    UserSessionDto resultDto = await UserInfoService.IsLogin(userSessionDto);
                    await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ResponseEntity<UserSessionDto>.Success(resultDto))), WebSocketMessageType.Text, true, token);
                }
                catch (Exception e) {
                    string sessionKey = Encoding.UTF8.GetString(buffer);
                    UserSessionDto userSessionDto = new UserSessionDto();
                    userSessionDto.SessionKey = sessionKey;
                    string[] cIds = clientId.Split("_");
                    userSessionDto.UserName = cIds[0];
                    await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ResponseEntity<UserSessionDto>.Success(300,null))), WebSocketMessageType.Text, true, token);
                    socket.Dispose();
                }

                //Encoding.ASCII.GetString(buffer);
                //foreach (var s in _sockets)
                //{
                //    await s.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, default);
                //}
            }
            sessionPools.Remove(clientId,out socket);
        }
    }
}
