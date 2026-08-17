using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.HQ.Module.DTO
{
    public class RackTaskDto
    {
        public string? PPID { get; set; }
        public string? RackNo { get; set; }
        public TaskStates TaskState { get; set; }
        public int? LocationId { get; set; }
        public string? MaterialNo { get; set; }
        public string? RackSide { get; set; }
    }
}
