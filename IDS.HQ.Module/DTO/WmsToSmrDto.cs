using Newtonsoft.Json;
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
        /// 
        [JsonPropertyName("OperateType")]
        [JsonProperty("OperateType")]
        public string OperateType { get; set; }

        /// <summary>
        /// "Up"=请求上架命令；"Up_end"=请求上架结束
        /// </summary>
        [JsonPropertyName("Rack_CMD")]
        [JsonProperty("Rack_CMD")]
        public string? RackCmd { get; set; }

        /// <summary>
        /// 料架ID
        /// </summary>
        [JsonPropertyName("RackId")]
        [JsonProperty("RackId")]
        public string? RackId { get; set; }

        /// <summary>
        /// A或者B面
        /// </summary>
        [JsonPropertyName("RackSide")]
        [JsonProperty("RackSide")]
        public string? RackSide { get; set; }

        /// <summary>
        /// 上架后对应储位的亮灯色：红-1；淡白-2；绿-3；蓝-4；黄绿-5；紫-6；黄-7；浅蓝-8；浅黄-9
        /// </summary>
        [JsonPropertyName("LightColor")]
        [JsonProperty("LightColor")]
        public int? LightColor { get; set; }

        /// <summary>
        /// 上架超时时间（秒）
        /// </summary>
        [JsonPropertyName("OutTime")]
        [JsonProperty("OutTime")]
        public int? OutTime { get; set; }

        /// <summary>
        /// 产品PPID
        /// </summary>
        [JsonPropertyName("PPID")]
        [JsonProperty("PPID")]
        public string? PPID { get; set; }

        /// <summary>
        /// 产品料号
        /// </summary>
        [JsonPropertyName("CompPN")]
        [JsonProperty("CompPN")]
        public string? CompPn { get; set; }

        /// <summary>
        /// 产品物料描述
        /// </summary>
        [JsonPropertyName("Description")]
        [JsonProperty("Description")]
        public string? Description { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        [JsonPropertyName("Qty")]
        [JsonProperty("Qty")]
        public decimal? Qty { get; set; }

        /// <summary>
        /// 客户料号
        /// </summary>
        [JsonPropertyName("CustomerPN")]
        [JsonProperty("CustomerPN")]
        public string? CustomerPn { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [JsonPropertyName("VendorCode")]
        [JsonProperty("VendorCode")]

        public string? VendorCode { get; set; }

        /// <summary>
        /// DC（Date Code）
        /// </summary>
        [JsonPropertyName("DateCode")]
        [JsonProperty("DateCode")]
        public string? DateCode { get; set; }

        /// <summary>
        /// LC（Lot Code）
        /// </summary>
        [JsonPropertyName("LotCode")]
        [JsonProperty("LotCode")]
        public string? LotCode { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        [JsonPropertyName("OrgCode")]
        [JsonProperty("OrgCode")]
        public string? OrgCode { get; set; }

        /// <summary>
        /// 09码
        /// </summary>
        [JsonPropertyName("NO_09")]
        [JsonProperty("NO_09")]
        public string? No09 { get; set; }

        /// <summary>
        /// GUPN
        /// </summary>
        [JsonPropertyName("GUPN")]
        [JsonProperty("GUPN")]
        public string? Gupn { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonPropertyName("UserId")]
        [JsonProperty("UserId")]
        public string? UserId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("Timestamp")]
        [JsonProperty("Timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("SessionId")]
        [JsonProperty("SessionId")]
        public string? SessionId { get; set; }
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
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// 料架ID
        /// </summary>
        [JsonPropertyName("rackId")]
        [JsonProperty("rackId")]
        public string RackId { get; set; }

        /// <summary>
        /// 储位ID（字符串空）
        /// </summary>
        [JsonPropertyName("cellId")]
        [JsonProperty("cellId")]
        public string CellId { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        [JsonPropertyName("message")]
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("timestamp")]
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("sessionId")]
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

    public class WmsOutBoundRequest
    {
        [JsonPropertyName("RackCmd")]
        [JsonProperty("RackCmd")]
        public string RackCmd { set; get; }// ": "Down",     --"Down"=请求下架命令  --"Down_end"=请求下架结束  

        [JsonPropertyName("RackClass")]
        [JsonProperty("RackClass")]
        public List<RackLocationDto> RackClass { set; get; }//

        [JsonPropertyName("LightColor")]
        [JsonProperty("LightColor")]
        public int LightColor { set; get; }//: 2,         --下架后对应储位的亮灯色

        [JsonPropertyName("OutTime")]
        [JsonProperty("OutTime")]
        public int OutTime { set; get; }//: 100,          --下架超时时间

        [JsonPropertyName("UserId")]
        [JsonProperty("UserId")]
        public string UserId { set; get; }//": "testUser",

        [JsonPropertyName("Timestamp")]
        [JsonProperty("Timestamp")]
        public DateTime Timestamp { set; get; }//": "2020-06-04T11:37:13.3709747+08:00",

        [JsonPropertyName("SessionId")]
        [JsonProperty("SessionId")]
        public string SessionId { set; get; }//": "3821663e-4736-43c3-a1ab-b4788df13441"
    }

    public class RackLocationDto
    {
        [JsonPropertyName("RackId")]
        [JsonProperty("RackId")]
        public string RackId { set; get; }

        [JsonPropertyName("CellList")]
        [JsonProperty("CellList")]
        public List<string> CellList { set; get; }
    }

    public class WmsCancelAlarm
    {
        [JsonPropertyName("RackId")]
        [JsonProperty("RackId")]
        public string RackId { set; get; }

        [JsonPropertyName("cellId")]
        [JsonProperty("cellId")]
        public string CellId { get; set; }

        [JsonPropertyName("RackSide")]
        [JsonProperty("RackSide")]
        public string RackSide { get; set; }

        [JsonPropertyName("UserId")]
        [JsonProperty("UserId")]
        public string UserId { set; get; }

        [JsonPropertyName("Timestamp")]
        [JsonProperty("Timestamp")]
        public DateTime Timestamp { set; get; }//": "2020-06-04T11:37:13.3709747+08:00",

        [JsonPropertyName("SessionId")]
        [JsonProperty("SessionId")]
        public string SessionId { set; get; }//": "3821663e-4736-43c3-a1ab-b4788df13441"
    }

    public class WmsCancelAlarmResponse
    {
        /// <summary>
        /// 状态码：0-上架正常，1-取消上架，其他-异常
        /// </summary>
        [JsonPropertyName("code")]
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonPropertyName("Message")]
        [JsonProperty("Message")]
        public List<string> Message { set; get; }

        [JsonPropertyName("Timestamp")]
        [JsonProperty("Timestamp")]
        public DateTime Timestamp { set; get; }

        [JsonPropertyName("SessionId")]
        [JsonProperty("SessionId")]
        public string SessionId { set; get; }
    }


    /// <summary>
    /// API请求头
    /// </summary>
    public class ApiRequestHeader
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        [JsonPropertyName("RequestID")]
        [JsonProperty("RequestID")]
        public string RequestId { get; set; }

        /// <summary>
        /// 来源系统
        /// </summary>
        [JsonPropertyName("SourceSystem")]
        [JsonProperty("SourceSystem")]
        public string SourceSystem { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [JsonPropertyName("ServiceName")]
        [JsonProperty("ServiceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        [JsonPropertyName("AppVersion")]
        [JsonProperty("AppVersion")]
        public string AppVersion { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [JsonPropertyName("Token")]
        [JsonProperty("Token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// 通用API请求（泛型）
    /// </summary>
    /// <typeparam name="T">请求数据类型</typeparam>
    public class ApiRequest<T>
    {
        /// <summary>
        /// 请求头
        /// </summary>
        [JsonPropertyName("APIReqHeader")]
        [JsonProperty("APIReqHeader")]
        public ApiRequestHeader ApiReqHeader { get; set; }

        /// <summary>
        /// 请求数据（泛型）
        /// </summary>
        [JsonPropertyName("APIReqData")]
        [JsonProperty("APIReqData")]
        public List<T> ApiReqData { get; set; }
    }


    /// <summary>
    /// 料架位置变更数据
    /// </summary>
    public class LocationInfoChangeData
    {
        /// <summary>
        /// 操作类型，传什么回传什么，回调处理数据时区分业务类型
        /// </summary>
        [JsonPropertyName("OperateType")]
        [JsonProperty("OperateType")]
        public string? OperateType { get; set; }

        /// <summary>
        /// 料盘ID/PPID
        /// </summary>
        [JsonPropertyName("PPID")]
        [JsonProperty("PPID")]
        public string? PpId { get; set; }

        /// <summary>
        /// 操作是否合法：1-合法，0-非法
        /// </summary>
        [JsonPropertyName("IsLegal")]
        [JsonProperty("IsLegal")]
        public string? IsLegal { get; set; }

        /// <summary>
        /// 料架ID
        /// </summary>
        [JsonPropertyName("RackId")]
        [JsonProperty("RackId")]
        public string? RackId { get; set; }

        /// <summary>
        /// 储位ID
        /// </summary>
        [JsonPropertyName("LedId")]
        [JsonProperty("LedId")]
        public string? LedId { get; set; }

        /// <summary>
        /// 状态：1-上架，0-下架
        /// </summary>
        [JsonPropertyName("Status")]
        [JsonProperty("Status")]
        public string? Status { get; set; }

        /// <summary>
        /// 时间戳/会话ID
        /// </summary>
        [JsonPropertyName("TimeStamp")]
        [JsonProperty("TimeStamp")]
        public string? TimeStamp { get; set; }
    }


    /// <summary>
    /// API响应头
    /// </summary>
    public class ApiResponseHeader
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        [JsonPropertyName("RequestID")]
        [JsonProperty("RequestID")]
        public string RequestId { get; set; }

        /// <summary>
        /// API状态码：成功0x000000，失败0x000009
        /// </summary>
        [JsonPropertyName("APICode")]
        [JsonProperty("APICode")]
        public string ApiCode { get; set; }

        /// <summary>
        /// API描述信息
        /// </summary>
        [JsonPropertyName("APIDesc")]
        [JsonProperty("APIDesc")]
        public string ApiDesc { get; set; }
    }

    /// <summary>
    /// API响应数据
    /// </summary>
    public class ApiResponseData
    {
        /// <summary>
        /// 结果：OK/FAIL
        /// </summary>
        [JsonPropertyName("Result")]
        [JsonProperty("Result")]
        public string Result { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        [JsonPropertyName("Msg")]
        [JsonProperty("Msg")]
        public string Msg { get; set; }
    }

    /// <summary>
    /// API响应（泛型）
    /// </summary>
    /// <typeparam name="T">响应数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 响应头
        /// </summary>
        [JsonPropertyName("APIResHeader")]
        [JsonProperty("APIResHeader")]
        public ApiResponseHeader ApiResHeader { get; set; }

        /// <summary>
        /// 响应数据（泛型）
        /// </summary>
        [JsonPropertyName("APIResData")]
        [JsonProperty("APIResData")]
        public T ApiResData { get; set; }
    }


    /// <summary>
    /// 货架信息查询请求
    /// </summary>
    public class RackInfoRequest
    {
        /// <summary>
        /// 货架ID
        /// </summary>
        [JsonPropertyName("RackId")]
        [JsonProperty("RackId")]
        public string RackId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [JsonPropertyName("UserId")]
        [JsonProperty("UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("Timestamp")]
        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("SessionId")]
        [JsonProperty("SessionId")]
        public string SessionId { get; set; }
    }

    /// <summary>
    /// 储位信息
    /// </summary>
    public class CellInfo
    {
        /// <summary>
        /// 储位号
        /// </summary>
        [JsonPropertyName("cellId")]
        [JsonProperty("cellId")]
        public int? CellId { get; set; }

        /// <summary>
        /// 储位状态：0-没料（PPID可放空），1-有料（PPID要放值）
        /// </summary>
        [JsonPropertyName("status")]
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 面别：A/B
        /// </summary>
        [JsonPropertyName("side")]
        [JsonProperty("side")]
        public string? Side { get; set; }

        /// <summary>
        /// PPID信息
        /// </summary>
        [JsonPropertyName("ppid")]
        [JsonProperty("ppid")]
        public string? Ppid { get; set; }

        /// <summary>
        /// 是否有料
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool HasMaterial => Status == 1;

        /// <summary>
        /// 是否为空位
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsEmpty => Status == 0;
    }

    /// <summary>
    /// 货架信息查询响应
    /// </summary>
    public class RackInfoResponse
    {
        /// <summary>
        /// 状态码：0-成功，-1-异常
        /// </summary>
        [JsonPropertyName("code")]
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// 货架ID
        /// </summary>
        [JsonPropertyName("rackId")]
        [JsonProperty("rackId")]
        public string RackId { get; set; }

        /// <summary>
        /// 储位列表
        /// </summary>
        [JsonPropertyName("celllist")]
        [JsonProperty("celllist")]
        public List<CellInfo> CellList { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        [JsonPropertyName("message")]
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonPropertyName("timestamp")]
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [JsonPropertyName("sessionId")]
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsSuccess => Code == 0;

        /// <summary>
        /// 获取有料的储位列表
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<CellInfo> OccupiedCells => CellList?.FindAll(x => x.HasMaterial);

        /// <summary>
        /// 获取空储位列表
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<CellInfo> EmptyCells => CellList?.FindAll(x => x.IsEmpty);

        /// <summary>
        /// 获取指定面的储位列表
        /// </summary>
        public List<CellInfo> GetCellsBySide(string side)
        {
            return CellList?.FindAll(x => x.Side?.Equals(side, StringComparison.OrdinalIgnoreCase) == true);
        }

        /// <summary>
        /// 获取A面储位
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<CellInfo> SideACells => GetCellsBySide("A");

        /// <summary>
        /// 获取B面储位
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<CellInfo> SideBCells => GetCellsBySide("B");
    }
}