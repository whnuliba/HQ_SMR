using IDS.Common;
using IDS.Security.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService
{
    public interface ISequenceGeneratorService : ISecBaseService<SequenceGenerator>
    {
        IdsResult<string> GeneratorNo(string clz);
        IdsResult<string> GeneratorNo(String clz, String pfix, String lockStr);
        IdsResult<string> GeneratorNo(String clz, String pfix, int seqLen, String lockStr);
    }
}
