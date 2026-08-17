// See https://aka.ms/new-console-template for more information

using IDS.Base.Utils;
using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice;
using IDS.SMR.Bootstrap;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Program
{
    private static void Main(string[] args)
    {

        ushort smrSocketPort = 9999;
        IServerConnection serverConnection = new HYBootstrap().RegisterServiceAndStartup(new IdsEndPoint(null, smrSocketPort));
        //注册全局连接器
        ServerConnectionHolder.SetConnection(serverConnection);

        //写入一个测试货架
        // <Shelf No="B001" IP="10.40.135.10" Port="5000" LocalIP="localhost" LocalPort="8902" Enabled="Y" Alarm="Y" InductiveShelf="Y" AQty="656" BQty="656" />
        RackNode rackNode = new RackNode { 
           No = "B001",
           Port = 5000,
           IP="127.0.0.1",
           LocalIP = "127.0.0.1",
           LocalPort = 8902,
           AQty = 656,
           BQty = 656,

        };
        SmartMaterialRackNode.Instance.AddNode(rackNode);
        ConcurrentDictionary<byte[],string> ss = new();
        byte[] b1 = { 1, 2, 3, 4, 5 };
        byte[] b2 = { 1, 2, 3, 4, 5 };
        Console.WriteLine(b1 == b2);
        Console.WriteLine(b1.Equals(b2));
        ss.AddOrUpdate(b1, "1", (key, oldValue) => "1");
        ss.AddOrUpdate(b2, "2", (key, oldValue) => "2");
        string c = "";

        //bootstrap.StartAll();
        //IServerConnection serverConnection = bootstrap.GetService(smrSocketPort);
        /*
        BaseUtil baseUtil = new BaseUtil();
        long a = baseUtil.GetSnowFlakeId(1L, 1L);
        byte[] arr = BitConverter.GetBytes(a);
        string str  = BitConverter.ToString(arr).Replace("-","");
        string hex = Convert.ToString(a, 16);
        //UdpClient udpClient = new UdpClient();
        UpdServiceListener updServiceListener = new UpdServiceListener(null, 9900);
        updServiceListener.OnReceive += (sender, remoteAddress, remotePort, pData) =>
        {
            string strCmd = string.Empty;
            if (pData != null && pData.Length>=15)
            {
                byte[] result = new byte[8];
                Array.Copy(pData, 3, result, 0, 8);
                //Array.Reverse(result);
                long value = BitConverter.ToInt64(result, 0);
                strCmd = value.ToString();//Encoding.UTF8.GetString(pData);
            }
            string str = $"客户端IP{remoteAddress};客户端端口{remotePort};内容是ID{strCmd}";
            Console.WriteLine(str);
            return IdsResult<string>.ok();
        };
        updServiceListener.Start();
        */
        Console.WriteLine("Hello, World!");
        Console.ReadLine();
    }
}