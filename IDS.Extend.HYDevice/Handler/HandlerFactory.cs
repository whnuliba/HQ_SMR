using IDS.Base.Utils;
using IDS.Device.Communication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.Handler
{
    public class HandlerFactory
    {
        private static readonly Lazy<HandlerFactory> _instance = new Lazy<HandlerFactory>(() => new HandlerFactory());

        private List<IReceiveHandler> _handlers;
        private HandlerFactory() {
            _handlers = ReceiveHandlerLoader.LoadHandlers("IDS.Extend.HYDevice.ReceiveHandler");
        }
        public static HandlerFactory Instance => _instance.Value;
        public IReceiveHandler GetHandler(byte value) {
            if (_handlers == null) return null;
            string result = "0x" + value.ToString("X2");
            return _handlers.Where(c => c.ReceiveKey == result).FirstOrDefault();
        }
    }
}
