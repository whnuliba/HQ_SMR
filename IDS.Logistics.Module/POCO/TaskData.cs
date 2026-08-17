using IDS.Logistics.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Logistics.Module.POCO
{
    public class TaskData
    {
        public CwLogisticsRoad Road { set; get; }
        public List<CwLogisticsCarrierInfo> CarrierInfos { set; get; }
        public int Priority { set; get; }
    }
}
