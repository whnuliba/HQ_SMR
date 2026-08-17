using IDS.Base;
using IDS.Common;
using IDS.Common.Utils;
using IDS.Extension;
using IDS.Fms.Adapter;
using IDS.Ioc;
using IDS.Logistics.Schedule;
using IDS.Persistence;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static IDS.Base.IdsFilter;
using static LinqToDB.Common.Configuration;

namespace IDS.Schedule
{
    [Route("schedule")]
    [PropertiesAutowired]
    [ApiController]
    public class CwQrtzScheduleJobController : DbBaseController<CwQrtzScheduleJob>
    {
        public CwQrtzScheduleJobAdapter CwQrtzScheduleJobAdapter { get; set; }

        public IScheduleService ScheduleService { get; set; }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override DbBaseAdapter<CwQrtzScheduleJob> Adapter()
        {
            return CwQrtzScheduleJobAdapter;
        }

        /**
         * 暂停任务
         * @param data
         * @return
         */
        [HttpPost]
        [Route("pauseJob")]
        public async Task<ResponseEntity<string>> pauseJob(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            await CwQrtzScheduleJobAdapter.PauseJob(data.data);
            return ResponseEntity<string>.Success("");
        }

        [HttpPost]
        [Route("pauseJobs")]
        public async Task<ResponseEntity<string>> pauseJobs(RequestData<List<string>> data)
        {
            if (!RequestData<List<string>>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            data.data.ForEach(async c=>{

                await CwQrtzScheduleJobAdapter.PauseJob(c);
            });
            return ResponseEntity<string>.Success("");
        }

        /**
         * 恢复任务
         * @param data
         * @return
         */
        [HttpPost]
        [Route("resumeJob")]
        public async Task<ResponseEntity<string>> resumeJob(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            CwQrtzScheduleJobAdapter.ResumeJob(data.data);
            return ResponseEntity<string>.Success("");
        }

        [HttpPost]
        [Route("resumeJobs")]
        public async Task<ResponseEntity<string>> resumeJobs(RequestData<List<string>> data)
        {
            if (!RequestData<List<string>>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            data.data.ForEach(async c=>{
               await CwQrtzScheduleJobAdapter.ResumeJob(c);
            });
            return ResponseEntity<string>.Success("");
        }

        /**
         * 删除任务
         * @param data
         * @return
         */
        [HttpPost]
        [Route("deleteJob")]
        public async Task<ResponseEntity<bool>> deleteJob(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<bool>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            await CwQrtzScheduleJobAdapter.DeleteJob(data.data);
            return ResponseEntity<bool>.Success(true);
        }

        /**
         * 创建任务
         * @param data
         * @return
         */
        [HttpPost]
        [Route("createJob")]
        public async Task<ResponseEntity<int>> createJob(RequestData<CwQrtzScheduleJob> data)
        {
            if (!RequestData<CwQrtzScheduleJob>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var res= await CwQrtzScheduleJobAdapter.CreateJob(data.data);
            return ResponseEntity<int>.Success(res);
        }

        /**
         * 启动任务
         * @param data
         * @return
         */
        [HttpPost]
        [Route("start")]
        public async Task<ResponseEntity<DateTime>> start(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<DateTime>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var res = await CwQrtzScheduleJobAdapter.Start(data.data);
            return ResponseEntity<DateTime>.Success(DateTime.Now);
        }


        /**
         * 启动任务
         * @param data
         * @return
         */
        [HttpPost]
        [Route("starts")]
        public async Task<ResponseEntity<DateTime>> starts(RequestData<List<string>> data)
        {
            if (!RequestData<List<string>>.isRequest(data))
                return ResponseEntity<DateTime>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            data.data.ForEach(async c=>{
                await CwQrtzScheduleJobAdapter.Start(c);
            });
            return ResponseEntity<DateTime>.Success(DateTime.Now);
        }


        [HttpPost]
        [Anonymous]
        [Route("getall")]
        public async Task<ResponseEntity<Page<VCwQrtzScheduleJob>>> getPages(Page<VCwQrtzScheduleJob> data)
        {

            var res = await CwQrtzScheduleJobAdapter.All(data,null);
            return ResponseEntity<Page<VCwQrtzScheduleJob>>.Success(res);
        }
        [Anonymous]
        [HttpPost]
        [Route("guest/query")]
        public async Task<ResponseEntity<Page<VCwQrtzScheduleJob>>> getEsbPages(Page<VCwQrtzScheduleJob> data)
        {

            Expression<Func<VCwQrtzScheduleJob, bool>> predicate = null;
            if (data != null && data.requestData != null) {
                if (!string.IsNullOrEmpty(data.requestData.ScheduleCode)) {
                    predicate = f=>f.ScheduleCode == data.requestData.ScheduleCode;
                }
                if (!string.IsNullOrEmpty(data.requestData.ScheduleName) && predicate!=null)
                {
                    predicate = predicate.And(f => f.ScheduleName == data.requestData.ScheduleName);
                }
                if (!string.IsNullOrEmpty(data.requestData.ScheduleName) && predicate == null)
                {
                    predicate = f => f.ScheduleName == data.requestData.ScheduleName;
                }
            }
            var res = await CwQrtzScheduleJobAdapter.All(data, predicate);

            return ResponseEntity<Page<VCwQrtzScheduleJob>>.Success(res);
        }

        [HttpPost]
        [Route("updateCronJobClass")]
        public async Task<ResponseEntity<int>> updateCronAndJobClass(RequestData<CwQrtzScheduleJob> data)
        {
            if (!RequestData<CwQrtzScheduleJob>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var res = CwQrtzScheduleJobAdapter.UpdateCronAndJobClass(data.data);
            return ResponseEntity<int>.Success(await res);
        }
        [HttpPost]
        [Route("update-parameter")]
        public async Task<ResponseEntity<int>> UpdateParameter(RequestData<CwQrtzScheduleJob> data)
        {
            if (!RequestData<CwQrtzScheduleJob>.isRequest(data))
                return ResponseEntity<int>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var res = CwQrtzScheduleJobAdapter.UpdateParameter(data.data);
            return ResponseEntity<int>.Success(await res);
        }

        
        /*



        */
    }
}
