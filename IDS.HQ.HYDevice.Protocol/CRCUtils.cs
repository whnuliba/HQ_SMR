using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.HYDevice.Protocol
{
    public class CRCUtils
    {
        public static byte[] GetCRC16(byte[] data)
        {
            byte b = byte.MaxValue;
            byte b2 = byte.MaxValue;
            byte b3 = 1;
            byte b4 = 160;
            for (int i = 0; i < data.Length; i++)
            {
                b ^= data[i];
                for (int j = 0; j <= 7; j++)
                {
                    byte b5 = b2;
                    byte b6 = b;
                    b2 = (byte)(b2 >> 1);
                    b = (byte)(b >> 1);
                    bool flag = (b5 & 1) == 1;
                    if (flag)
                    {
                        b |= 128;
                    }
                    bool flag2 = (b6 & 1) == 1;
                    if (flag2)
                    {
                        b2 ^= b4;
                        b ^= b3;
                    }
                }
            }
            return new byte[]
            {
                b2,  b
            };
        }
    }
}
