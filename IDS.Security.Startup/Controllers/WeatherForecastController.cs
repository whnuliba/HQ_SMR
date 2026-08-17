using IDS.Base;
using IDS.Ioc;
using IDS.Security.Adapter;
using IDS.Security.Module;
using IDS.Security.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDS.Bpms.Api.Controllers
{
    [ApiController]
    [PropertiesAutowired]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public virtual IDbContextFactory<AuthDbContext> DbContextFactory { get; set; }
        public virtual UserInfoAdapter UserInfoAdapter { set; get; }
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public Page<UserInfo> Get()
        {
            // ActivatorUtilities.
            string tableName = "USER_INFO";
            string where = "UserName='wanghao'";
            string orderBy = "CreateDate DESC";
            int pageIndex = 1;
            int pageSize = 20;


            var page = UserInfoAdapter.GetPage(tableName, where, orderBy, pageIndex, pageSize);

            //var dbContext = DbContextFactory.CreateDbContext();
            //using (var context = dbContext)
            //{
            //    DbSet<MenuInfo> me = context.MenuInfo;
            //    foreach (var item in me) {
            //        MenuInfo d = item as MenuInfo;
            //    }
            //    String a = "a";

            //}
            return page;

            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //    {
            //        Date = DateTime.Now.AddDays(index),
            //        TemperatureC = Random.Shared.Next(-20, 55),
            //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //    })
            //    .ToArray();
        }
    }
}