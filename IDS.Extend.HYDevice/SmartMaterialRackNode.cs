using IDS.Device.Communication;
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
        private readonly static ConcurrentDictionary<string, RackNode> _rackNode = new ConcurrentDictionary<string, RackNode>();

        public static SmartMaterialRackNode Instance => _instance.Value;
        private SmartMaterialRackNode() { }
        public RackNode AddNode(RackNode rackNode) {
            _rackNode.AddOrUpdate(rackNode.IP, rackNode, (k, ov) => rackNode);
            return rackNode;
        }
        public void RemoveNode(RackNode rackNode)
        {
            _rackNode.TryRemove(rackNode.IP,out _);
        }
        public void RemoveNode(string rackNode)
        {
            _rackNode.TryRemove(rackNode, out _);
        }
        public RackNode GetRackNode(string key) {
            RackNode node = null;
            if (_rackNode.TryGetValue(key, out node)) {
                return node;
            }
            return node;
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
        //上下感应货架指令来自前端操作，常驻内存，会同步到Redis。
        public Queue<LocationInfo> WaitDownInductiveQueue {private set; get; } = new();
        public Queue<LocationInfo> WaitUpInductiveQueue { private set; get; } = new ();
        //同步实时的货位状态信息
        public List<LocationStatusInfo> locationStatusInfos { set; get; } = new ();

    }

}
