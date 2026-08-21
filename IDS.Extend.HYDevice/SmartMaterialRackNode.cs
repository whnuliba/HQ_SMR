using IDS.Device.Communication;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.Ioc;
using LinqToDB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice
{
    public class SmartMaterialRackNode
    {
        private static readonly Lazy<SmartMaterialRackNode> _instance = new Lazy<SmartMaterialRackNode>(() => new SmartMaterialRackNode());
        private readonly static ConcurrentDictionary<string, RackNode> _rackNodeWithIp = new ConcurrentDictionary<string, RackNode>();
        private readonly static ConcurrentDictionary<string, RackNode> _rackNodeWithNo = new ConcurrentDictionary<string, RackNode>();

        public static SmartMaterialRackNode Instance => _instance.Value;
        private SmartMaterialRackNode() { }
        public RackNode AddNode(RackNode rackNode) {
            _rackNodeWithIp.AddOrUpdate(rackNode.IP, rackNode, (k, ov) => rackNode);
            _rackNodeWithNo.AddOrUpdate(rackNode.No, rackNode, (k, ov) => rackNode);
            return rackNode;
        }
        public void RemoveNode(RackNode rackNode)
        {
            _rackNodeWithIp.TryRemove(rackNode.IP,out _);
            _rackNodeWithNo.TryRemove(rackNode.No, out _);
        }
        public void RemoveNode(string rackNode)
        {
            RemoveNode(GetRackNode(rackNode));
        }
        public RackNode GetRackNode(string key) {
            RackNode node = null;
            if (_rackNodeWithIp.TryGetValue(key, out node)) {
                return node;
            }
            if(node==null && _rackNodeWithNo.TryGetValue(key, out node))
                return node;
            return node;
        }
        public void NoticeRackMultiLightOn(string rackNo, Dictionary<int, byte> OnLight, Action<IdsSession>? action = null) {

            if (OnLight != null && OnLight.Count > 0)
            {
                var rack = GetRackNode(rackNo);
                var result = OnLight.GroupBy(kvp => kvp.Value)
                    .ToDictionary(g => g.Key, g => g.Where(f=>f.Key!=null).Select(kvp => kvp.Key).ToList());
                //这个地方取决于要发多少总颜色的灯信息
                foreach (var kvp in result) {
                    var conn = ServerConnectionHolder.GetDefaultConnection();
                    var idsEndpoint = new IdsEndPoint(rack.IP, rack.Port);
                    //获取报文
                    var message = DeviceMessage.GetMultiLightOnMessage(kvp.Value, kvp.Key);
                    conn.Send(message, idsEndpoint, action);
                }
            }
        }
        public void NoticeRack(string rackNo, byte[] data,Action<IdsSession>? action=null) {

            var rack = GetRackNode(rackNo);
            var conn = ServerConnectionHolder.GetDefaultConnection();
            var idsEndpoint = new IdsEndPoint(rack.IP, rack.Port);
            conn.Send(data, idsEndpoint, action);
        }
        public void NoticeRackMultiLightOff(string rackNo, List<int> addr, Action<IdsSession>? action = null)
        {

            if (addr != null && addr.Count > 0)
            {
                var rack = GetRackNode(rackNo);
                var conn = ServerConnectionHolder.GetDefaultConnection();
                var message = DeviceMessage.GetMultiLightOffMessage(addr);
                var idsEndpoint = new IdsEndPoint(rack.IP, rack.Port);
                conn.Send(message, idsEndpoint, action);
            }
        }

        public void Initialize() {
            //用于同步数据库
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.AutofacServiceProvider.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using (var ctx = dbContext.CreateDbContext()) {
              ctx.Set<Rack>().ToList().ForEach(item =>{
                  var node = new RackNode
                  {
                      No = item.RackNo,
                      IP = item.IP,
                      Port = (ushort)item.Port,
                      Enabled = "Y",
                  };
                  AddNode(node);
              });


            }
        }
    }
    public class RackNode {
        // <Shelf No="B001" IP="10.40.135.10" Port="5000" LocalIP="localhost" LocalPort="8902" Enabled="Y" Alarm="Y" InductiveShelf="Y" AQty="656" BQty="656" />
        public string No { set; get; }
        public string IP { set; get; }
        public ushort Port { set; get; }
        public string LocalIP { set; get; }
        public ushort LocalPort { set; get; }
        public string Enabled { set; get; } = "Y";
        public string Alarm { set; get; } = "Y";
        public string InductiveShelf { set; get; } = "Y";
        public int AQty { set; get; } = 656;
        public int BQty { set; get; } = 656;
        public string RackSide { set; get; }

    }

}
