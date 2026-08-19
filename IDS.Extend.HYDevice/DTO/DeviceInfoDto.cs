using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice.DTO
{
    public class DeviceInfoDto
    {
        public string DeviceNo { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string Type { get ; set; }

        public string Address { get; set; }
        public string port { get; set; }
        public string RackNo { get; set; }
        public bool Success { get { 
              if(string.IsNullOrEmpty(Type) && Type=="0x00") return true; return false;
            }}
    }

    /// <summary>
    /// 切换测试模    式
    /// </summary>
    public class Test { 
      public string RackNo { set; get; }
      public byte Mode { get; set; }
    }
    /// <summary>
    /// 切换测试模式
    /// </summary>
    public class TestRequest
    {
        public string RackNo { get; set; }
        public byte Mode { get; set; }  // 1-测试模式，0-正常模式
    }

    /// <summary>
    /// 单灯闪烁请求
    /// </summary>
    public class SingleLightFlashingRequest
    {
        public string RackNo { get; set; }      // 货架编号
        public int Times { get; set; }           // 闪烁次数：9999一直闪烁，0停止闪烁
        public int Addr { get; set; }            // 储位编号
        public byte Color { get; set; }          // 颜色序号
    }

    /// <summary>
    /// 循环全亮/灭请求
    /// </summary>
    public class LightAllLoopRequest
    {
        public string RackNo { get; set; }
        public byte Length { get; set; }         // 颜色数目
        public byte Mode { get; set; }           // 1-亮，0-灭
    }

    /// <summary>
    /// 多彩灯亮/灭请求
    /// </summary>
    public class LightOneLoopByColorRequest
    {
        public string RackNo { get; set; }
        public int Addr { get; set; }            // 开始储位序号
        public int LedQty { get; set; }          // 灯数
        public byte Length { get; set; }         // 颜色数目
        public byte Mode { get; set; }           // 1-亮，0-灭
    }

    /// <summary>
    /// 单灯亮/灭请求
    /// </summary>
    public class LightOneLoopRequest
    {
        public string RackNo { get; set; }
        public int Addr { get; set; }            // 开始储位序号
        public int LedQty { get; set; }          // 灯数
        public byte Color { get; set; }          // 颜色序号
        public byte Mode { get; set; }           // 1-亮，0-灭
    }

    /// <summary>
    /// 所有灯全亮请求
    /// </summary>
    public class LightAllRequest
    {
        public string RackNo { get; set; }
        public byte Color { get; set; }          // 颜色代号
    }

    /// <summary>
    /// 所有灯全灭请求
    /// </summary>
    public class DownAllRequest
    {
        public string RackNo { get; set; }
    }

    /// <summary>
    /// 单灯亮请求
    /// </summary>
    public class LightSingleRequest
    {
        public string RackNo { get; set; }
        public int LedAddr { get; set; }         // 灯的地址
        public byte Color { get; set; }          // 颜色代号
    }

    /// <summary>
    /// 单灯灭请求
    /// </summary>
    public class DownSingleRequest
    {
        public string RackNo { get; set; }
        public int LedAddr { get; set; }         // 灯的地址
    }

    /// <summary>
    /// 多灯亮请求
    /// </summary>
    public class LightMultiRequest
    {
        public string RackNo { get; set; }
        public List<int> LedAddrs { get; set; }  // 地址列表，必须从小到大排序，单次控制672灯
        public byte Color { get; set; }          // 颜色代号
    }

    /// <summary>
    /// 多灯灭请求
    /// </summary>
    public class DownMultiRequest
    {
        public string RackNo { get; set; }
        public List<int> LedAddrs { get; set; }  // 地址列表，必须从小到大排序，单次控制672灯
    }

    /// <summary>
    /// 报警灯/蜂鸣器请求
    /// </summary>
    public class AlarmLightRequest
    {
        public string RackNo { get; set; }
        public string ShelfSide { get; set; }    // 货架面
        public byte Color { get; set; }          // 0-红灯，1-绿灯，2-蜂鸣器
    }

    /// <summary>
    /// 取消报警灯/蜂鸣器请求
    /// </summary>
    public class AlarmDownRequest
    {
        public string RackNo { get; set; }
        public string RackSide { get; set; }    // 货架面
        public byte Color { get; set; }          // 0-红灯，1-绿灯，2-蜂鸣器
    }

    /// <summary>
    /// 上感应货架请求
    /// </summary>
    public class UpInductiveShelfRequest
    {
        public string RackNo { get; set; }
        public string ShelfSide { get; set; }    // 货架面
    }

    /// <summary>
    /// 取消上感应货架请求
    /// </summary>
    public class CancelUpInductiveShelfRequest
    {
        public string RackNo { get; set; }
        public string PkgId { get; set; }        // 取消发送命令包的ID
    }

    /// <summary>
    /// 下感应货架请求
    /// </summary>
    public class DownInductiveShelfRequest
    {
        public string RackNo { get; set; }
        public List<int> LocationAddrs { get; set; }  // 要下的储位列表
    }

    /// <summary>
    /// 取消下感应货架请求
    /// </summary>
    public class CancelDownInductiveShelfRequest
    {
        public string RackNo { get; set; }
        public string PkgId { get; set; }        // 取消发送命令包的ID
    }

    /// <summary>
    /// 报警请求
    /// </summary>
    public class AlarmRequest
    {
        public string RackNo { get; set; }
        public string ShelfSide { get; set; }    // 货架面
        public int Mode { get; set; }            // 0-单个储位；1-多个储位；2-单面
        public int Length { get; set; }          // 储位数目
        public string Addrs { get; set; }        // 储位，间隔符;
        public string AlarmMsg { get; set; }     // 报警信息
    }

    /// <summary>
    /// 取消报警请求
    /// </summary>
    public class AlarmCancelRequest
    {
        public string RackNo { get; set; }
        public string RackSide { get; set; }    // 货架面
        public int Mode { get; set; }            // 0-单个储位；1-多个储位；2-单面
        public int Length { get; set; }          // 储位数目
        public string Addrs { get; set; }        // 储位，间隔符;
    }

    /// <summary>
    /// 塔灯闪烁请求
    /// </summary>
    public class AlarmLightFlashingRequest
    {
        public string RackNo { get; set; }
        public string RackSide { get; set; }    // 货架面：0-A；1-B；2-AB
        public int Mode { get; set; }            // 0-闪烁；1-取消闪烁
    }

    /// <summary>
    /// 获取储位状态请求
    /// </summary>
    public class GetAddrStatusRequest
    {
        public string RackNo { get; set; }
    }

    /// <summary>
    /// 获取初始化配置信息请求
    /// </summary>
    public class QueryInitInfoRequest
    {
        public string RackNo { get; set; }
    }

    /// <summary>
    /// 感应货架反馈响应
    /// </summary>
    public class InductiveShelfResponse
    {
        public string ShelfNo { get; set; }
        public List<int> Addrs { get; set; }    // 变化的储位列表
        public string PkgId { get; set; }        // 命令包ID
    }

}
