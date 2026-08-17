
using IDS.Common;
using IDS.Device.Communication;

namespace IDS.Device.Communication
{
    public interface IBootstrap
    {
        //注册需要开启多少的监听端口
        public IBootstrap RegisterService(List<IdsEndPoint> endPoints, Func<IdsEndPoint, byte[],IdsResult<string>> handler);
        public IBootstrap RegisterService(IdsEndPoint endPoints, Func<IdsEndPoint, byte[], IdsResult<string>> handler);
        public IBootstrap RegisterService(IdsEndPoint endPoints);
        public IServerConnection RegisterServiceAndStartup(IdsEndPoint endPoints);
        public IServerConnection GetService(ushort severName);
        public void StartAll();
    }
}
