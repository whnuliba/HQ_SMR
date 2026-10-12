using IDS.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Module
{
    public class UserInfo : IdsBaseEntity
    {
        [IdsColumn]
      public string? Username { set; get; }
        [IdsColumn]
        public string? Password { set; get; }
        [IdsColumn]
        public string? RealName { set; get; }
        [IdsColumn]
        public string? WorkNo { set; get; }
        [IdsColumn]
        public string? Permissions { set; get; }
    }
}
