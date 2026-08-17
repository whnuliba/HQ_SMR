using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public abstract class LongBaseEntity
    {
        public long? Id { get; set; } = null!;
        public string? CreateUser { get; set; } = null!;

        public string? LastModifyUser { get; set; }
        [IdsColumn]
        public int? Status { get; set; }

        public abstract void saveInit();

        public abstract void updateInit();
    }
}
