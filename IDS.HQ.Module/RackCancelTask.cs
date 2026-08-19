using IDS.Base;

namespace IDS.HQ.Module
{
    public class RackCancelTask : IdsLongBaseEntity
    {

        public string? RackNo { get; set; }
        public int? TaskState { get; set; }
        public string? PPID { get; set; }
        public int? Location { get; set; }
        public int? TaskType { get; set; }
        public string? TaskDescription { get; set; }
        public string? MaterialNo { get; set; }
        public string? MaterialName { get; set; }
        public string? Locations { get; set; }
        public string? RackSide { get; set; }
    }



}








 