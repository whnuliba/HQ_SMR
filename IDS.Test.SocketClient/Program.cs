// See https://aka.ms/new-console-template for more information

using IDS.Common;
using IDS.Device.Communication;
using IDS.HQ.HYDevice.Protocol;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {

        UpdClientListener updClientListener = new UpdClientListener("127.0.0.1", 9999);
        updClientListener.Connect();
        updClientListener.OnReceive += (Isender, data) =>
        {

            return IdsResult<string>.ok();
        };
        //UdpClient udpClient = new UdpClient();
        int i = 0;
        while (true) {

            byte[] bitMessage = DeviceMessage.GetTestfMessage();//GetModeSwitchMessage(1);


            string message = $"{DateTime.Now.Ticks}:{new Guid().ToString("N")}";
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            bool res =  updClientListener.Send(bitMessage, bitMessage.Length);
            if(res) Console.WriteLine($"发送成功,长度{bitMessage.Length}");
            else Console.WriteLine("发送失败");
            Thread.Sleep(1000);
            //i ++;
           // if (i > 10) break;
        }
        Console.ReadLine();

    }
}