using IDS.Common;
using IDS.Extend.HYDevice.ReceiveHandler;
using IDS.HQ.Module;
using IDS.Ioc;
using IDS.Persistence;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.Extend.HYDevice.Utils
{
    public class RackRunningLogUtils<T> where T: IDSContext
    {
        public static ILog Logger = LogManager.GetLogger(typeof(RackRunningLogUtils<T>));

        public static async Task Error<E>(string message) where E : class
        {
            Logger.Error(message);
            await CreateLog<E>(LogLevel.Error, message);
        }
        public static async Task Info<E>(string message) where E : class
        {
            Logger.Info(message);
            await CreateLog<E>(LogLevel.Info, message);
        }
        public static async Task Warm<E>(string message) where E : class
        {
            Logger.Warn(message);
            await CreateLog<E>(LogLevel.Warm, message);
        }
        public static async Task CreateLog<E>(LogLevel level, string message) where E : class
        {
            var log = new RackRunningLog
            {
                LogLevel = level.ToString(),
                Message = message,
                Class = nameof(E)
            };
            log.saveInit();
            await CreateLog(log);
        }

        public static async Task CreateLog<E>(LogLevel level, string msg, params string [] arguments) 
        {
          string message = string.Format(msg, arguments);
          await CreateLog<E>(level, message);
        }
        public static async Task CreateLog(RackRunningLog log)
        {
            await IdsMessageHandler<object>.ErrorMessage(log?.Message);
            IDbContextFactory<T> dbContext = ContainerUtils.GetRequiredService<IDbContextFactory<T>>();
            using (var ctx = dbContext.CreateDbContext()) {
                log.saveInit();
                ctx.Insert(log);
            }
        }

        public static async Task CreateLog(IDSContext ctx, RackRunningLog log)
        {
            await IdsMessageHandler<object>.ErrorMessage(log?.Message);
            log.saveInit();
            ctx.Insert(log);
        }
    }

    public enum LogLevel { 
       Error,
       Info,
       Warm,
    }
}
