using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice
{
    /// <summary>
    /// 储位地址信息
    /// </summary>
    public class LocationInfo
    {
        /// <summary>
        /// 储位序号（从 1 开始）
        /// </summary>
        public int Addr { get; set; }

        /// <summary>
        /// 储位状态（0：弹起/下架，1：按下/上架）
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 是否按下（上架）
        /// </summary>
        public bool IsPressed => Status == 1;

        /// <summary>
        /// 是否弹起（下架）
        /// </summary>
        public bool IsReleased => Status == 0;

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusDescription => Status == 1 ? "按下(上架)" : "弹起(下架)";

        /// <summary>
        /// 状态显示字符（Y：按下，N：弹起）
        /// </summary>
        public string StatusChar => Status == 1 ? "Y" : "N";

        public override string ToString()
        {
            return $"储位[{Addr}] {StatusDescription}";
        }

        /// <summary>
        /// 生成状态字符串（用于 XML）
        /// </summary>
        public string ToStatusString()
        {
            return $"{Addr},{Status};";
        }
    }

    public class LocationStatusInfo
    {
        /// <summary>
        /// 料架编码
        /// </summary>
        public string RackNo { get; set; }
        /// <summary>
        /// 储位
        /// </summary>
        public string CellNo { get; set; }
        /// <summary>
        /// 储位状态
        /// </summary>

        public string CellState { get; set; }

        public string UserId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        public string SessionId { get; set; }
    }
}
