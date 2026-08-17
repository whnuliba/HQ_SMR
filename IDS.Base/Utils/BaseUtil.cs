using IDS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base.Utils
{
    public class BaseUtil
    {
        public static string uuid() { 
          return Guid.NewGuid().ToString("N");
        }
        public long GetSnowFlakeId(long cid, long mid) { 
           return  SnowFlakeWorker.GetNextId(cid, mid);
        }
    }
}
