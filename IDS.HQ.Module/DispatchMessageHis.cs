using IDS.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IDS.HQ.Module
{
    public class DispatchMessageHis : IdsBaseEntity
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
        /// 任务ID（19位字符）
        /// </summary>
        public string? TaskId { get; set; }

        /// <summary>
        /// 产品PPID
        /// </summary>
        public string? PpId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// 消息（文本）
        /// </summary>

        public string? Message { get; set; }

        /// <summary>
        /// 请求ID/会话ID
        /// </summary>
        public string? RequestId { get; set; }
        public string? RespMessage { get; set; }
        public string? IOStatus { get; set; }
        public int? PostCount { set; get; }
        public string? ExtId { set; get; }
        public string? BizStatus { set; get; }
        [NotMapped]
        public DateTime? BeginTime { set; get; }
        [NotMapped]
        public DateTime? EndTime { set; get; }
    }
}
