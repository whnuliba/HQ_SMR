using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Extend.HYDevice
{
    //List<int> alarmAddrs, int alarmMode, int locationMode, string side
    public class RackAlarmInfo
    {
        public List<int> locations { set; get; }
        public LocationInfo location { set; get; }
        public int AlarmMode {set; get; }
        public int LocationMode {set; get; }
        public byte Side { set; get; }
        public string ErrorInfo { set; get; }
    }
}
