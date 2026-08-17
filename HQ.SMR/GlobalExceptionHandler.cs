using IDS.Base;
using IDS.Ioc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HQ.SMR
{
    public class GlobalExceptionHandler : IAsyncExceptionFilter
    {
        public virtual ILogger<GlobalExceptionHandler> Logger { set; get; }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            // 如果异常没有被处理则进行处理
            if (context.ExceptionHandled == false)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "application/json;charset=utf-8",
                    Content = JsonConvert.SerializeObject(ResponseEntity<String>.Error(context.Exception.Message))
                };
                Logger?.LogError(context.Exception.Message);
            }
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
