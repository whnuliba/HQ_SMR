using IDS.Base;
using IDS.Common;

namespace IDS.HQ.Service
{
    public interface IRackTaskService<T>
    {
        IdsResult<T> Putway(T data);
    }
}
