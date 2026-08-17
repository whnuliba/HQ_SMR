using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.DTO
{
    public class UserSessionDto
    {
        public string? UserId { set; get; }
        public string? UserName { set; get; }
        public string? SessionKey { set; get; }
        //平台
        public string? Platform { set; get; }
        public int? State { set; get; }
    }
}
