using IDS.Common;
using IDS.Device.Communication;
using IDS.Extend.HYDevice.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IDS.Extend.HYDevice.ReceiveHandler
{
    /// <summary>
    /// 查询储位状态返回
    /// </summary>
    public class HYLocationStatusQueryHandler : MessageHandler
    {
        public override string ReceiveKey { get; set; } = "0x11";

        public override IdsResult<object> Handle<E>(byte[] data, IdsSession session, DeviceCommand<E> command)
        {
            //储位从0开始索引
            //状态的分析
            //获取数据长度 
            int dateLength = (int)data[13] * 256 + (int)data[14];
            byte[] addrArray = new byte[dateLength];
            Array.Copy(data, 15, addrArray, 0, dateLength);
            string status = string.Empty;
            var LocationInfos = new List<LocationInfo>();
            for (int i = 0; i < addrArray.Length; i++)
            {
                LocationInfo location = new LocationInfo
                {
                    Addr = i,
                    Status = addrArray[i]
                };
                LocationInfos.Add(location);
            }
            return IdsResult<object>.ok(LocationInfos);
        }
    }
}
