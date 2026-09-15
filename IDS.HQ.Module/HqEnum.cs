using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.Module
{
    public enum TaskTypes { 
      IN=0, //入库
      OUT=1,//出库
    }
    public enum TaskStates
    {
        UP_WAIT = 0, //等待上架
        DOWN_WAIT = 1,//等待下架
        UP_COMPLETE = 2, //上架完成
        DOWN_COMPLETE = 3,//下架完成
    }
    public enum TaskCmds
    {
        Up,//--"Up"=请求上架命令  --"Up_end"=请求上架结束
        Up_end
    }
    public enum LocationStates { 
        //空闲
        FREE = 0,
        //载货
        LOADING = 1, 
    }
    public enum Light { 
      R =1,
      G = 2,
      B = 3,
    }

    public enum Enables {
        Forbidden=0,
        Action=1,
    }

    /// <summary>
    /// 储位亮灯颜色
    /// 红-1；淡白-2；绿-3；蓝-4；黄绿-5；紫-6；黄-7；浅蓝-8；浅黄-9
    /// </summary>
    public enum LightColor
    {
        [System.ComponentModel.Description("红")]
        Red = 1,

        [System.ComponentModel.Description("淡白")]
        LightWhite = 2,

        [System.ComponentModel.Description("绿")]
        Green = 3,

        [System.ComponentModel.Description("蓝")]
        Blue = 4,

        [System.ComponentModel.Description("黄绿")]
        YellowGreen = 5,

        [System.ComponentModel.Description("紫")]
        Purple = 6,

        [System.ComponentModel.Description("黄")]
        Yellow = 7,

        [System.ComponentModel.Description("浅蓝")]
        LightBlue = 8,

        [System.ComponentModel.Description("浅黄")]
        LightYellow = 9
    }

    public enum ALARM { 
       RED = 0,
       GREEN = 1,
        //蜂鸣器
       BUZZER = 2,
    }

}
