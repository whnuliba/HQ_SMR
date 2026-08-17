using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common.Utils
{
    public class IdUtils
    {
        private static long Mid = 0l;
        private static long Cid = 0l;
        public static long Id { get {

                if (Mid != 0 && Cid != 0) {
                   return SnowFlakeWorker.GetNextId(Mid, Cid);
                }

                string mid = AppConfig.GetConfigInfo("SnowFlake:Mid");
                string cid = AppConfig.GetConfigInfo("SnowFlake:Cid");
                if (string.IsNullOrEmpty(mid) || string.IsNullOrEmpty(cid)) {
                    throw new BussinessException("please configure mechanical identify argument");
                }
                if (!long.TryParse(mid, out long _mid)) {
                    throw new BussinessException("the mechanical identify argument is illegal and it requre long type");
                }
                if (!long.TryParse(cid, out long _cid))
                {
                    throw new BussinessException("the Data Center identify argument is illegal and it requre long type");
                }
                Mid = _mid;
                Cid = _cid;
                if (Mid != 0 && Cid != 0)
                {
                    return SnowFlakeWorker.GetNextId(Mid, Cid);
                }
                throw new BussinessException("the SnowFlake identify argument is illegal and it is 0");
            } }
    }
}
