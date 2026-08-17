using System;
using System.Threading;

namespace IDS.Base
{
    public class CurrentUser
    {
        private static readonly AsyncLocal<AuthUserInfo> _asyncLocal = new();
        private static readonly AsyncLocal<IdsUserInfo> _asyncIdsUserLocal = new();
        public static void SetUser(AuthUserInfo userInfo)
        {
            _asyncLocal.Value=userInfo;
        }
        public static void Remove() {
            _asyncLocal.Value = null;
        }
        public static AuthUserInfo GetUserInfo() {
            return _asyncLocal.Value; ;
        }
        public string UserName() {
            return null;
        }


        public static void SetUser(IdsUserInfo userInfo)
        {
            _asyncIdsUserLocal.Value = userInfo;
        }
        public static void IdsRemove()
        {
            _asyncIdsUserLocal.Value = null;
        }
        public static IdsUserInfo GetIdsUserInfo()
        {
            return _asyncIdsUserLocal.Value; ;
        }

    }
    public class AuthUserInfo {
        public string UserName { set; get; }
        public string RealName { set; get; }
        public string Mobile { set; get; }
        public string Id { set; get; }
    }

}
