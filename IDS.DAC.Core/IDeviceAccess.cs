using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//DAC =Device Access Control
namespace IDS.DAC.Core
{
    public interface IDeviceAccess
    {
        bool Read(out byte[] buf);
        Task<bool> ReadAsync(out byte[] buf);
        bool Write(byte[] buf);
        Task<bool> WriteAsync(byte[] buf);
        bool Read<T>(out T buf) where T : class;
        Task<bool> ReadAsync<T>(out T buf) where T : class;
        bool Write<T>(T data) where T : class;
        Task<bool> WriteAsync<T>(T data) where T : class;

    }
}
