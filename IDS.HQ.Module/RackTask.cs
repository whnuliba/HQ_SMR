using IDS.Base;

namespace IDS.HQ.Module
{
    public class RackTask:IdsBaseEntity
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
        public int? AfterLightColor { get; set; } //任务完成对应的颜色
        public string? TaskCmd { set; get; } //"Up"=请求上架命令  --"Up_end"=请求上架结束
        public string? ExtendId { get; set; }
        public string? TaskGroupId { get; set; }
        public string? OperateType { set; get; }
    }



}








 