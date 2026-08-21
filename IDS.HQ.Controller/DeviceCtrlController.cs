using IDS.Base;
using IDS.Common;
using IDS.Extend.HYDevice;
using IDS.Extend.HYDevice.DTO;
using IDS.HQ.HYDevice.Protocol;
using IDS.HQ.Module;
using IDS.HQ.Module.DTO;
using IDS.Ioc;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDS.HQ.Controller
{
    [Route("deviceCtrl")]
    [PropertiesAutowired]
    [ApiController]
    public class DeviceCtrlController
    {
        [HttpPost]
        [Route("Test")]
        public ResponseEntity<object> Test(RequestData<Test> data) {
            if (!RequestData<Test>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");
            //SmartMaterialRackNode
            byte[]message = DeviceMessage.GetTestModeMessage(data.data.Mode);
            IdsResult<object> res;
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message, session => {
                res = session.HandlerResult;
            });
            return ResponseEntity<object>.Success("ok");
        }
        /// <summary>
        /// 单灯闪烁
        /// </summary>
        [HttpPost]
        [Route("SingleLightFlashing")]
        public ResponseEntity<object> SingleLightFlashing(RequestData<SingleLightFlashingRequest> data)
        {
            if (!RequestData<SingleLightFlashingRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetSingleLightFlashMessage(
                data.data.Times,
                data.data.Addr,
                data.data.Color);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 循环全亮、灭
        /// </summary>
        [HttpPost]
        [Route("LightAllLoop")]
        public ResponseEntity<object> LightAllLoop(RequestData<LightAllLoopRequest> data)
        {
            if (!RequestData<LightAllLoopRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetLoopAllLightOnOffMessage(data.data.Length, data.data.Mode);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 多彩灯亮、灭
        /// </summary>
        [HttpPost]
        [Route("LightOneLoopByColor")]
        public ResponseEntity<object> LightOneLoopByColor(RequestData<LightOneLoopByColorRequest> data)
        {
            if (!RequestData<LightOneLoopByColorRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetMultiColorLightOnOffMessage(
                data.data.Addr,
                data.data.LedQty,
                data.data.Length,
                data.data.Mode);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 单灯亮、灭
        /// </summary>
        [HttpPost]
        [Route("LightOneLoop")]
        public ResponseEntity<object> LightOneLoop(RequestData<LightOneLoopRequest> data)
        {
            if (!RequestData<LightOneLoopRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetSingleLightOnOffMessage(
                data.data.Addr,
                data.data.LedQty,
                data.data.Color,
                data.data.Mode);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 所有灯全亮
        /// </summary>
        [HttpPost]
        [Route("LightAll")]
        public ResponseEntity<object> LightAll(RequestData<LightAllRequest> data)
        {
            if (!RequestData<LightAllRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetAllLightOnMessage(data.data.Color);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 所有灯全灭
        /// </summary>
        [HttpPost]
        [Route("DownAll")]
        public ResponseEntity<object> DownAll(RequestData<DownAllRequest> data)
        {
            if (!RequestData<DownAllRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetAllLightOffMessage();
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 单灯亮
        /// </summary>
        [HttpPost]
        [Route("LightSingle")]
        public ResponseEntity<object> LightSingle(RequestData<LightSingleRequest> data)
        {
            if (!RequestData<LightSingleRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetSingleLightOnMessage(data.data.LedAddr, data.data.Color);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 单灯灭
        /// </summary>
        [HttpPost]
        [Route("DownSingle")]
        public ResponseEntity<object> DownSingle(RequestData<DownSingleRequest> data)
        {
            if (!RequestData<DownSingleRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetSingleLightOffMessage(data.data.LedAddr);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 多灯亮
        /// </summary>
        [HttpPost]
        [Route("LightMulti")]
        public ResponseEntity<object> LightMulti(RequestData<LightMultiRequest> data)
        {
            if (!RequestData<LightMultiRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            if (data.data.LedAddrs == null || data.data.LedAddrs.Count == 0)
                return ResponseEntity<object>.Error("地址列表不能为空");

            // 验证地址列表是否从小到大排序
            var sortedList = data.data.LedAddrs.OrderBy(x => x).ToList();
            if (!data.data.LedAddrs.SequenceEqual(sortedList))
                return ResponseEntity<object>.Error("地址列表必须从小到大排序");

            if (data.data.LedAddrs.Count > 672)
                return ResponseEntity<object>.Error("单次控制最多672个灯");

            byte[] message = DeviceMessage.GetMultiLightOnMessage(data.data.LedAddrs, data.data.Color);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 多灯灭
        /// </summary>
        [HttpPost]
        [Route("DownMulti")]
        public ResponseEntity<object> DownMulti(RequestData<DownMultiRequest> data)
        {
            if (!RequestData<DownMultiRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            if (data.data.LedAddrs == null || data.data.LedAddrs.Count == 0)
                return ResponseEntity<object>.Error("地址列表不能为空");

            // 验证地址列表是否从小到大排序
            var sortedList = data.data.LedAddrs.OrderBy(x => x).ToList();
            if (!data.data.LedAddrs.SequenceEqual(sortedList))
                return ResponseEntity<object>.Error("地址列表必须从小到大排序");

            if (data.data.LedAddrs.Count > 672)
                return ResponseEntity<object>.Error("单次控制最多672个灯");

            byte[] message = DeviceMessage.GetMultiLightOffMessage(data.data.LedAddrs);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 大灯亮，蜂鸣器响
        /// </summary>
        [HttpPost]
        [Route("AlarmLight")]
        public ResponseEntity<object> AlarmLight(RequestData<AlarmLightRequest> data)
        {
            if (!RequestData<AlarmLightRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte shelfSide = data.data.ShelfSide == "A" ? (byte)0 : (byte)1;
            byte[] message = DeviceMessage.GetBigLightOnBuzzerMessage(shelfSide, data.data.Color, true);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 大灯灭，蜂鸣器停
        /// </summary>
        [HttpPost]
        [Route("AlarmDown")]
        public ResponseEntity<object> AlarmDown(RequestData<AlarmDownRequest> data)
        {
            if (!RequestData<AlarmDownRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte shelfSide = data.data.RackSide == "A" ? (byte)0 : (byte)1;
            byte[] message = DeviceMessage.GetBigLightOnBuzzerMessage(shelfSide, data.data.Color, false);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }

        /// <summary>
        /// 获取初始化配置信息
        /// </summary>
        [HttpPost]
        [Route("QueryInitInfo")]
        public ResponseEntity<object> QueryInitInfo(RequestData<QueryInitInfoRequest> data)
        {
            if (!RequestData<QueryInitInfoRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");
            byte[] message = DeviceMessage.GetQueryInitInfo();
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("ok");
        }
        /// <summary>
        /// 取消报警
        /// </summary>
        [HttpPost]
        [Route("AlarmCancel")]
        public ResponseEntity<object> AlarmCancel(RequestData<AlarmCancelRequest> data)
        {
            if (!RequestData<AlarmCancelRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");
            byte alarmAddr = (data.data.RackSide == "A") ? (byte)0 : (byte)1;
            byte[] message = DeviceMessage.GetAlarmLight(alarmAddr, false);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("取消报警请求已发送");
        }

        /// <summary>
        /// 塔灯闪烁
        /// </summary>
        [HttpPost]
        [Route("AlarmLightFlashing")]
        public ResponseEntity<object> AlarmLightFlashing(RequestData<AlarmLightFlashingRequest> data)
        {
            if (!RequestData<AlarmLightFlashingRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte shelfSide = 0;
            if (data.data.RackSide == "A")
                shelfSide = 0;
            else if (data.data.RackSide == "B")
                shelfSide = 1;
            else if (data.data.RackSide == "AB")
                shelfSide = 2;

            byte[] message = DeviceMessage.GetAlarmLightFlashingMessage(shelfSide, data.data.Mode);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("塔灯闪烁请求已发送");
        }

        /// <summary>
        /// 大灯亮，蜂鸣 器报警  
        /// shelfNo:货架编号
        //shelfSide:货架面
        //color:0-红灯，1-绿灯， 2-蜂鸣器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAlarmLightToLight")]
        public ResponseEntity<object> GetAlarmLightToLight(RequestData<AlarmDownRequest> data) {
            if (!RequestData<AlarmDownRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");
            byte alarmAddr = (data.data.RackSide == "A") ? (byte)0 : (byte)1;
            byte[] message = DeviceMessage.GetAlarmLight(alarmAddr, true);
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("塔灯闪烁请求已发送");
        }

        /// <summary>
        /// 获取储位状态
        /// </summary>
        [HttpPost]
        [Route("GetAddrStatus")]
        public ResponseEntity<object> GetAddrStatus(RequestData<GetAddrStatusRequest> data)
        {
            if (!RequestData<GetAddrStatusRequest>.isRequest(data))
                return ResponseEntity<object>.Error("请传入合法参数");

            byte[] message = DeviceMessage.GetAddrStatusMessage();
            SmartMaterialRackNode.Instance.NoticeRack(data.data.RackNo, message);
            return ResponseEntity<object>.Success("获取储位状态请求已发送");
        }
    }
}
