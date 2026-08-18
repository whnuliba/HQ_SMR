using IDS.Extend.HYDevice.DTO;
using IDS.HQ.HYDevice.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.Utils
{
        /// <summary>
        /// 感应货架命令解析辅助类
        /// </summary>
        public  class ParseInductiveUtils
        {
            /// <summary>
            /// 解析感应货架命令（modeType == 15）
            /// </summary>
            /// <param name="cmd">原始字节数据</param>
            /// <param name="addrs">解析出的储位列表</param>
            /// <param name="pkgID">包 ID（10 字节）</param>
            /// <returns>解析是否成功</returns>
            public static bool ParseInductiveCmd(byte[] cmd, out List<LocationInfo> addrs, out byte[] pkgID)
            {
                pkgID = new byte[10];
                addrs = new List<LocationInfo>();

                // 1. 检查数据长度是否足够
                if (cmd == null || cmd.Length < 15)
                {
                    return false;
                }

                // 2. 提取包 ID（从索引 1 开始，10 字节）
                Array.Copy(cmd, 1, pkgID, 0, 10);

                // 3. 计算数据长度（索引 13 和 14 是长度字段，大端序）
                int dataLength = cmd[13] * 256 + cmd[14];

                // 4. 检查数据长度是否有效
   
                // 5. 提取 CRC 校验数据（从开头到 dataLength 位置，不包含最后 2 字节 CRC）
                byte[] crcData = new byte[dataLength - 2];
                Array.Copy(cmd, 0, crcData, 0, crcData.Length);

                // 6. 计算 CRC 并验证
                byte[] calculatedCrc = CRCUtils.GetCRC16(crcData);
                if (calculatedCrc[0] != cmd[cmd.Length - 1] || calculatedCrc[1] != cmd[cmd.Length - 2])
                {
                    return false; // CRC 校验失败
                }

                // 7. 解析储位数据
                // 从索引 15 开始，每条记录 3 字节：地址(2字节) + 状态(1字节)
                int recordCount = (dataLength - 15) / 3;
                if (recordCount <= 0)
                {
                    return true; // 没有储位数据，但解析成功
                }

                for (int i = 0; i < recordCount; i++)
                {
                    int offset = 15 + i * 3;
                    var location = new LocationInfo
                    {
                        // 地址：高字节在前（大端序）
                        Addr = cmd[offset] * 256 + cmd[offset + 1],
                        // 状态：0=弹起/下架，1=按下/上架
                        Status = cmd[offset + 2]
                    };
                    addrs.Add(location);
                }

                return true;
            }

            /// <summary>
            /// 解析感应货架命令（简化版，只返回储位列表）
            /// </summary>
            public static List<LocationInfo> ParseLocations(byte[] cmd)
            {
                if (ParseInductiveCmd(cmd, out var addrs, out _))
                {
                    return addrs;
                }
                return new List<LocationInfo>();
            }

            /// <summary>
            /// 检查是否为有效的感应货架命令
            /// </summary>
            public static bool IsValidInductiveCmd(byte[] cmd)
            {
                return ParseInductiveCmd(cmd, out _, out _);
            }
        }
    
}
