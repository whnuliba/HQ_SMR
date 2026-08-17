using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Persistence
{
    public class IdsRedisLock
    {
        //private readonly ConnectionMultiplexer _redis;
        //private readonly IDatabase _database;

        public IdsRedisLock(IdsRedis redisClient)
        {
            RedisClient = redisClient;
        }
        public virtual IdsRedis RedisClient { set; get; }
        public async Task<bool> LockAsync(string? lockKey, string? value, TimeSpan lockTimeout)
        {
            var acquired = await RedisClient.GetDatabase().StringSetAsync(lockKey, value, lockTimeout, When.NotExists);
            if (!acquired)
            {
                // 循环重试直到成功或超时
                var startTime = DateTime.UtcNow;
                while (true)
                {
                    Thread.Sleep(10); // 等待一段时间后再重试

                    if (await RedisClient.GetDatabase().StringSetAsync(lockKey, value, lockTimeout, When.NotExists))
                    {
                        acquired = true; // 成功获取到锁
                        break;
                    }
                }
            }
            return acquired;
        }

        public async Task UnLockAsync(string lockKey, string value)
        {
            //验证锁的拥有者才能释放锁
            string currentValue = await RedisClient.GetDatabase().StringGetAsync(lockKey);
            if (currentValue == value)
            {
                await RedisClient.GetDatabase().KeyDeleteAsync(lockKey);
            }
        }



        public bool Lock(string? lockKey, string? value, TimeSpan lockTimeout)
        {
            var acquired =  RedisClient.GetDatabase().StringSet(lockKey, value, lockTimeout, When.NotExists);
            if (!acquired)
            {
                // 循环重试直到成功或超时
                var startTime = DateTime.UtcNow;
                while (true)
                {
                    Thread.Sleep(10); // 等待一段时间后再重试

                    if (RedisClient.GetDatabase().StringSet(lockKey, value, lockTimeout, When.NotExists))
                    {
                        acquired = true; // 成功获取到锁
                        break;
                    }
                }
            }
            return acquired;
        }

        public async Task UnLock(string lockKey, string value)
        {
            //验证锁的拥有者才能释放锁
            string currentValue = await RedisClient.GetDatabase().StringGetAsync(lockKey);
            if (currentValue == value)
            {
                await RedisClient.GetDatabase().KeyDeleteAsync(lockKey);
            }
        }

    }
}
