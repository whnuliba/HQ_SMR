using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Jwt
{
    public class SimpleGrantedAuthority { 
       public string? Role { get; set; }
    
    }
    public class JwtResponse
    {
        public String token { set; get; }
        public JwtUser userInfo { set; get; }

        public JwtResponse(String token)
        {
            this.token = token;
        }
        public JwtResponse(String token, JwtUser userInfo)
        {
            this.token = token;
            this.userInfo = userInfo;
        }
    }
}
