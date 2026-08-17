using IDS.Base;
using IDS.Common;
using LinqToDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static LinqToDB.Common.Configuration;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace IDS.Persistence
{
    public class FmsMongodbClients
    {
        private static readonly Lazy<FmsMongodbClients> lazy = new Lazy<FmsMongodbClients>(() => new FmsMongodbClients());
        public static FmsMongodbClients Singleton { get { return lazy.Value; } }

        public string MongoConnectionStr { set; get; }
        public string MongoDatabase { set; get; }
        public MongoClient Client { set; get; }

        public FmsMongodbClients()
        {
            if (string.IsNullOrEmpty(MongoConnectionStr) && string.IsNullOrEmpty(MongoDatabase))
            {
                MongoConnectionStr = AppConfig.GetConfigInfo("Mongodb:ConnectionStrings");
                MongoDatabase = AppConfig.GetConfigInfo("Mongodb:Database");
                if (string.IsNullOrEmpty(MongoConnectionStr) || string.IsNullOrEmpty(MongoDatabase))
                    return;
            }
            if (Client == null)
                Client = new MongoClient(MongoConnectionStr);

        }

        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            string name = typeof(TDocument).Name;
            return Client.GetDatabase(MongoDatabase).GetCollection<TDocument>(name);
        }
        public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
        {
            return Client.GetDatabase(MongoDatabase).GetCollection<TDocument>(name);
        }
        public async Task<List<TDocument>> FindAsync<TDocument>(string dicName, int limit, int skip, Expression<Func<TDocument, bool>> predicate)
        {
            //var filter = Builders<TDocument>.Filter.Eq("name", "John Doe"); // 查询条件
            List<TDocument> list = new List<TDocument>();
            var options = new FindOptions<TDocument> { Limit = limit, Skip = skip }; // 查询选项，例如限制返回结果的数量
            var collection = GetCollection<TDocument>(dicName);

            using (var cursor = await collection.FindAsync(predicate, options))
            {
                return await cursor.ToListAsync();
            }
        }
        public async Task<List<TDocument>> FindAsync<TDocument>(string dicName, Expression<Func<TDocument, bool>> predicate)
        {
            //var filter = Builders<TDocument>.Filter.Eq("name", "John Doe"); // 查询条件
            List<TDocument> list = new List<TDocument>();
            var collection = GetCollection<TDocument>(dicName);

            using (var cursor = await collection.FindAsync(predicate))
            {
                return await cursor.ToListAsync();
            }
        }

        public async Task<List<TDocument>> FindAsync<TDocument>(int limit, int skip, Expression<Func<TDocument, bool>> predicate)
        {
            //var filter = Builders<TDocument>.Filter.Eq("name", "John Doe"); // 查询条件
            List<TDocument> list = new List<TDocument>();
            var options = new FindOptions<TDocument> { Limit = limit, Skip = skip }; // 查询选项，例如限制返回结果的数量
            var collection = GetCollection<TDocument>();

            using (var cursor = await collection.FindAsync(predicate, options))
            {
                return await cursor.ToListAsync();
            }
        }
        public async Task<List<TDocument>> FindAsync<TDocument>(Expression<Func<TDocument, bool>> predicate)
        {
            //var filter = Builders<TDocument>.Filter.Eq("name", "John Doe"); // 查询条件
            List<TDocument> list = new List<TDocument>();
            var collection = GetCollection<TDocument>();

            using (var cursor = await collection.FindAsync(predicate))
            {
                return await cursor.ToListAsync();
            }
        }
        public async Task<Page<TDocument>> FindAsync<TDocument>(string dicName, Page<TDocument> page, Expression<Func<TDocument, bool>> predicate)
        {
            List<TDocument> list = new List<TDocument>();
            var options = new FindOptions<TDocument> { Limit = page.pageSize, Skip = (page.current - 1) * page.pageSize }; // 查询选项，例如限制返回结果的数量
            var collection = GetCollection<TDocument>(dicName);
            //var s = collection.AsQueryable().Where(predicate).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).ToList();
            using (var cursor = await collection.FindAsync(predicate, options))
            {
                list = await cursor.ToListAsync();
            }

            var count = await collection.CountAsync(predicate, null, default);
            Page<TDocument> page1 = new Page<TDocument>((int)count, list, page.pageSize, page.current);
            return page1;
        }

        public async Task<Page<TDocument>> FindAsync<TDocument>(string dicName, Page<TDocument> page, Expression<Func<TDocument, bool>> predicate, string sortJson)
        {
            List<TDocument> list = new List<TDocument>();
            var options = new FindOptions<TDocument> { Limit = page.pageSize, Skip = (page.current - 1) * page.pageSize, Sort = sortJson }; // 查询选项，例如限制返回结果的数量
            var collection = GetCollection<TDocument>(dicName);
            //var s = collection.AsQueryable().Where(predicate).Skip((page.current - 1) * page.pageSize).Take(page.pageSize).ToList();
            using (var cursor = await collection.FindAsync(predicate, options))
            {
                list = await cursor.ToListAsync();
            }

            var count = await collection.CountAsync(predicate, null, default);
            Page<TDocument> page1 = new Page<TDocument>((int)count, list, page.pageSize, page.current);
            return page1;
        }

        public async Task<Page<TDocument>> FindAsync<TDocument>(Page<TDocument> page, Expression<Func<TDocument, bool>> predicate)
        {
            //var filter = Builders<TDocument>.Filter.Eq("name", "John Doe"); // 查询条件
            List<TDocument> list = new List<TDocument>();
            var options = new FindOptions<TDocument> { Limit = page.pageSize, Skip = (page.current - 1) * page.pageSize }; // 查询选项，例如限制返回结果的数量
            var collection = GetCollection<TDocument>();

            using (var cursor = await collection.FindAsync(predicate, options))
            {
                list = await cursor.ToListAsync();
            }
            var count = await collection.CountAsync(predicate, null, default);
            Page<TDocument> page1 = new Page<TDocument>((int)count, list, page.pageSize, page.current);
            return page1;
        }

        public async Task InsertOneAsync<TDocument>(string dicName, TDocument document)
        {
            var collection = GetCollection<TDocument>(dicName);
            await collection.InsertOneAsync(document);
        }

        public async Task InsertOneAsync<TDocument>(TDocument document)
        {
            var collection = GetCollection<TDocument>();
            await collection.InsertOneAsync(document);
        }


        public void InsertOne<TDocument>(string dicName, TDocument document)
        {
            var collection = GetCollection<TDocument>(dicName);
            collection.InsertOne(document);
        }

        public void InsertOne<TDocument>(TDocument document)
        {
            var collection = GetCollection<TDocument>();
            collection.InsertOne(document);
        }

        public void UpdateMany<TDocument>(string updateDefinition, Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>();
            /*
            var update = Builders<TDocument>.Update
                        .Set(x => x.Property1, value1)
                        .Set(x => x.Property2, value2)
                        .Set(x => x.Property3, value3);
            */
            collection.UpdateMany<TDocument>(predicate, updateDefinition);
        }


        public async Task UpdateManyAsync<TDocument>(string updateDefinition, Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>();
            await collection.UpdateManyAsync<TDocument>(predicate, updateDefinition);
        }

        public void InsertList<TDocument>(List<TDocument> lstCustomer)
        {

            var collection = GetCollection<TDocument>();
            collection.InsertMany(lstCustomer);
        }
        public async Task InsertListAsync<TDocument>(List<TDocument> lstCustomer)
        {

            var collection = GetCollection<TDocument>();
            await collection.InsertManyAsync(lstCustomer);
        }


        public void InsertList<TDocument>(string name,List<TDocument> lstCustomer)
        {

            var collection = GetCollection<TDocument>(name);
            collection.InsertMany(lstCustomer);
        }
        public async Task InsertListAsync<TDocument>(string name, List<TDocument> lstCustomer)
        {

            var collection = GetCollection<TDocument>(name);
            await collection.InsertManyAsync(lstCustomer);
        }


        public DeleteResult DeleteMany<TDocument>(Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>();
            return collection.DeleteMany(predicate);
        }
        public async Task<DeleteResult> DeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>();
            return await collection.DeleteManyAsync(predicate);
        }

        public async Task<DeleteResult> DeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>();
            return await collection.DeleteOneAsync(predicate);
        }

        public DeleteResult DeleteOne<TDocument>(Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>();
            return collection.DeleteOne(predicate);
        }



        public DeleteResult DeleteMany<TDocument>(string docName, Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>(docName);
            return collection.DeleteMany(predicate);
        }
        public async Task<DeleteResult> DeleteManyAsync<TDocument>(string docName, Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>(docName);
            return await collection.DeleteManyAsync(predicate);
        }

        public async Task<DeleteResult> DeleteOneAsync<TDocument>(string docName, Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>(docName);
            return await collection.DeleteOneAsync(predicate);
        }

        public DeleteResult DeleteOne<TDocument>(string docName, Expression<Func<TDocument, bool>> predicate)
        {
            var collection = GetCollection<TDocument>(docName);
            return collection.DeleteOne(predicate);
        }

        public void bulkInsert<TDocument>(List<TDocument> document) where TDocument : WriteModel<TDocument>
        {
            var collection = GetCollection<TDocument>();
            IEnumerable<WriteModel<TDocument>> requests = document;
            collection.BulkWrite(document);
        }


        /// <summary>
        /// 构建更新操作定义 
        /// </summary>
        /// <param name="bc">bsondocument文档</param>
        /// <returns></returns>
        private List<UpdateDefinition<BsonDocument>> BuildUpdateDefinition(BsonDocument bc, string parent)
        {
            var updates = new List<UpdateDefinition<BsonDocument>>();
            foreach (var element in bc.Elements)
            {
                var key = parent == null ? element.Name : $"{parent}.{element.Name}";
                var subUpdates = new List<UpdateDefinition<BsonDocument>>();
                //子元素是对象
                if (element.Value.IsBsonDocument)
                {
                    updates.AddRange(BuildUpdateDefinition(element.Value.ToBsonDocument(), key));
                }
                //子元素是对象数组
                else if (element.Value.IsBsonArray)
                {
                    var arrayDocs = element.Value.AsBsonArray;
                    var i = 0;
                    foreach (var doc in arrayDocs)
                    {
                        if (doc.IsBsonDocument)
                        {
                            updates.AddRange(BuildUpdateDefinition(doc.ToBsonDocument(), key + $".{i}"));
                        }
                        else
                        {
                            updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
                            continue;
                        }
                        i++;
                    }
                }
                //子元素是其他
                else
                {
                    updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
                }
            }
            return updates;
        }
        /*
        /// <summary>更新</summary>
        public async Task<IEnumerable<string>> UpdateAsync(MetadataCollection metadatas)
        {
            List<string> result = null;
            var kmds = metadatas.Select(e => e.As<Metadata>()).ToList();
            var docs = kmds.ConvertAll(DicConvertToBsonDoc);
            var updateOptions = new UpdateOptions { IsUpsert = true };
            try
            {
                foreach (var doc in docs)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq(f => f[iiid], doc[iiid]);
                    var update = Builders<BsonDocument>.Update.Combine(BuildUpdateDefinition(doc, null));
                    await _access.UpdateAsync(filter, update, updateOptions);
                }
                result = metadatas.Select(s => s.IIId).ToList();
            }
            catch (Exception ex)
            {
                IndexExceptionCodes.UpdatingIndexFailed.ThrowUserFriendly(ex.Message, "更新索引失败");
            }
            return result;
        }
        */
    }


}
