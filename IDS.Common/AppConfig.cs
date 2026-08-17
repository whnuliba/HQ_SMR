using Microsoft.Extensions.Configuration;

namespace IDS.Common
{
    /// <summary>
    /// 获取.NetCore配置文件信息
    /// </summary>
    public class AppConfig
    {

        public static IConfigurationRoot configuration { get; set; }
        public static void InitGlobalConfiguration(ConfigurationManager configuration)
        {
            configuration = configuration;
        }
        private static object obj = new object();
        public static string GetConfigInfo(string Key)
        {

            if (configuration == null)
            {
                lock (obj)
                {
                    if (configuration == null)
                    {
                        var builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json");
                        configuration = builder.Build();
                    }

                }
            }
            //IConfigurationRoot configuration = builder.Build();
            string configStr = configuration.GetSection($"{Key}").Value;
            if (!string.IsNullOrWhiteSpace(configStr))
            {
                return configStr;
            }
            return null;
        }

    }
}
