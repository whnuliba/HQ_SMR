using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public class IdsBaseEntity : BaseEntity
    {
        public DateTime? CreateTime { get; set; }

        public DateTime? LastModifyTime { get; set; }
        public override void saveInit()
        {

            CreateTime = DateTime.Now;
            CreateUser = CurrentUser.GetUserInfo()?.UserName;
        }

        public override void updateInit()
        {

            LastModifyTime = DateTime.Now;
            LastModifyUser = CurrentUser.GetUserInfo()?.UserName;
        }
    }
}
