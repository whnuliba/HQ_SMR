using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Base
{
    public class IdsFilter
    {
        /// <summary>
        /// 不需要登陆的地方加个这个空的拦截器
        /// </summary>
        public class AuthFilterAttribute : ActionFilterAttribute { }
        public class AnonymousAttribute : ActionFilterAttribute {
           public bool CheckIsuuer{ get; set; } 
           public bool CheckAudience { get; set; }
           public AnonymousAttribute() { }
            public AnonymousAttribute(bool checkAudience) {
                CheckAudience = checkAudience; 
            }
         }
        
    }
}
