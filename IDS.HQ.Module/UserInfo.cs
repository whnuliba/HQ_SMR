using IDS.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Module
{
    public class UserInfo : IdsBaseEntity
    {
      public string? Username { set; get; }
        public string? Password { set; get; }
        public string? RealName { set; get; }
        public string? WorkNo { set; get; }
        public string? Permissions { set; get; }
    }
}
