using IDS.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IDS.HQ.Module
{
    public class MaterialInfo : IdsBaseEntity
    {

        /// <summary>
        /// 操作类型，用于回调处理数据时区分业务类型
        /// </summary>
        public string? OperateType { get; set; }

        /// <summary>
        /// 料架ID
        /// </summary>
        public string? RackId { get; set; }

        /// <summary>
        /// A或者B面
        /// </summary>
        public string? RackSide { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        //[Required]
        public string? TaskId { get; set; }

        /// <summary>
        /// 亮灯色：1-红，2-淡白，3-绿，4-蓝，5-黄绿，6-紫，7-黄，8-浅蓝，9-浅黄
        /// </summary>
        public int? LightColor { get; set; }

        /// <summary>
        /// 上架超时时间（秒）
        /// </summary>
        public int? OutTime { get; set; }

        /// <summary>
        /// 产品PPID
        /// </summary>
        //[Column("PPID")]
        //[StringLength(50)]
        public string? PPID { get; set; }

        /// <summary>
        /// 产品料号
        /// </summary>
        //[StringLength(100)]
        public string? CompPn { get; set; }

        /// <summary>
        /// 产品物料描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public decimal? Qty { get; set; }

        /// <summary>
        /// 客户料号
        /// </summary>
        //[StringLength(100)]
        public string? CustomerPn { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        //[StringLength(200)]
        public string? VendorCode { get; set; }

        /// <summary>
        /// DC（Date Code）
        /// </summary>
        //[StringLength(50)]
        public string? DateCode { get; set; }

        /// <summary>
        /// LC（Lot Code）
        /// </summary>
        //[StringLength(50)]
        public string? LotCode { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public string? OrgCode { get; set; }

        /// <summary>
        /// 09码
        /// </summary>
        public string? No09 { get; set; }

        /// <summary>
        /// GUPN
        /// </summary>
       // [StringLength(100)]
        public string? Gupn { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        public string? SessionId { get; set; }
    }
}
