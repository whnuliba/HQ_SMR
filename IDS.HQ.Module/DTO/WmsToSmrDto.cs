using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace IDS.HQ.Module.DTO
{
    /// <summary>
    /// 上架请求报文
    /// </summary>
    public class WmsPuywayRequest
    {
        /// <summary>
        /// 操作类型，传什么就回传什么，用于回调处理数据时区分业务类型
        /// </summary>
        [JsonPropertyName("OperateType")]
        public string OperateType { get; set; }

        /// <summary>
        /// "Up"=请求上架命令；"Up_end"=请求上架结束
        /// </summary>
        [JsonPropertyName("Rack_CMD")]
        public string RackCmd { get; set; }

        /// <summary>
        /// 料架ID
        /// </summary>
        [JsonPropertyName("RackId")]
        public string RackId { get; set; }

        /// <summary>
        /// A或者B面
        /// </summary>
        [JsonPropertyName("RackSide")]
        public string RackSide { get; set; }

        /// <summary>
        /// 上架后对应储位的亮灯色：红-1；淡白-2；绿-3；蓝-4；黄绿-5；紫-6；黄-7；浅蓝-8；浅黄-9
        /// </summary>
        [JsonPropertyName("LightColor")]
        public int LightColor { get; set; }

        /// <summary>
        /// 上架超时时间（秒）
        /// </summary>
        [JsonPropertyName("OutTime")]
        public int OutTime { get; set; }

        /// <summary>
        /// 产品PPID
        /// </summary>
        [JsonPropertyName("PPID")]
        public string PPID { get; set; }

        /// <summary>
        /// 产品料号
        /// </summary>
        [JsonPropertyName("CompPN")]
        public string CompPn { get; set; }

        /// <summary>
        /// 产品物料描述
        /// </summary>
        [JsonPropertyName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        [JsonPropertyName("Qty")]
        public string Qty { get; set; }

        /// <summary>
        /// 客户料号
        /// </summary>
        [JsonPropertyName("CustomerPN")]
        public string CustomerPn { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [JsonPropertyName("VendorCode")]
        public string VendorCode { get; set; }

        /// <summary>
        /// DC（Date Code）
        /// </summary>
        [JsonPropertyName("DateCode")]
        public string DateCode { get; set; }

        /// <summary>
        /// LC（Lot Code）
        /// </summary>
        [JsonPropertyName("LotCode")]
        public string LotCode { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        [JsonPropertyName("OrgCode")]
        public string OrgCode { get; set; }

        /// <summary>
        /// 09码
        /// </summary>
        [JsonPropertyName("NO_09")]
        public string No09 { get; set; }

        /// <summary>
        /// GUPN
        /// </summary>
        [JsonPropertyName("GUPN")]
        public string Gupn { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("SessionId")]
        public string SessionId { get; set; }
    }

    /// <summary>
    /// 上架返回报文
    /// </summary>
    public class WmsResponse
    {
        /// <summary>
        /// 状态码：0-上架正常，1-取消上架，其他-异常
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// 料架ID
        /// </summary>
        [JsonPropertyName("rackId")]
        public string RackId { get; set; }

        /// <summary>
        /// 储位ID（字符串空）
        /// </summary>
        [JsonPropertyName("cellId")]
        public string CellId { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
    }

    public class WmsOutBoundRequest {
        public string RackCmd { set; get; }// ": "Down",     --"Down"=请求下架命令  --"Down_end"=请求下架结束  
        public List<RackLocationDto> RackClass { set; get; }//
         public int  LightColor { set; get; }//: 2,         --下架后对应储位的亮灯色
         public int OutTime { set; get; }//: 100,          --下架超时时间
        public string UserId { set; get; }//": "testUser",
        public DateTime Timestamp { set; get; }//": "2020-06-04T11:37:13.3709747+08:00",
        public string SessionId { set; get; }//": "3821663e-4736-43c3-a1ab-b4788df13441"
    }
    public class RackLocationDto { 
       public string RackId { set; get; }
       public List<string> CellList { set; get; }

    }
}
