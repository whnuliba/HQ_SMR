using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static log4net.Appender.ColoredConsoleAppender;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.HQ.HYDevice.Protocol
{
    public class DeviceMessage : BaseProtocol
    {



        //测试专用
        public static byte[] GetTestfMessage()
        {
            return GetMessage(15, 0, null);
        }

        public static byte[] GetTestModeMessage(byte mode)
        {
            return GetMessage(16, 27, new byte[] { mode });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmAddrs">报警的储位号</param>
        /// <param name="alarmMode">报警模式</param>
        /// <param name="locationMode">位置模式</param>
        /// <param name="side">0 A面 1B面</param>
        /// <returns></returns>
        public static byte[] GetAlarm(List<int> alarmAddrs, int alarmMode, int locationMode, byte side) {
            int num = 18 + alarmAddrs.Count * 2;
            byte[] data = new byte[alarmAddrs.Count];
            byte[] array = new byte[num-13];
            array[0] = side;
            array[1] = (byte)alarmMode;
            array[2] = (byte)locationMode;
            string strLength = alarmAddrs.Count.ToString("x").PadLeft(4, '0');
            array[3] = Convert.ToByte(strLength.Substring(0, 2), 16);
            array[4] = Convert.ToByte(strLength.Substring(2), 16);
            for (int j = 0; j < alarmAddrs.Count; j++)
            {
                string addrStr = alarmAddrs[j].ToString("x").PadLeft(4, '0');
                array[4 + 2 * j + 1] = Convert.ToByte(addrStr.Substring(0, 2), 16);
                array[4 + 2 * j + 2] = Convert.ToByte(addrStr.Substring(2), 16);
            }
            return GetMessage((byte)(num + 2), 28, data);

        }

        /// <summary>
        /// 切换测试模
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cmd1"></param>
        /// <param name="cmdType"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] GetModeSwitchMessage(byte mode)
        {
            byte[] data = { mode };
            return GetMessage(16, 27, data);
        }

        //TODO 单灯闪烁
        /// <summary>
        /// 单灯闪烁
        /// </summary>
        /// <param name="times">闪缩次数</param>
        /// <param name="addr">位置</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static byte[] GetSingleLightFlashMessage(int times, int addr, byte color)
        {
            byte[] message = new byte[5];
            string timesStr = times.ToString("x").PadLeft(4, '0');
            message[0] = Convert.ToByte(timesStr.Substring(0, 2), 16);
            message[1] = Convert.ToByte(timesStr.Substring(2), 16);
            string addrStr = addr.ToString("x").PadLeft(4, '0');
            message[2] = Convert.ToByte(addrStr.Substring(0, 2), 16);
            message[3] = Convert.ToByte(addrStr.Substring(2), 16);
            message[4] = color;
            return GetMessage(20, 29, message);
        }
        //TODO   循环全亮、灭
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">颜色数目</param>
        /// <param name="mode"> 1-亮，0-灭</param>
        /// <returns></returns>
        public static byte[] GetLoopAllLightOnOffMessage(byte length, byte mode)
        {
            byte[] message = { length, mode };
            // Implementation for loop all light on/off message
            return GetMessage(17, 24, message);
        }
        //TODO   多彩灯亮、灭
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr">开始储位序号</param>
        /// <param name="ledQty">灯数</param>
        /// <param name="length">:颜色数目</param>
        /// <param name="mode">:1-亮，0-灭</param>
        /// <returns></returns>
        public static byte[] GetMultiColorLightOnOffMessage(int addr, int ledQty, byte length, byte mode)
        {
            byte[] message = new byte[6];
            message[0] = mode;
            string addrStr = addr.ToString("x").PadLeft(4, '0');
            message[1] = Convert.ToByte(addrStr.Substring(0, 2), 16);
            message[2] = Convert.ToByte(addrStr.Substring(2), 16);
            string ledQtyStr = ledQty.ToString("x").PadLeft(4, '0');
            message[3] = Convert.ToByte(ledQtyStr.Substring(0, 2), 16);
            message[4] = Convert.ToByte(ledQtyStr.Substring(2), 16);
            message[5] = length;
            return GetMessage(21, 25, message);
        }
        //TODO 单灯亮、灭
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr">开始储位序号</param>
        /// <param name="ledQty">灯数</param>
        /// <param name="color">颜色序号</param>
        /// <param name="mode">1-亮，0-灭</param>
        /// <returns></returns>
        public static byte[] GetSingleLightOnOffMessage(int addr, int ledQty, byte color, byte mode)
        {
            byte[] message = new byte[6];
            message[0] = mode;
            string addrStr = addr.ToString("x").PadLeft(4, '0');
            message[1] = Convert.ToByte(addrStr.Substring(0, 2), 16);
            message[2] = Convert.ToByte(addrStr.Substring(2), 16);
            string ledQtyStr = ledQty.ToString("x").PadLeft(4, '0');
            message[3] = Convert.ToByte(ledQtyStr.Substring(0, 2), 16);
            message[4] = Convert.ToByte(ledQtyStr.Substring(2), 16);
            message[5] = color;
            return GetMessage(21, 26, message);
        }
        // TODO  所有灯全亮
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">颜色代号</param>
        /// <returns></returns>
        public static byte[] GetAllLightOnMessage(byte color)
        {
            byte[] message = { color };
            return GetMessage(16, 6, message);
        }

        //TODO  所有灯全灭
        public static byte[] GetAllLightOffMessage()
        {
            return GetMessage(15, 9, null);
        }
        // TODO  单灯亮
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledAddr">灯的地址</param>
        /// <param name="color">颜色代号</param>
        /// <returns></returns>
        public static byte[] GetSingleLightOnMessage(int ledAddr, byte color)
        {
            byte[] message = new byte[3];
            string text = ledAddr.ToString("x").PadLeft(4, '0');
            message[0] = Convert.ToByte(text.Substring(0, 2), 16);
            message[1] = Convert.ToByte(text.Substring(2), 16);
            message[2] = color;
            return GetMessage(18, 7, message);
        }
        // TODO  单灯灭
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledAddr">灯的地址</param>
        /// <returns></returns>
        public static byte[] GetSingleLightOffMessage(int ledAddr)
        {
            byte[] message = new byte[2];
            string text = ledAddr.ToString("x").PadLeft(4, '0');
            message[0] = Convert.ToByte(text.Substring(0, 2), 16);
            message[1] = Convert.ToByte(text.Substring(2), 16);
            return GetMessage(17, 10, message);
        }
        // TODO  多灯亮
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledAddr">地址列表,必须从小到大排序，单次控制672灯</param>
        /// <param name="color">颜色代号</param>
        /// <returns></returns>
        public static byte[] GetMultiLightOnMessage(List<int> ledAddrs, byte color)
        {
            int num = 5 + ledAddrs.Count * 2 + 1 + 10;
            byte[] message = new byte[2 + ledAddrs.Count * 2 + 1];
            string text = (num + 2).ToString("x").PadLeft(4, '0');
            message[0] = Convert.ToByte(text.Substring(0, 2), 16);
            message[1] = Convert.ToByte(text.Substring(2), 16);
            for (int j = 0; j < ledAddrs.Count; j++)
            {
                string text2 = ledAddrs[j].ToString("x").PadLeft(4, '0');
                message[1 + j * 2 + 1] = Convert.ToByte(text2.Substring(0, 2), 16);
                message[1 + j * 2 + 2] = Convert.ToByte(text2.Substring(2), 16);
            }
            message[1 + ledAddrs.Count * 2 + 1] = color;

            return GetMessage(0, 8, message);
        }
        // TODO  多灯灭
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledAddrs">地址列表,必须从小到大排序，单次控制672灯</param>
        /// <returns></returns>
        public static byte[] GetMultiLightOffMessage(List<int> ledAddrs)
        {
            int num = 5 + ledAddrs.Count * 2 + 1 + 10;
            byte[] message = new byte[2 + ledAddrs.Count * 2+1];
            string text = (num + 2).ToString("x").PadLeft(4, '0');
            message[0] = Convert.ToByte(text.Substring(0, 2), 16);
            message[1] = Convert.ToByte(text.Substring(2), 16);
            for (int j = 0; j < ledAddrs.Count; j++)
            {
                string text2 = ledAddrs[j].ToString("x").PadLeft(4, '0');
                message[1 + j * 2 + 1] = Convert.ToByte(text2.Substring(0, 2), 16);
                message[1 + j * 2 + 2] = Convert.ToByte(text2.Substring(2), 16);
            }
            message[1 + ledAddrs.Count * 2 + 1] = 0;
            return GetMessage(0, 8, message);
        }
        //TODO 大灯亮，蜂鸣器响
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmAddr">货架面</param>
        /// <param name="color">0-红灯，1-绿灯，2-蜂鸣器</param>
        /// <param name="light">是否亮灯</param>
        /// <returns></returns>
        public static byte[] GetBigLightOnBuzzerMessage(byte alarmAddr, byte color, bool light)
        {
            byte[] message = new byte[3];
            message[0] = alarmAddr;
            message[1] = color;
            bool flag = !light;
            if (flag)
            {
                message[2] = 0;
            }
            else
            {
                message[2] = 1;
            }
            return GetMessage(18, 12, message);
        }


        //TODO 大灯灭，蜂鸣器停 light来控制
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmAddr">货架面</param>
        /// <param name="color">0-红灯，1-绿灯，2-蜂鸣器</param>
        /// <param name="light">是否亮灯</param>
        /// <returns></returns>
        public static byte[] GetBigLightOnBuzzerMessage(byte shelfSide, bool light)
        {
            byte[] message = new byte[5];
            bool flag = shelfSide.Equals("A");
            byte b;
            if (flag)
            {
                b = 0;
            }
            else
            {
                b = 1;
            }
            message[0] = b;
            message[1] = 1;
            bool flag2 = !light;
            if (flag2)
            {
                message[2] = 0;
            }
            else
            {
                message[2] = 1;
            }
            message[3] = b;
            message[3] = 2;
            bool flag3 = !light;
            if (flag3)
            {
                message[4] = 0;
            }
            else
            {
                message[4] = 1;
            }
            return GetMessage(21, 12, message);
        }

        // TODO 查询初始化信息返回
        public static byte[] GetQueryInitInfo() {
            return GetMessage(15, 5, null);
        }


        //  ===================================================================================

        public static byte[] GetAlarmLight(byte side, bool light) {
            int num = 19;
            byte[] array = new byte[6];
            array[0] = side;
            array[1] = 1;
            bool flag2 = !light;
            if (flag2)
            {
                array[2] = 0;
            }
            else
            {
                array[2] = 1;
            }
            array[3] = side;
            array[4] = 2;
            bool flag3 = !light;
            if (flag3)
            {
                array[5] = 0;
            }
            else
            {
                array[5] = 1;
            }
            return GetMessage((byte)(num+2), 12, array);
        }

        /// <summary>
        /// 大灯灭，蜂鸣 器灭 走该接口
        /// </summary>
        /// <param name="alarmAddr"></param>
        /// <param name="color"></param>
        /// <param name="light"></param>
        /// <returns></returns>
        public static byte[] GetAlarmLight(byte alarmAddr, byte color, bool light)
        {
            byte[] array = new byte[3];
            array[0] = alarmAddr;
            array[1] = color;
            if (light)
            {
                array[2] = 1;
            }
            else
            {
                array[2] = 0;
            }
            return GetMessage(18, 12, array);
        }

       
        /// <summary>
        /// 塔灯闪烁（对应文档API中的 AlarmLightFlashing）
        /// </summary>
        /// <param name="shelfSide">货架面 0-A；1-B；2-AB</param>
        /// <param name="mode">0-闪烁；1-取消闪烁</param>
        /// <returns></returns>
        public static byte[] GetAlarmLightFlashingMessage(byte shelfSide, int mode)
        {
            byte[] data = new byte[2];
            data[0] = shelfSide;
            data[1] = (byte)mode;
            return GetMessage(17, 30, data);
        }

        /// <summary>
        /// 获取储位状态（对应文档API中的 GetAddrStatus）
        /// </summary>
        /// <returns></returns>
        public static byte[] GetAddrStatusMessage()
        {
            return GetMessage(15, 16, null);
        }

    }
}
