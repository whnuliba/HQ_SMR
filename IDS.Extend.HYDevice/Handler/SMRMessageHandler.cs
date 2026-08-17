using IDS.Base.Utils;
using IDS.Common;
using IDS.Device.Communication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.Extend.HYDevice.Handler
{
    //智能料架消息处理器
    public class SMRMessageHandler<E> : ISMRMessageHandler<E>
    {
        public IdsResult<E> ReceiveHandler(IdsEndPoint endPoint, IServerConnection serverConnection, byte[] dataArray)
        {
            //获取消息类别 
           // byte cmdType = dataArray[12];
           //这里还需要处理没有类型的情况
            string strCmd = string.Empty;
            if (dataArray != null && dataArray.Length >= 15)
            {

                long value = 0;
                byte cmdType = dataArray[12];
                string type = "0x" + cmdType.ToString("X2");
                int stratIndex = 3;
                int len = 8;
                byte[] result = new byte[len];
                Array.Copy(dataArray, stratIndex, result, 0, len);
                try
                {
                    value = BitConverter.ToInt64(result, 0);
                }
                catch
                {
                    BaseUtil baseUtil = new BaseUtil();
                    value = baseUtil.GetSnowFlakeId(1l, 1l);
                }
                var session  = SessionContext.Instance.GetSession(value);
                if (session == null) {
                    //若不存在就自己创建一个呆SessionKey的
                    byte[] key = new byte[10];
                    Array.Copy(dataArray, 1, key, 0, 10);
                    session = SessionContext.Instance.CreadeSession(value, serverConnection, dataArray);
                    session.SessionKey = key;
                }
                //处理Session
                session.ResponseData = dataArray;
                session.ResponseTime = DateTime.Now;
                session.ResponseEndPoint = endPoint;
                //HYConstant
                IReceiveHandler hander =  HandlerFactory.Instance.GetHandler(cmdType);
                if (hander == null) {
                  string  msgType = "Unknown type [0x" + Convert.ToString(cmdType, 16) + "]";
                    return IdsResult<E>.failure(msgType);
                }
                RackNode rack = SmartMaterialRackNode.Instance.GetRackNode(session.ResponseEndPoint.Address);
                //处理设备指令
                DeviceCommand<RackNode> command = new DeviceCommand<RackNode> { 
                 Message = dataArray,
                 IPEndPoint = session.ResponseEndPoint,
                 Extend= rack,
                };
                hander?.Handle(dataArray, session,command);
            }
            return IdsResult<E>.ok();
        }
        public string GetCharId(byte[] dataArray) {
                byte cmdType = dataArray[12];
                string type = "0x" + cmdType.ToString("X2");
                    byte[] result10 = new byte[10];
                    Array.Copy(dataArray, 1, result10, 0,10);
                    //Array.Reverse(result);
                    return Encoding.ASCII.GetString(result10);
        }
        public long GetLongId(byte[] dataArray)
        {

            byte[] result = new byte[8];
            Array.Copy(dataArray, 3, result, 0, 8);
            //Array.Reverse(result);
            long value;
            try
            {
                value = BitConverter.ToInt64(result, 0);
            }
            catch
            {
                BaseUtil baseUtil = new BaseUtil();
                value = baseUtil.GetSnowFlakeId(1l, 1l);
            }
            return value;
        }
    }
}
