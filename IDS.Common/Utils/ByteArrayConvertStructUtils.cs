using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common.Utils
{
    public class ByteArrayConvertStructUtils
    {
        // 启动OPC UA服务器
        public static ILog Log = LogManager.GetLogger(typeof(ByteArrayConvertStructUtils));
        /// <summary>
        /// 字节数组转结构体(按小端模式)
        /// </summary>
        /// <param name="bytearray">字节数组</param>
        /// <param name="obj">目标结构体</param>
        /// <param name="startoffset">bytearray内的起始位置</param>
        public static void ByteArrayToStructure(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            // 从结构体指针构造结构体
            obj = Marshal.PtrToStructure(i, obj.GetType());
            try
            {
                // 将字节数组复制到结构体指针
                Marshal.Copy(bytearray, startoffset, i, len);
            }
            catch (Exception ex)
            {
                Log.Error("ByteArrayToStructure FAIL: error " + ex.ToString());
            }
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);  //释放内存，与 AllocHGlobal() 对应

        }

        /// <summary>
        /// 字节数组转结构体(按大端模式)
        /// </summary>
        /// <param name="bytearray">字节数组</param>
        /// <param name="obj">目标结构体</param>
        /// <param name="startoffset">bytearray内的起始位置</param>
        public static void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            byte[] temparray = (byte[])bytearray.Clone();
            // 从结构体指针构造结构体
            obj = Marshal.PtrToStructure(i, obj.GetType());
            // 做大端转换
            object thisBoxed = obj;
            Type test = thisBoxed.GetType();
            int reversestartoffset = startoffset;
            // 列举结构体的每个成员，并Reverse
            foreach (var field in test.GetFields())
            {
                object fieldValue = field.GetValue(thisBoxed); // Get value

                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());  //Get Type
                if (typeCode != TypeCode.Object)  //如果为值类型
                {
                    Array.Reverse(temparray, reversestartoffset, Marshal.SizeOf(fieldValue));
                    reversestartoffset += Marshal.SizeOf(fieldValue);
                }
                else  //如果为引用类型
                {
                    reversestartoffset += ((byte[])fieldValue).Length;
                }
            }
            try
            {
                //将字节数组复制到结构体指针
                Marshal.Copy(temparray, startoffset, i, len);
            }
            catch (Exception ex)
            {
                Log.Error("ByteArrayToStructure FAIL: error " + ex.ToString());
            }
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);  //释放内存
        }
    }
}
