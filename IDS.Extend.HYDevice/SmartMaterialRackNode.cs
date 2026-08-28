using HPSocket.Sdk;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Device.Communication;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace IDS.Extend.HYDevice
{
    public class SmartMaterialRackNode
    {
        public ILog Logger = LogManager.GetLogger(typeof(SmartMaterialRackNode));
        private static readonly Lazy<SmartMaterialRackNode> _instance = new Lazy<SmartMaterialRackNode>(() => new SmartMaterialRackNode());
        private readonly static ConcurrentDictionary<string, RackNode> _rackNodeWithIp = new ConcurrentDictionary<string, RackNode>();
        private readonly static ConcurrentDictionary<string, RackNode> _rackNodeWithNo = new ConcurrentDictionary<string, RackNode>();
        //允许正在上架的任务
        private readonly static List<int> _allowPutwayList = new();
        public static SmartMaterialRackNode Instance => _instance.Value;
        private SmartMaterialRackNode() { }
        public bool AddAllowPutwayAddr(string taskId, List<int?> addrs)
        {
            IdsRedis redisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            //保存任务
            return redisClient.GetDatabase().StringSet(HYConstant.AllowPutwayaDDRKey + taskId, JsonConvert.SerializeObject(addrs), TimeSpan.FromMinutes(60));
        }
        public bool RemoveAllowPutwayAddr(string taskId)
        {
            IdsRedis redisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            return redisClient.GetDatabase().KeyDelete(HYConstant.AllowPutwayaDDRKey + taskId);
        }
        public bool IsAllowPutwayAddr(string taskId, int addr)
        {
            IdsRedis redisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            RedisValue addrValue = redisClient.GetDatabase().StringGet(taskId);
            if (!addrValue.HasValue)
            {
                return false;
            }
            List<int> addrs = JsonConvert.DeserializeObject<List<int>>(addrValue);
            if (addrs.Contains(addr))
            {
                return true;
            }
            return false;
        }
        public RackNode AddNode(RackNode rackNode)
        {
            _rackNodeWithIp.AddOrUpdate(rackNode.IP, rackNode, (k, ov) => rackNode);
            _rackNodeWithNo.AddOrUpdate(rackNode.No, rackNode, (k, ov) => rackNode);
            return rackNode;
        }
        public void RemoveNode(RackNode rackNode)
        {
            _rackNodeWithIp.TryRemove(rackNode.IP, out _);
            _rackNodeWithNo.TryRemove(rackNode.No, out _);
        }
        public void RemoveNode(string rackNode)
        {
            RemoveNode(GetRackNode(rackNode));
        }
        public RackNode GetRackNode(string key)
        {
            RackNode node = null;
            if (_rackNodeWithIp.TryGetValue(key, out node))
            {
                return node;
            }
            if (node == null && _rackNodeWithNo.TryGetValue(key, out node))
                return node;
            return node;
        }
        /// <summary>
        /// 报警与接触报警的公共方法
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="data"></param>
        /// <param name="session"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool IsExistsAlarm(string rack, string side, int? addr)
        {
            IdsRedis redisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            var rackNode = GetRackNode(rack);
            bool exist = redisClient.GetDatabase().HashExists($"{HYConstant.RackAlarmRecordKey}:{rackNode.No}_{side}", addr + "");
            if (!exist)
            {
                //查看是否有面在报警
                exist = redisClient.GetDatabase().HashExists($"{HYConstant.RackAlarmRecordKey}:{rackNode.No}_{side}", side);
            }
            return exist;
        }
        public virtual IdsResult<object> SendAlarmNotice<E>(E data, IdsSession session, Action<IdsSession>? action = null)
        {
            var alarm = data as RackAlarmInfo;
            //计算是报警还是解除报警
            int alarmMode = alarm.AlarmMode;
            IdsRedis redisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();

            var message = DeviceMessage.GetAlarm(alarm.locations, alarm.AlarmMode, alarm.LocationMode, alarm.Side);
            RackNode rack;
            if (!string.IsNullOrEmpty(alarm.RackNo))
            {
                rack = GetRackNode(alarm.RackNo);
            }
            else
            {
                rack = GetRackNode(session?.ResponseEndPoint.Address);
            }
            if (rack != null)
            {
                session?.ServerConnection.Send(message, new IdsEndPoint(rack.IP, rack.Port), (session) =>
                {
                    //按储位维度存储报警到redis，redis中每个储位维护一个报警信息。
                    //发送设备报警完成后，记录到缓存,不管是否发送成功都需要记录
                    action?.Invoke(session);
                    string _side = alarm.Side == 0 ? "A" : "B";
                    string fieldKey = $"{rack.No}_{_side}";
                    string hexNoDash = BitConverter.ToString(message).Replace("-", " ");
                    string msg = $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}|报文[{hexNoDash}]|{alarm.ErrorInfo}";
                    //计算报警地址, 单储位报警
                    HashEntry[] hashFields = new HashEntry[0];
                    RedisValue[] cancelAlarm = new RedisValue[0];
                    //增加写入数据库操作
                    List<RackAlarm> rackAlarms = new();
                    if (alarm.AlarmMode == 0 && alarm.LocationMode == 0)
                    {
                        hashFields = new HashEntry[1];
                        hashFields[0] = new HashEntry(alarm.locations[0] + "", $"{msg}");
                        var rackalarm = new RackAlarm
                        {
                            Id = IdUtils.Id + "",
                            Message = $"{msg}",
                            AlarmType = alarm.location.Status,
                            LocationType = alarm.LocationMode,
                            RackNo = rack.No,
                            RackSide = _side,
                            Location = alarm?.location?.Addr.ToString(),
                            Status = 0,
                            HandleState = 0
                        };
                        rackalarm.saveInit();
                        rackAlarms.Add(rackalarm);
                    }
                    //计算报警地址, 多储位报警
                    if (alarm.AlarmMode == 0 && alarm.LocationMode == 1)
                    {
                        hashFields = new HashEntry[alarm.locations.Count];
                        for (int i = 0; i < alarm.locations.Count; i++)
                        {
                            hashFields[i] = new HashEntry(alarm.locations[i], $"{msg}");
                            var rackalarm = new RackAlarm
                            {
                                Id = IdUtils.Id + "",
                                Message = $"{msg}",
                                AlarmType = alarm.location.Status,
                                LocationType = alarm.LocationMode,
                                RackNo = rack.No,
                                RackSide = _side,
                                Location = alarm.locations[i] + "",
                                Status = 0,
                                HandleState = 0
                            };
                            rackalarm.saveInit();
                            rackAlarms.Add(rackalarm);
                        }
                    }
                    //计算报警地址, 单面
                    if (alarm.AlarmMode == 0 && alarm.LocationMode == 2)
                    {
                        hashFields = new HashEntry[1];
                        hashFields[0] = new HashEntry(_side, $"{msg}");

                        var rackalarm = new RackAlarm
                        {
                            Id = IdUtils.Id + "",
                            Message = $"{msg}",
                            AlarmType = alarm.location.Status,
                            LocationType = alarm.LocationMode,
                            RackNo = rack.No,
                            RackSide = _side,
                            Status = 0,
                            HandleState = 0
                        };
                        rackalarm.saveInit();
                        rackAlarms.Add(rackalarm);
                    }

                    if (alarm.AlarmMode == 1 && alarm.LocationMode == 0)
                    {
                        cancelAlarm = new RedisValue[1];
                        cancelAlarm[0] = new RedisValue(alarm.locations[0] + "");
                        var rackalarm = new RackAlarm
                        {
                            Id = IdUtils.Id + "",
                            Message = $"{msg}",
                            AlarmType = alarm.location.Status,
                            LocationType = alarm.LocationMode,
                            RackNo = rack.No,
                            RackSide = _side,
                            Location = alarm.locations[0] + "",
                            Status = 0,
                            HandleState = 0
                        };
                        rackalarm.saveInit();
                        rackAlarms.Add(rackalarm);
                    }
                    //计算报警地址, 多储位报警
                    if (alarm.AlarmMode == 1 && alarm.LocationMode == 1)
                    {
                        cancelAlarm = new RedisValue[alarm.locations.Count];
                        for (int i = 0; i < alarm.locations.Count; i++)
                        {
                            cancelAlarm[i] = new RedisValue(alarm.locations[i] + "");
                            var rackalarm = new RackAlarm
                            {
                                Id = IdUtils.Id + "",
                                Message = $"{msg}",
                                AlarmType = alarm.location.Status,
                                LocationType = alarm.LocationMode,
                                RackNo = rack.No,
                                RackSide = _side,
                                Location = alarm.locations[i] + "",
                                Status = 0,
                                HandleState = 0
                            };
                            rackalarm.saveInit();
                            rackAlarms.Add(rackalarm);
                        }
                    }
                    //计算报警地址, 单面
                    if (alarm.AlarmMode == 1 && alarm.LocationMode == 2)
                    {
                        cancelAlarm = new RedisValue[1];
                        cancelAlarm[0] = new RedisValue(_side);
                        var rackalarm = new RackAlarm
                        {
                            Id = IdUtils.Id + "",
                            Message = $"{msg}",
                            AlarmType = alarm.location.Status,
                            LocationType = alarm.LocationMode,
                            RackNo = rack.No,
                            RackSide = _side,
                            Status = 0,
                            HandleState = 0
                        };
                        rackalarm.saveInit();
                        rackAlarms.Add(rackalarm);
                    }

                    using (var ctx = dbContext.CreateDbContext())
                    {
                        using (var ts = new TransactionScope())
                        {
                            if (alarm.AlarmMode == 0)
                            {
                                //节点写入到缓存
                                redisClient.GetDatabase().HashSet($"{HYConstant.RackAlarmRecordKey}:{fieldKey}", hashFields);
                                var options = new BulkCopyOptions
                                {
                                    BulkCopyType = BulkCopyType.ProviderSpecific,
                                    KeepIdentity = true,
         
                                };
                                ctx.BulkCopy(options, rackAlarms);
                             }
                            if (alarm.AlarmMode == 1)
                            {

                                //判断若是按面接触，则整个面的所有储位全部解除报警
                                if (alarm.LocationMode == 2)
                                {
                                    ctx.RackAlarm
                                        .Where(a => a.RackNo == rack.No && a.RackSide == _side && a.HandleState == 0)
                                        .ExecuteUpdateAsync(setters => setters
                                            .SetProperty(a => a.HandleState, 1)
                                            .SetProperty(a => a.LastModifyTime, DateTime.Now)
                                     );
                                    redisClient.GetDatabase().KeyDelete($"{HYConstant.RackAlarmRecordKey}:{fieldKey}");
                                }
                                else
                                {

                                    ctx.RackAlarm
                                         .Where(a => a.RackNo == rack.No && a.RackSide == _side && cancelAlarm.Contains(a.Location) && a.HandleState == 0)
                                         .ExecuteUpdateAsync(setters => setters
                                             .SetProperty(a => a.HandleState, 1)
                                             .SetProperty(a => a.LastModifyTime, DateTime.Now)
                                      );
                                    redisClient.GetDatabase().HashDelete($"{HYConstant.RackAlarmRecordKey}:{fieldKey}", cancelAlarm);
                                    //这里还要反向操作下，若所有储位报警都接触，则删除整个Key
                                    var keys = redisClient.GetDatabase().HashKeys($"{HYConstant.RackAlarmRecordKey}:{fieldKey}");
                                    if (keys != null && keys.Length == 1 && keys[0].ToString() == _side)
                                    {
                                        ctx.RackAlarm
                                        .Where(a => a.RackNo == rack.No && a.RackSide == _side && a.HandleState == 0)
                                        .ExecuteUpdateAsync(setters => setters
                                            .SetProperty(a => a.HandleState, 1)
                                            .SetProperty(a => a.LastModifyTime, DateTime.Now));
                                        redisClient.GetDatabase().KeyDelete($"{HYConstant.RackAlarmRecordKey}:{fieldKey}");
                                    }
                                }
                            }
                            ts.Complete();
                        }
                    }
                });
                return IdsResult<object>.ok();
            }
            return IdsResult<object>.failure();
        }

        /// <summary>
        /// 发送蜂鸣灯报警
        /// </summary>
        /// <param name="rackNo"></param>
        /// <param name="side"></param>
        /// <param name="message"></param>
        /// <param name="action"></param>
        public void SendRackAlarmLight(string rackNo, byte side, string message, Action<IdsSession>? action = null)
        {
            RackNode node = null;
            if (!_rackNodeWithNo.TryGetValue(rackNo, out node))
            {
                return;
            }
            string _side = side == 0 ? "A" : "B";
            Logger.Warn($"货架{rackNo},面{_side}触发报警{message}");
            byte[] alarm = DeviceMessage.GetAlarmLight((byte)side, (byte)ALARM.BUZZER, true);
            IdsEndPoint idsEnd = new IdsEndPoint(node.IP, node.Port);
            var conn = ServerConnectionHolder.GetDefaultConnection();
            conn?.Send(alarm, idsEnd, (session) =>
            {
                action?.Invoke(session);
            });
        }
        /// <summary>
        ///  解除蜂鸣灯报警
        /// </summary>
        /// <param name="rackNo"></param>
        /// <param name="side"></param>
        /// <param name="action"></param>

        public void SendCancelRackAlarmLight(string rackNo, byte side, Action<IdsSession>? action = null)
        {
            RackNode node = null;
            if (!_rackNodeWithNo.TryGetValue(rackNo, out node))
            {
                return;
            }
            IdsRedis redisClient = ContainerUtils.GetRequiredService<IdsRedis>();
            byte[] message = DeviceMessage.GetBigLightOnBuzzerMessage(side, (int)LightColor.Green, false); IdsEndPoint idsEnd = new IdsEndPoint(node.IP, node.Port);
            var conn = ServerConnectionHolder.GetDefaultConnection();
            conn?.Send(message, idsEnd, (session) =>
            {
                string _side = side == 0 ? "A" : "B";
                string fieldKey = $"{node.No}_{_side}";
                redisClient.RemoveHashFieldCache(HYConstant.RackAlarmRecordKey, fieldKey);
                action?.Invoke(session);
            });
        }

        public void NoticeRackMultiLightOn(string rackNo, Dictionary<int, byte> OnLight, Action<IdsSession>? action = null)
        {

            if (OnLight != null && OnLight.Count > 0)
            {
                var rack = GetRackNode(rackNo);
                var result = OnLight.GroupBy(kvp => kvp.Value)
                    .ToDictionary(g => g.Key, g => g.Where(f => f.Key != null).Select(kvp => kvp.Key).ToList());
                //这个地方取决于要发多少总颜色的灯信息
                foreach (var kvp in result)
                {
                    var conn = ServerConnectionHolder.GetDefaultConnection();
                    var idsEndpoint = new IdsEndPoint(rack.IP, rack.Port);
                    //获取报文
                    var message = DeviceMessage.GetMultiLightOnMessage(kvp.Value, kvp.Key);
                    conn.Send(message, idsEndpoint, action);
                }
            }
        }
        public void NoticeRack(string rackNo, byte[] data, Action<IdsSession>? action = null)
        {

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

        public void Initialize()
        {
            //用于同步数据库
            IDbContextFactory<RackDbContext> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<RackDbContext>>();
            using (var ctx = dbContext.CreateDbContext())
            {
                ctx.Set<Rack>().ToList().ForEach(item =>
                {
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
    public class RackNode
    {
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
