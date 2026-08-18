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
        DOWN_WAIT = 1,//等地下架
        UP_COMPLETE = 2, //上架完成
        DOWN_COMPLETE = 3,//下架完成
    }
    public enum LocationStates { 
        //空闲
        FREE = 0,
        //载货
        LOADING = 1, 
    }
}
