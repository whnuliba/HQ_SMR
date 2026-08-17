using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.HYDevice.Protocol
{
    public class ByteUtils
    {
        public static byte[] StructToBytesFast<T>(T structure) where T : unmanaged
        {
            Span<byte> bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref structure, 1));
            return bytes.ToArray(); // 如果不想拷贝，可以直接使用 Span<byte>
        }


        /// <summary>
        /// 将结构体转换为 byte 数组
        /// </summary>
        public static byte[] StructToBytesWithMarshal<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf<T>(structure);
            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, false);
                Marshal.Copy(ptr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return bytes;
        }

        /// <summary>
        /// 将 byte 数组转换为结构体
        /// </summary>
        public static T BytesToStruct<T>(byte[] bytes) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            if (bytes.Length < size)
                throw new ArgumentException($"字节数组长度不足，需要 {size} 字节，实际 {bytes.Length} 字节");

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, ptr, size);
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// 获取结构体在内存中的大小（字节数）
        /// </summary>
        public static int SizeOf<T>() where T : struct
        {
            return Marshal.SizeOf<T>();
        }
        public static byte[] Combine(byte[] first, byte[] second)
        {
            if (first == null) return second;
            if (second == null) return first;

            byte[] result = new byte[first.Length + second.Length];
            Span<byte> span = result;
            first.AsSpan().CopyTo(span);
            second.AsSpan().CopyTo(span.Slice(first.Length));
            return result;
        }

        /// <summary>
        /// 拼接多个 byte 数组（性能最优）
        /// </summary>
        public static byte[] Combine(params byte[][] arrays)
        {
            if (arrays == null || arrays.Length == 0)
                return Array.Empty<byte>();

            if (arrays.Length == 1)
                return arrays[0] ?? Array.Empty<byte>();

            int totalLen = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i] != null)
                    totalLen += arrays[i].Length;
            }

            byte[] result = new byte[totalLen];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i] != null)
                {
                    Buffer.BlockCopy(arrays[i], 0, result, offset, arrays[i].Length);
                    offset += arrays[i].Length;
                }
            }
            return result;
        }

        public static byte[] CombineWithBlock(byte[] first, byte[] second)
        {
            if (first == null) return second;
            if (second == null) return first;

            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }
    } 
    }
