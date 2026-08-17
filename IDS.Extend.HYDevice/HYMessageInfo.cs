using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice
{
    public class HYMessageInfo
    {
        //客户端IP
        public string ClientIp { get; set; }
        //客户端端口
        public ushort ClientPort { get; set; }
        //报文内容
        public byte[] MessageContent { get; set; }

    }
}
