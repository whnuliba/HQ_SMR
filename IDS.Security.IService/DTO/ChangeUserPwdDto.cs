using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.DTO
{
    public class ChangeUserPwdDto
    {
        public String? userName { set; get; }
        public String? password { set; get; }
        public String? newPassword { set; get; }
    }
}
