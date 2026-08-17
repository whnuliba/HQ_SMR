using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.DTO
{
    public class DeviceInitInfoDto
    {
        // ========== 私有字段 ==========
        private byte[] _rawData;
        private string _shelfNo;
        private string _id;
        private string _type;
        private string _msg;
        private List<LEDColor> _colors;
        private List<int> _boards;
        private string _serverIP;
        private int _serverPort;
        private string _shelfIP;
        private int _shelfPort;
        private string _getWay;
        private string _dns;
        private string _mac;
        private string _mode;
        private string _wifiName;
        private string _wifiPwd;
        private bool _isInductiveEnabled;
        private byte _flashTimes;
        private byte _downTime;
        private byte _lightTime;

        // ========== 构造函数 ==========
        private DeviceInitInfoDto() { }

        /// <summary>
        /// 从字节数据解析并创建实例（工厂方法）
        /// </summary>
        /// <param name="data">原始字节数据</param>
        /// <param name="shelfNo">货架编号</param>
        /// <param name="id">命令 GUID</param>
        /// <returns>解析后的 DeviceInitInfo 实例</returns>
        public static DeviceInitInfoDto Parse(byte[] data, string shelfNo, string id)
        {
            var info = new DeviceInitInfoDto();
            info._rawData = data;
            info._shelfNo = shelfNo;
            info._id = id;
            info._type = "0x00";
            info._msg = "OK";

            info.ParseData(data);

            return info;
        }

        // ========== 私有解析方法 ==========
        private void ParseData(byte[] data)
        {
            int offset = 13;

            // 1. 颜色数量
            int colorQty = data[offset];
            offset += 1;

            // 2. 颜色列表
            _colors = new List<LEDColor>();
            for (int i = 0; i < colorQty; i++)
            {
                _colors.Add(new LEDColor
                {
                    R = data[offset + i * 3],
                    G = data[offset + i * 3 + 1],
                    B = data[offset + i * 3 + 2]
                });
            }
            offset += colorQty * 3;

            // 3. 板卡数量
            int boardQty = data[offset];
            offset += 1;

            // 4. 板卡列表
            _boards = new List<int>();
            for (int i = 0; i < boardQty; i++)
            {
                int boardId = data[offset + i * 2] * 256 + data[offset + i * 2 + 1];
                _boards.Add(boardId);
            }
            offset += boardQty * 2;

            // 5. 货架 IP
            _shelfIP = $"{data[offset]}.{data[offset + 1]}.{data[offset + 2]}.{data[offset + 3]}";
            offset += 4;

            // 6. GetWay（网关）
            _getWay = $"{data[offset]}.{data[offset + 1]}.{data[offset + 2]}.{data[offset + 3]}";
            offset += 4;

            // 7. MAC 地址
            _mac = $"{data[offset]:X2}:{data[offset + 1]:X2}:{data[offset + 2]:X2}:{data[offset + 3]:X2}:{data[offset + 4]:X2}:{data[offset + 5]:X2}";
            offset += 6;

            // 8. DNS
            _dns = $"{data[offset]}.{data[offset + 1]}.{data[offset + 2]}.{data[offset + 3]}";
            offset += 4;

            // 9. 货架端口
            _shelfPort = data[offset] * 256 + data[offset + 1];
            offset += 2;

            // 10. 跳过 2 个未知字节
            offset += 2;

            // 11. 网络模式
            _mode = data[offset] == 1 ? "ETHO" : "WIFI";
            offset += 1;

            // 12. 服务器 IP
            _serverIP = $"{data[offset]}.{data[offset + 1]}.{data[offset + 2]}.{data[offset + 3]}";
            offset += 4;

            // 13. 服务器端口
            _serverPort = data[offset] * 256 + data[offset + 1];
            offset += 2;

            // 14. WiFi 名称
            _wifiName = ReadNullTerminatedString(data, offset, 32);
            offset += 32;

            // 15. WiFi 密码
            _wifiPwd = ReadNullTerminatedString(data, offset, 32);
            offset += 32;

            // 16. 感应货架
            int inductiveOffset = 14 + _colors.Count * 3;
            _isInductiveEnabled = data[inductiveOffset] == 1;

            // 17. FlashTimes、DownTime、LightTime
            _flashTimes = 0;
            _downTime = data[14 + colorQty * 3 + 2];
            _lightTime = data[14 + colorQty * 3 + 1];
        }

        private string ReadNullTerminatedString(byte[] data, int startIndex, int maxLength)
        {
            int length = 0;
            for (int i = 0; i < maxLength; i++)
            {
                if (data[startIndex + i] == 0)
                    break;
                length++;
            }
            return length == 0 ? string.Empty : Encoding.ASCII.GetString(data, startIndex, length);
        }

        // ========== 公共属性 ==========

        /// <summary>
        /// 货架编号（如 "A01"）
        /// </summary>
        public string RackNo => _shelfNo;

        /// <summary>
        /// 命令 GUID（用于请求-响应匹配）
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// 返回类型（固定 "0x00" 表示成功）
        /// </summary>
        public string Type => _type;

        /// <summary>
        /// 返回消息（"OK" 表示成功）
        /// </summary>
        public string Msg => _msg;

        /// <summary>
        /// LED 颜色列表（R/G/B 值）
        /// </summary>
        public IReadOnlyList<LEDColor> Colors => _colors.AsReadOnly();

        /// <summary>
        /// 板卡 ID 列表
        /// </summary>
        public IReadOnlyList<int> Boards => _boards.AsReadOnly();

        /// <summary>
        /// 服务器 IP 地址
        /// </summary>
        public string ServerIP => _serverIP;

        /// <summary>
        /// 服务器端口号
        /// </summary>
        public int ServerPort => _serverPort;

        /// <summary>
        /// 货架 IP 地址
        /// </summary>
        public string ShelfIP => _shelfIP;

        /// <summary>
        /// 货架端口号
        /// </summary>
        public int ShelfPort => _shelfPort;

        /// <summary>
        /// 网关地址
        /// </summary>
        public string GetWay => _getWay;

        /// <summary>
        /// DNS 服务器地址
        /// </summary>
        public string DNS => _dns;

        /// <summary>
        /// MAC 地址（格式：AA:BB:CC:DD:EE:FF）
        /// </summary>
        public string MAC => _mac;

        /// <summary>
        /// 网络模式（"ETHO"：有线网络，"WIFI"：无线网络）
        /// </summary>
        public string Mode => _mode;

        /// <summary>
        /// WiFi 名称（SSID）
        /// </summary>
        public string WifiName => _wifiName;

        /// <summary>
        /// WiFi 密码
        /// </summary>
        public string WifiPwd => _wifiPwd;

        /// <summary>
        /// 是否启用感应货架（true：启用，false：未启用）
        /// </summary>
        public bool IsInductiveEnabled => _isInductiveEnabled;

        /// <summary>
        /// 闪烁次数（固定为 0）
        /// </summary>
        public byte FlashTimes => _flashTimes;

        /// <summary>
        /// 下降时间（感应货架按键按下持续时间）
        /// </summary>
        public byte DownTime => _downTime;

        /// <summary>
        /// 点亮时间（LED 点亮持续时间）
        /// </summary>
        public byte LightTime => _lightTime;

        // ========== 行为方法 ==========

        /// <summary>
        /// 生成返回 XML 报文
        /// </summary>
        /// <returns>XML 格式的响应字符串</returns>
        public string ToReturnXml()
        {
            var sb = new StringBuilder();

            sb.Append($"<?xml version='1.0'?><ReturnCMD ShelfNo=\"{_shelfNo}\" ID=\"{_id}\" Type=\"{_type}\" Msg=\"{_msg}\">");

            sb.Append("<Colors>");
            foreach (var color in _colors)
            {
                sb.Append($"<Color R=\"{color.R}\" G=\"{color.G}\" B=\"{color.B}\"/>");
            }
            sb.Append("</Colors>");

            sb.Append("<Boards>");
            foreach (var board in _boards)
            {
                sb.Append($"<Board Qty=\"{board}\"/>");
            }
            sb.Append("</Boards>");

            sb.Append($"<Server IP=\"{_serverIP}\" Port=\"{_serverPort}\"/>");

            sb.Append($"<Shelf IP=\"{_shelfIP}\" Port=\"{_shelfPort}\" ");
            sb.Append($"GetWay=\"{_getWay}\" DNS=\"{_dns}\" MAC=\"{_mac}\" ");
            sb.Append($"Mode=\"{_mode}\" WIFIName=\"{_wifiName}\" WIFIPwd=\"{_wifiPwd}\" ");
            sb.Append($"InductiveShelf=\"{(_isInductiveEnabled ? "Y" : "N")}\" ");
            sb.Append($"FlashTimes=\"{_flashTimes}\" DownTime=\"{_downTime}\" LightTime=\"{_lightTime}\"/>");

            sb.Append("</ReturnCMD>");

            return sb.ToString();
        }

        /// <summary>
        /// 生成返回字节数组（含 CRC 校验）
        /// </summary>
        /// <param name="getCrc">CRC 计算函数（接收字节数组，返回 CRC 字节数组）</param>
        /// <returns>完整的响应报文（XML + CRC）</returns>
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
        /// 获取属性摘要字符串（用于日志输出）
        /// </summary>
        /// <returns>格式化的摘要信息</returns>
        public string GetSummary()
        {
            return $"货架[{_shelfNo}], IP[{_shelfIP}], Port[{_shelfPort}], 感应[{(IsInductiveEnabled ? "Y" : "N")}], 颜色[{_colors?.Count ?? 0}], 板卡[{_boards?.Count ?? 0}]";
        }

        /// <summary>
        /// 判断是否为有效数据
        /// </summary>
        /// <returns>true：数据有效；false：数据无效</returns>
        public bool IsValid()
        {
            return _colors != null && _colors.Count > 0 && _boards != null && _boards.Count > 0;
        }
    }
}
