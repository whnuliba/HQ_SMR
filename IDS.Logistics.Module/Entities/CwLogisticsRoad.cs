using IDS.Base;
using System;
using System.Collections.Generic;

namespace IDS.Logistics.Module.Entities;

/// <summary>
/// 物流路径
/// </summary>
public partial class CwLogisticsRoad : IdsBaseEntity
{
    public string? FromLocationCode { get; set; }

    public int? FromLocationCmd { get; set; }

    public string? FromService { get; set; }

    public string? FromCondCode { get; set; }

    public string? ToLocationCode { get; set; }

    public int? ToLocationCmd { get; set; }

    public string? ToService { get; set; }

    public string? ToCondCode { get; set; }

    public int? TaskBalance { get; set; }

    public int? FromAreaCode { get; set; }

    public int? FromLocationType { get; set; }

    public int? FromProcessCode { get; set; }

    public int? FromMaterialCode { get; set; }

    public int? FromScanCode { get; set; }

    public int? FromState1 { get; set; }

    public int? FromState2 { get; set; }

    public int? FromState3 { get; set; }

    public int? FromState4 { get; set; }

    public int? ToLocationType { get; set; }

    public int? ToAreaCode { get; set; }

    public int? ToProcessCode { get; set; }

    public int? ToMaterialCode { get; set; }

    public int? ToScanCode { get; set; }

    public int? ToState1 { get; set; }

    public int? ToState2 { get; set; }

    public int? ToState3 { get; set; }

    public int? ToState4 { get; set; }

    public string? CarrierTypeService { get; set; }

    public string? BeforeCreateService { get; set; }

    public string? AfterCreateService { get; set; }

    public string? BeforeSendService { get; set; }

    public string? AfterSendService { get; set; }

    public string? BeforeCompleteService { get; set; }

    public string AfterCompleteService { get; set; } = null!;
}
