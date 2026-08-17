using IDS.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.HYDevice.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    public unsafe struct BaseStruct
    public struct BaseStruct
    {
        public byte FrameHead;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] IdPrefix = {0xFF,0xFF };
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Id;
        public byte Cmd1; // 报文长度
        public byte CmdType; //指令类型

        public BaseStruct(byte[] id, byte cmd1, byte cmdType)
        {
            FrameHead = 170;
            Cmd1 = cmd1;
            CmdType = cmdType;
            Id = new byte[8];
            if (id != null)
            {
                int len = Math.Min(id.Length, 8);
                Array.Copy(id, 0, Id, 0, len);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct HYDeviceStruct
    {
        public BaseStruct BaseStruct;
        public byte[] Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CRC;
        public HYDeviceStruct(BaseStruct baseStruct, byte[] data)
        {
            BaseStruct = baseStruct;
            Data = data;
            var bytes = ByteUtils.StructToBytesWithMarshal(BaseStruct);
            var msg = ByteUtils.Combine(bytes, Data);
            var crc = CRCUtils.GetCRC16(msg);
            CRC = new byte[2];
            CRC[0] = crc[1];
            CRC[1] = crc[0];
        }

       }
        public struct BaseStructWithLangId
        {
            public byte FrameHead;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] Id;
            public byte Cmd1; // 报文长度
            public byte CmdType; //指令类型

            public BaseStructWithLangId(byte[] id, byte cmd1, byte cmdType)
            {
                FrameHead = 170;
                Cmd1 = cmd1;
                CmdType = cmdType;
                Id = new byte[10];
                if (id != null)
                {
                    int len = Math.Min(id.Length, 10);
                    Array.Copy(id, 0, Id, 0, len);
                }
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HYDeviceStructWithLangId
        {
            public BaseStructWithLangId BaseStructWithLangId;
            public byte[] Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] CRC;
            public HYDeviceStructWithLangId(BaseStructWithLangId baseStruct, byte[] data)
            {
                BaseStructWithLangId = baseStruct;
                Data = data;
                var bytes = ByteUtils.StructToBytesWithMarshal(BaseStructWithLangId);
                var msg = ByteUtils.Combine(bytes, Data);
                var crc = CRCUtils.GetCRC16(msg);
                CRC = new byte[2];
                CRC[0] = crc[1];
                CRC[1] = crc[0];
            }
        }
    #region  业务报文

    #region  模式切换

    public class BaseProtocol
    {

        public static byte[] CreateMessage(byte[] id, byte cmd1, byte cmdType, byte[] data)
        {
            var baseStruct = new BaseStruct(id, cmd1, cmdType);
            var headMessageTypes = ByteUtils.StructToBytesWithMarshal(baseStruct);
            var dataMessage = data!=null && data.Length>0?ByteUtils.Combine(headMessageTypes, data): headMessageTypes;
            var crc = CRCUtils.GetCRC16(dataMessage);

            byte [] CRC = new byte[2];
            CRC[0] = crc[1];
            CRC[1] = crc[0];
           return ByteUtils.Combine(dataMessage, CRC);
        }

        public static byte[] CreateMessageWithLangId(byte[] id, byte cmd1, byte cmdType, byte[] data)
        {
            var baseStruct = new BaseStructWithLangId(id, cmd1, cmdType);
            var headMessageTypes = ByteUtils.StructToBytesWithMarshal(baseStruct);
            var dataMessage = data != null && data.Length > 0 ? ByteUtils.Combine(headMessageTypes, data) : headMessageTypes;
            var crc = CRCUtils.GetCRC16(dataMessage);

            byte[] CRC = new byte[2];
            CRC[0] = crc[1];
            CRC[1] = crc[0];
            return ByteUtils.Combine(dataMessage, CRC);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd1">报文长度</param>
        /// <param name="cmdType">报文类型</param>
        /// <param name="data">报文体</param>
        /// <returns></returns>
        public static byte[] GetMessage(byte cmd1, byte cmdType, byte [] data)
        {
            BaseUtil baseUtil = new BaseUtil();
            long timestamp = baseUtil.GetSnowFlakeId(1l, 1l);
            Console.WriteLine("id:" + timestamp);
            byte[] id = BitConverter.GetBytes(timestamp);
            var hyDeviceStruct = CreateMessage(id, cmd1, cmdType, data);
            return hyDeviceStruct;
        }

        public static byte[] GetMessage(byte [] id,byte cmd1, byte cmdType, byte[] data)
        {
            var hyDeviceStruct = CreateMessageWithLangId(id, cmd1, cmdType, data);
            return hyDeviceStruct;
        }
        public void CheckMessage1(byte[] message)
        {
            if (message == null || message.Length < 14)
            {
                throw new Exception("报文长度不正确");
            }
            byte[] crc = new byte[2];
            Array.Copy(message, message.Length - 2, crc, 0, 2);
            byte[] data = new byte[message.Length - 2];
            Array.Copy(message, 0, data, 0, message.Length - 2);
            var crcCheck = CRCUtils.GetCRC16(data);
            if (crc[0] != crcCheck[1] || crc[1] != crcCheck[0])
            {
                throw new Exception("报文CRC校验失败");
            }
        }


        public void CheckMessage(byte[] message)
        {
            // 1. 最小长度检查：固定头(1+2+8+1+1) + 最小Data(0) + CRC(2) = 15 字节
            if (message == null || message.Length < 16)
            {
                throw new Exception($"报文长度不正确，至少需要 15 字节，实际 {message?.Length ?? 0} 字节");
            }

            // 2. 检查帧头
            if (message[0] != 0xAA)
            {
                throw new Exception($"帧头错误，期望 0xAA，实际 0x{message[0]:X2}");
            }

            // 4. 读取长度字段（第 12 个字节，索引 11）
            int dataLength = message[11];
            Console.WriteLine($"Data 长度: {dataLength}");

            // 5. 计算预期报文总长度 = 13 (固定头) + dataLength + 2 (CRC)
            int expectedLength = 13 + dataLength + 2;  // 13 = 1+2+8+1+1
            if (message.Length != expectedLength)
            {
                throw new Exception($"报文长度不匹配，期望 {expectedLength} 字节，实际 {message.Length} 字节");
            }

            // 6. CRC 校验
            // 提取 CRC（最后 2 字节）
            byte[] receivedCrc = new byte[2];
            Array.Copy(message, message.Length - 2, receivedCrc, 0, 2);

            // 提取数据部分（从开头到 CRC 之前）
            byte[] dataForCrc = new byte[message.Length - 2];
            Array.Copy(message, 0, dataForCrc, 0, message.Length - 2);

            // 计算 CRC
            byte[] calculatedCrc = CRCUtils.GetCRC16(dataForCrc);

            // 注意：你的 CRC 是高位在后，低位在前（小端序存储）
            if (receivedCrc[0] != calculatedCrc[1] || receivedCrc[1] != calculatedCrc[0])
            {
                throw new Exception($"CRC 校验失败，期望 {calculatedCrc[1]:X2}{calculatedCrc[0]:X2}，实际 {receivedCrc[0]:X2}{receivedCrc[1]:X2}");
            }

            Console.WriteLine("报文校验通过 ✅");
        }
    }
    #endregion

    #endregion

}
