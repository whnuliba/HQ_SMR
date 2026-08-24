using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.Extend.HYDevice.DTO
{
    public class RackLocationTaskDto
    {
        public string TaskId { get; set; }
        public List<int?> Locations { get; set; }
        public int? AfterLightColor { get; set; } //任务完成对应的颜色
    }
}
