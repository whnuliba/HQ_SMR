using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.Device.Communication
{
    public class LocationUtils
    {
        public static int DeviceToSmr(int i) {
            return i + 1;
        }
        public static int SmrToDevice(int i)
        {
            if (i == 0) return 0;
            return i - 1;
        }
    }
}
