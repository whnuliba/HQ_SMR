using IDS.Base;
using IDS.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Persistence
{
    public abstract class DbBaseController<T> : ControllerBase where T : BaseEntity
    {

        public abstract DbBaseAdapter<T> Adapter();
        [Route("add")]
        [HttpPost]
        public ResponseEntity<string> Save(RequestData<T> record) {

            if (!RequestData<T>.isRequest(record)) {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
              }
            Adapter().save(record.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route("delete")]
        [HttpPost]
        public ResponseEntity<string> delete(RequestData<T> record) {


            if (!RequestData<T>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            Adapter().delete(record.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route("del")]
        [HttpPost]
        public ResponseEntity<string> deleteById(RequestData<string>? record) {

            if (!RequestData<string>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            Adapter().deleteById(record.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route("edit")]
        [HttpPost]
        public ResponseEntity<string> update(RequestData<T> record) {

            if (!RequestData<T>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            Adapter().update(record.data);
            return ResponseEntity<string>.Success("OK");

        }
        [Route("page")]
        [HttpPost]
        public ResponseEntity<Page<T>> GetPage(string tableName, string where, string orderBy, int pageIndex, int pageSize) {
           var page = Adapter().GetPage(tableName, where, orderBy, pageIndex, pageSize);
            return ResponseEntity<Page<T>>.Success(page);
        }

        [Route("list")]
        [HttpPost]
        public virtual ResponseEntity<Page<T>> List(Page<T> data)
        {
            var page = Adapter().GetPages(data, null);
            return ResponseEntity<Page<T>>.Success(page);
        }
        [Route("queryById")]
        [HttpPost]
        public virtual ResponseEntity<T> QueryById(RequestData<string>? record)
        {
            var page = Adapter().QueryById(record.data);
            return ResponseEntity<T>.Success(page);
        }
    }
}
