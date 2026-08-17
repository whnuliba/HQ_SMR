using IDS.Base;
using Mysqlx.Crud;

namespace IDS.Security.Jwt
{
    public class JwtUser
    {

        public String id { set; get; }
        public  String username { set; get; }
        public  String password { set; get; }
        public  bool enabled { set; get; }
        public String realName { set; get; }
        public String deptId { set; get; }
        public String jobId { set; get; }
        public String orgId { set; get; }
        public String mobile { set; get; }
        public String deptCode { set; get; }
        public String jobNo { set; get; }
        public String orgCode { set; get; }
        public String deptName { set; get; }
        public String jobName { set; get; }
        public String orgName { set; get; }
        public DateTime? AccountExpireTime { get; set; }
        public DateTime? PasswordExpireTime { get; set; }
        public string? Alias { get; set; }
        public string? Lock { get; set; }
        public string? ChangePassword { get; set; }
        public string? NameSpell { get; set; }

        public List<SimpleGrantedAuthority> Authorities { set; get; }

        public JwtUser() { }
        public JwtUser(
                String id,
                String username,
                String password, List<SimpleGrantedAuthority> authorities,
                bool enabled
        )
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.Authorities = authorities;
            this.enabled = enabled;
        }

        public JwtUser(
                String realName,
                String id,
                String username,
                String password, List<SimpleGrantedAuthority> authorities,
                bool enabled
        )
        {
            this.realName = realName;
            this.id = id;
            this.username = username;
            this.password = password;
            this.Authorities = authorities;
            this.enabled = enabled;
        }
    }
}
