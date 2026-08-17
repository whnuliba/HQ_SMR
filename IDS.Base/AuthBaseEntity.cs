using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public class AuthBaseEntity : BaseEntity
    {


        public DateTime? CreateDate { get; set; }


        public DateTime? LastModifyDate { get; set; }


        public override void saveInit() {

            CreateDate = DateTime.Now;
            CreateUser = CurrentUser.GetUserInfo()?.UserName;
        }

        public override void updateInit()
        {

            LastModifyDate = DateTime.Now;
            LastModifyUser = CurrentUser.GetUserInfo()?.UserName;
        }

    }



}
