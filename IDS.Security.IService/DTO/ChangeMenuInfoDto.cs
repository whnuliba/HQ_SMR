using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.IService.DTO
{
    public class ChangeMenuInfoDto
    {
        public String pid {get; set; }
        public List<String> id { get; set; }
        public String menuGroup { get; set; }
    }
}
