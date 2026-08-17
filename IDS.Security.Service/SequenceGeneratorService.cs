using IDS.Base.Utils;
using IDS.Common;
using IDS.Ioc;
using IDS.Persistence;
using IDS.Security.IService;
using IDS.Security.Module;
using Microsoft.AspNetCore.Http;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Security.Service
{
    [AutoInjection]
    public class SequenceGeneratorService : SecBaseService<SequenceGenerator, AuthDbContext>, ISequenceGeneratorService
    {
     
        public IdsRedisLock IdsRedisLock { get; set; }
    public  IdsResult<string> GeneratorNo(string clz)
        {
            string lockStr = "generator_lock:generatorNo";
            string prefix = DateTime.Now.ToString("yyyyMM");
            string value = BaseUtil.uuid();
            try
            {
                if (IdsRedisLock.Lock(lockStr, value, TimeSpan.FromSeconds(10)))
                    {
                        try
                        {
                        using (var ctx = DbContext())
                        {
                            string update = $"update SEQUENCE_GENERATOR set INCREASE =  INCREASE+1\r\n  " +
                                $"where CLASSIFICATION = '{clz}'" +
                                $" AND PREFIX='{prefix}'";
                            ctx.Sql(update);
                            SequenceGenerator generator = ctx.SequenceGenerator.Where(c => c.Classification == clz && c.Prefix == prefix).FirstOrDefault();
                            if (generator == null)
                            {
                                generator = new SequenceGenerator();
                                generator.saveInit();
                                generator.Id = BaseUtil.uuid();
                                generator.Increase = 1;
                                generator.Prefix = prefix;
                                generator.Classification = clz;
                                int i = ctx.Save(generator);
                                if (i <= 0)
                                    return IdsResult<string>.ok(false); //string.format("%04d", 1)
                                return IdsResult<string>.ok(true, null, prefix + "1".PadLeft(4, '0'));
                            }
                            return IdsResult<string>.ok(true, null, prefix + (generator.Increase + "").PadLeft(4, '0'));
                          }
                        }
                        catch (Exception ex)
                        {
                            return IdsResult<string>.ok(false, ex.Message);
                        }
                        finally
                        {
                            IdsRedisLock.UnLock(lockStr, value);
                        }
                    }
            }
            catch (Exception e)
            {
                return IdsResult<string>.ok(false, e.Message);
            }
            return IdsResult<string>.ok(false);
        }

        public IdsResult<string> GeneratorNo(string clz, string pfix, string lockStr)
        {
            string lkStr = "generator_common_lock:generatorNo";
            string prefix = DateTime.Now.ToString("yyyyMM");
            string value = BaseUtil.uuid();

            if (!string.IsNullOrWhiteSpace(lockStr))
                lkStr = lockStr;
            if (!string.IsNullOrWhiteSpace(pfix))
                prefix = pfix + prefix;
            try
            {
                if (IdsRedisLock.Lock(lkStr, value, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        using (var ctx = DbContext())
                        {
                            string update = $"update SEQUENCE_GENERATOR set INCREASE =  INCREASE+1\r\n  " +
                                $"where CLASSIFICATION = '{clz}'" +
                                $" AND PREFIX='{prefix}'";
                            ctx.Sql(update);
                            SequenceGenerator generator = ctx.SequenceGenerator.Where(c => c.Classification == clz && c.Prefix == prefix).FirstOrDefault();
                            if (generator == null)
                            {
                                generator = new SequenceGenerator();
                                generator.saveInit();
                                generator.Id = BaseUtil.uuid();
                                generator.Increase = 1;
                                generator.Prefix = prefix;
                                generator.Classification = clz;
                                int i = ctx.Save(generator);
                                if (i <= 0)
                                    return IdsResult<string>.ok(false); //string.format("%04d", 1)
                                return IdsResult<string>.ok(true, null, prefix + "1".PadLeft(5, '0'));
                            }
                            return IdsResult<string>.ok(true, null, prefix + (generator.Increase + "").PadLeft(5, '0'));
                        }
                    }
                    catch (Exception ex)
                    {
                        return IdsResult<string>.ok(false, ex.Message);
                    }
                    finally
                    {
                         IdsRedisLock.UnLock(lockStr, value);
                    }
                }
            }
            catch (Exception e)
            {
                return IdsResult<string>.ok(false, e.Message);
            }
            return IdsResult<string>.ok(false);
        }

        public IdsResult<string> GeneratorNo(string clz, string pfix, int seqLen, string lockStr)
        {
            if (seqLen == 0)
                seqLen = 6;
            string lkStr = "generator_common_lock:generatorNo";
            string prefix = DateTime.Now.ToString("yyyyMM");
            string value = BaseUtil.uuid();

            if (!string.IsNullOrWhiteSpace(lockStr))
                lkStr = lockStr;
            if (!string.IsNullOrWhiteSpace(pfix))
                prefix = pfix + prefix;
            try
            {
                if (IdsRedisLock.Lock(lkStr, value, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        using (var ctx = DbContext())
                        {
                            string update = $"update SEQUENCE_GENERATOR set INCREASE = INCREASE+1\r\n  " +
                                $"where CLASSIFICATION = '{clz}'" +
                                $" AND PREFIX='{prefix}'";
                            ctx.Sql(update);
                            SequenceGenerator generator = ctx.SequenceGenerator.Where(c => c.Classification == clz && c.Prefix == prefix).FirstOrDefault();
                            if (generator == null)
                            {
                                generator = new SequenceGenerator();
                                generator.saveInit();
                                generator.Id = BaseUtil.uuid();
                                generator.Increase = 1;
                                generator.Prefix = prefix;
                                generator.Classification = clz;
                                int i = ctx.Save(generator);
                                if (i <= 0)
                                    return IdsResult<string>.ok(false); //string.format("%04d", 1)
                                return IdsResult<string>.ok(true, null, prefix + "1".PadLeft(seqLen, '0'));
                            }
                            return IdsResult<string>.ok(true, null, prefix + (generator.Increase + "").PadLeft(seqLen, '0'));
                        }
                    }
                    catch (Exception ex)
                    {
                        return IdsResult<string>.ok(false, ex.Message);
                    }
                    finally
                    {
                         IdsRedisLock.UnLock(lockStr, value);
                    }
                }
            }
            catch (Exception e)
            {
                return IdsResult<string>.ok(false, e.Message);
            }
            return IdsResult<string>.ok(false);
        }
    }
}
