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
        public int AlarmMode {set; get; } // 0是报警 1是取消报警 
        public int LocationMode {set; get; } //0-单个储位；1-多        个储位；2-单面
        public byte Side { set; get; }
        public string ErrorInfo { set; get; }
        public string RackNo { set; get; }
    }
}
