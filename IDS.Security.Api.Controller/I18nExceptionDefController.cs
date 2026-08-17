using IDS.Base;
using IDS.Common.Utils;
using IDS.Common;
using IDS.Ioc;
using IDS.Security.IService;
using IDS.Security.Module;
using IDS.Security.Service;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Security.Adapter;
using IDS.Security.IService.DTO;

namespace IDS.Security.Api.Controller
{
    [Route("i18n")]
    [PropertiesAutowired]
    [ApiController]
    public class I18nExceptionDefController : ControllerBase
    {
        public I18nExceptionDefAdapter I18nExceptionDefAdapter { get; set; }

        [Route("guest/get-i18n")]
        [HttpPost]
        public ResponseEntity<Dictionary<string, object>> exceptionCode(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<Dictionary<string, object>>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var result = I18nExceptionDefAdapter.Exception(data.data);
            if (result.Success)
                return ResponseEntity<Dictionary<string, object>>.Success(result.Data);
            return ResponseEntity<Dictionary<string, object>>.Error();
        }

        [Route("guest/get-exce-str")]
        [HttpPost]
        public ResponseEntity<string> exception(RequestData<I18nExceptionDefDto> data)
        {
            if (!RequestData<I18nExceptionDefDto>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var result = I18nExceptionDefAdapter.Exception(data.data.ExceptionCode, data.data.Ln);
            if (result.Success)
                return ResponseEntity<string>.Success(result.Data);
            return ResponseEntity<string>.Error();
        }
        [Route("guest/refresh")]
        [HttpPost]
        public ResponseEntity<string> refresh(RequestData<string> data)
        {
            if (!RequestData<string>.isRequest(data))
                return ResponseEntity<string>.Error(EnumUtil.GetEnumDescription(IdsErrorCode.PARAMETER_NULL));
            var result = I18nExceptionDefAdapter.Refresh(data.data);
            if (result.Success)
                return ResponseEntity<string>.Success(result.Data);
            return ResponseEntity<string>.Error();
        }
        [Route("guest/refreshAll")]
        [HttpPost]
        public ResponseEntity<string> refreshAll()
        {
            var result = I18nExceptionDefAdapter.Refresh();
            if (result.Success)
                return ResponseEntity<string>.Success(result.Data);
            return ResponseEntity<string>.Error();
        }


        [Route("add")]
        [HttpPost]
        public ResponseEntity<string> Save(RequestData<I18nExceptionDef> record)
        {

            if (!RequestData<I18nExceptionDef>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            I18nExceptionDefAdapter.save(record.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route("delete")]
        [HttpPost]
        public ResponseEntity<string> delete(RequestData<I18nExceptionDef> record)
        {


            if (!RequestData<I18nExceptionDef>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            I18nExceptionDefAdapter.delete(record.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route("del")]
        [HttpPost]
        public ResponseEntity<string> deleteById(RequestData<string>? record)
        {

            if (!RequestData<string>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            I18nExceptionDefAdapter.deleteById(record.data);
            return ResponseEntity<string>.Success("OK");
        }
        [Route("edit")]
        [HttpPost]
        public ResponseEntity<string> update(RequestData<I18nExceptionDef> record)
        {

            if (!RequestData<I18nExceptionDef>.isRequest(record))
            {
                return ResponseEntity<string>.Error("执行失败,参数不存在!");
            }
            I18nExceptionDefAdapter.update(record.data);
            return ResponseEntity<string>.Success("OK");

        }


        [Route("list")]
        [HttpPost]
        public virtual ResponseEntity<Page<I18nExceptionDef>> List(Page<I18nExceptionDef> data)
        {
            var page = I18nExceptionDefAdapter.List(data, null);
            return ResponseEntity<Page<I18nExceptionDef>>.Success(page);
        }
    }
}
