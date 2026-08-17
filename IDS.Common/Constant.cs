using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Common
{
    public class IdsConstant
    {
        public const String NOT_ROLE_CODE = "ROLE_LOGIN";

        public const String SYS_APP_INFO = "SYSTEM";
        public const String UKEY_APP_CODE = "UKEY_CODE";
        public const String UKEY_APP_ADDR = "UKEY_ADDR";

        public const String UKEY_SECRET = "UKEY_SECRET";

        public const String ROLE_PREFIX = "ROLE_";
        public const String COOKIE_TOKEN = "AUTH_USER";
        public const String SUPER_ADMIN_ROLE = "ROLE_SUPER_ADMIN";
        public const String ADMIN_ROLE = "ROLE_ADMIN";
        public const String SUPER_ADMIN_ACCOUNT = "chradmin";
        public const String SYS_PARAMS_PREFIX = "SYS_PARAMS_";
        public const String SYS_PARAMS_PREFIX_V2 = "SYS_PARAMS:";
        public const String LOCAL_CACHE_PROCESS_PARAMS_KEY = "CELLTYPE_PROCESS_LOCAL_CACHE_KEY";
        public const String EXT_DATA_SOURCE_CACHE_KEY = "EXT_DATA_SOURCE_CACHE_KEY_";


        /**
         * 假电芯条码
         */
        public String FAKE_CELL_BARCODE = "#";
    }
}
