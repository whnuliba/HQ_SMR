using IDS.Extend.HYDevice.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.DTO
{
    /*
     正常上架流程：
        客户端点“上架” → 服务器记录授权 → 货架灯亮（可能通过 0x06 亮灯指令，通过其他路径发给货架，这是另一条指令流）。
        工人把货物放到储位 → 货架感应到（0x0F） → 服务器匹配授权队列 → 验证通过 → 灯灭，记录库存增加。
        异常拿取流程：
        工人没有经过系统授权，直接拿走货物。
        货架感应到（0x0F） → 服务器查询授权队列 → 找不到匹配项 → 触发报警（AlarmNode），通知管理员。
     */
    /// <summary>
    /// 感应货架储位状态变更信息（充血模型）
    /// 用于处理 modeType == 15 的主动反馈数据
    /// </summary>
    public class InductiveShelfInfoDto
    {
        // ========== 私有字段 ==========
        private string _shelfNo;
        private string _id;
        private string _type;
        private string _msg;
        private List<LocationInfo> _locations;
        private byte[] _pkgId;
        private byte[] _rawData;
        private bool _isValid;

        // ========== 构造函数 ==========
        private InductiveShelfInfoDto() { }

        /// <summary>
        /// 从字节数据解析并创建实例（工厂方法）
        /// </summary>
        /// <param name="data">原始字节数据</param>
        /// <param name="shelfNo">货架编号</param>
        /// <param name="id">命令 GUID</param>
        /// <returns>解析后的 InductiveShelfInfo 实例</returns>
        public static InductiveShelfInfoDto Parse(byte[] data, string shelfNo, string id)
        {
            var info = new InductiveShelfInfoDto();
            info._rawData = data;
            info._shelfNo = shelfNo;
            info._id = id;
            info._type = "0x0F";
            info._msg = "OK";
            info._locations = new List<LocationInfo>();
            info._isValid = false;

            // 解析数据
            info._isValid = ParseInductiveUtils.ParseInductiveCmd(data, out info._locations, out info._pkgId);

            // 如果没有解析到数据，设为空列表
            if (info._locations == null)
            {
                info._locations = new List<LocationInfo>();
            }
            // 同步货位状态信息
            //var rackNode = SmartMaterialRackNode.Instance.GetRackNode(shelfNo);
            //if (rackNode != null) {
            //    rackNode.locationStatusInfos.Clear();
            //    var locs =  info._locations.Select(item =>{
            //        return new LocationStatusInfo {
            //            RackNo = shelfNo,
            //            CellNo = item.Addr.ToString().PadLeft(4, '0'),
            //            CellState = item.Status.ToString(),
            //            Timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            //            UserId = "HQ_SMR",
            //            SessionId = new Guid().ToString("N")
            //        };
            //    }).ToList();
            //    rackNode.locationStatusInfos.AddRange(locs);
            //}

            return info;
        }
        
        /// <summary>
        /// 从已解析的储位列表创建实例
        /// </summary>
        public static InductiveShelfInfoDto FromLocations(string shelfNo, string id, List<LocationInfo> locations)
        {
            return new InductiveShelfInfoDto
            {
                _shelfNo = shelfNo,
                _id = id,
                _type = "0x0F",
                _msg = "OK",
                _locations = locations ?? new List<LocationInfo>(),
                _isValid = true
            };
        }



        // ========== 公共属性 ==========

        /// <summary>
        /// 货架编号
        /// </summary>
        public string ShelfNo => _shelfNo;

        /// <summary>
        /// 命令 GUID
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// 返回类型（固定 "0x0F"）
        /// </summary>
        public string Type => _type;

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg => _msg;

        /// <summary>
        /// 储位状态列表（只读）
        /// </summary>
        public IReadOnlyList<LocationInfo> Locations => _locations.AsReadOnly();

        /// <summary>
        /// 包 ID（10 字节）
        /// </summary>
        public byte[] PkgId => _pkgId;

        /// <summary>
        /// 解析是否成功
        /// </summary>
        public bool IsValid => _isValid;

        /// <summary>
        /// 是否有储位状态变更
        /// </summary>
        public bool HasLocations => _locations != null && _locations.Count > 0;

        /// <summary>
        /// 所有储位序号列表
        /// </summary>
        public List<int> Addresses
        {
            get
            {
                var addrs = new List<int>();
                foreach (var loc in _locations)
                {
                    addrs.Add(loc.Addr);
                }
                return addrs;
            }
        }

        /// <summary>
        /// 按下的储位数量（上架）
        /// </summary>
        public int PressedCount
        {
            get
            {
                int count = 0;
                foreach (var loc in _locations)
                {
                    if (loc.IsPressed) count++;
                }
                return count;
            }
        }

        /// <summary>
        /// 弹起的储位数量（下架）
        /// </summary>
        public int ReleasedCount
        {
            get
            {
                int count = 0;
                foreach (var loc in _locations)
                {
                    if (loc.IsReleased) count++;
                }
                return count;
            }
        }

        // ========== 行为方法 ==========

        /// <summary>
        /// 获取指定状态的储位列表
        /// </summary>
        /// <param name="status">状态值（0：弹起/下架，1：按下/上架）</param>
        /// <returns>匹配的储位列表</returns>
        public List<LocationInfo> GetLocationsByStatus(byte status)
        {
            var result = new List<LocationInfo>();
            foreach (var loc in _locations)
            {
                if (loc.Status == status)
                    result.Add(loc);
            }
            return result;
        }

        /// <summary>
        /// 获取按下的储位列表（上架）
        /// </summary>
        public List<LocationInfo> GetPressedLocations()
        {
            return GetLocationsByStatus(1);
        }

        /// <summary>
        /// 获取弹起的储位列表（下架）
        /// </summary>
        public List<LocationInfo> GetReleasedLocations()
        {
            return GetLocationsByStatus(0);
        }

        /// <summary>
        /// 检查是否包含指定储位
        /// </summary>
        public bool ContainsAddress(int addr)
        {
            foreach (var loc in _locations)
            {
                if (loc.Addr == addr)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取储位状态字符串（用于 XML 生成）
        /// </summary>
        /// <returns>格式："地址1,状态1;地址2,状态2;"</returns>
        public string GetLocationStatusString()
        {
            if (!HasLocations)
                return string.Empty;

            var sb = new StringBuilder();
            foreach (var loc in _locations)
            {
                sb.Append(loc.ToStatusString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成返回 XML 报文
        /// </summary>
        /// <returns>XML 格式的响应字符串</returns>
        public string ToReturnXml()
        {
            if (!HasLocations)
            {
                return $"<?xml version='1.0'?><ReturnCMD ShelfNo=\"{_shelfNo}\" ID=\"{_id}\" Type=\"{_type}\" Msg=\"{_msg}\"/>";
            }

            string statusStr = GetLocationStatusString();
            return $"<?xml version='1.0'?><ReturnCMD ShelfNo=\"{_shelfNo}\" ID=\"{_id}\" Type=\"{_type}\" Msg=\"{_msg}\"><Location Status=\"{statusStr}\"/></ReturnCMD>";
        }

        /// <summary>
        /// 生成返回字节数组（含 CRC）
        /// </summary>
        /// <param name="getCrc">CRC 计算函数</param>
        /// <returns>完整的响应报文</returns>
        public byte[] ToResponseBytes(Func<byte[], byte[]> getCrc)
        {
            string xml = ToReturnXml();
            byte[] data = Encoding.UTF8.GetBytes(xml);
            byte[] crc = getCrc(data);

            byte[] result = new byte[data.Length + 2];
            Array.Copy(data, 0, result, 0, data.Length);
            result[result.Length - 2] = crc[1];
            result[result.Length - 1] = crc[0];

            return result;
        }

        /// <summary>
        /// 生成客户端确认 XML
        /// </summary>
        public string ToClientConfirmXml()
        {
            return $"<?xml version='1.0'?><CMD ShelfNo=\"{_shelfNo}\" ID=\"{_id}\" Type=\"0x00\" Msg=\"OK\"></CMD>";
        }

        /// <summary>
        /// 生成客户端确认字节数组（含 CRC）
        /// </summary>
        public byte[] ToClientConfirmBytes(Func<byte[], byte[]> getCrc)
        {
            string xml = ToClientConfirmXml();
            byte[] data = Encoding.UTF8.GetBytes(xml);
            byte[] crc = getCrc(data);

            byte[] result = new byte[data.Length + 2];
            Array.Copy(data, 0, result, 0, data.Length);
            result[result.Length - 2] = crc[1];
            result[result.Length - 1] = crc[0];

            return result;
        }

        /// <summary>
        /// 获取摘要字符串（用于日志）
        /// </summary>
        public string GetSummary()
        {
            if (!IsValid)
                return $"货架[{_shelfNo}] 解析失败";

            if (!HasLocations)
                return $"货架[{_shelfNo}] 无储位状态变更";

            return $"货架[{_shelfNo}], 按下(上架)[{PressedCount}], 弹起(下架)[{ReleasedCount}], 总计[{_locations.Count}]";
        }

        /// <summary>
        /// 获取详细日志字符串
        /// </summary>
        public string GetDetailLog()
        {
            if (!IsValid)
                return $"货架[{_shelfNo}] 解析失败";

            if (!HasLocations)
                return $"货架[{_shelfNo}] 无储位状态变更";

            var sb = new StringBuilder();
            sb.Append($"货架[{_shelfNo}] 储位状态变更:");
            foreach (var loc in _locations)
            {
                sb.Append($" {loc}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 按面（A/B）分组获取储位
        /// </summary>
        /// <param name="aQty">A 面储位数量</param>
        /// <returns>按面分组的储位字典</returns>
        public Dictionary<string, List<LocationInfo>> GroupBySide(int aQty)
        {
            var result = new Dictionary<string, List<LocationInfo>>
            {
                ["A"] = new List<LocationInfo>(),
                ["B"] = new List<LocationInfo>()
            };

            foreach (var loc in _locations)
            {
                // 储位序号从 1 开始，A 面为 1~aQty，B 面为 aQty+1 ~ 总
                if (loc.Addr <= aQty)
                    result["A"].Add(loc);
                else
                    result["B"].Add(loc);
            }

            return result;
        }

        /// <summary>
        /// 按面分组并过滤指定状态
        /// </summary>
        public Dictionary<string, List<LocationInfo>> GroupBySideAndStatus(int aQty, byte status)
        {
            var grouped = GroupBySide(aQty);
            var result = new Dictionary<string, List<LocationInfo>>
            {
                ["A"] = new List<LocationInfo>(),
                ["B"] = new List<LocationInfo>()
            };

            foreach (var kv in grouped)
            {
                foreach (var loc in kv.Value)
                {
                    if (loc.Status == status)
                        result[kv.Key].Add(loc);
                }
            }

            return result;
        }
    }
}
