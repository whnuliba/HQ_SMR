using System;
using System.Collections.Generic;

namespace IDS.Security.Module;

public partial class InterfaceTicket
{
    public string Id { get; set; } = null!;

    public DateTime? CreateTime { get; set; }

    public string? CreateUser { get; set; }

    public DateTime? LastModifyTime { get; set; }

    public string? LastModifyUser { get; set; }

    public int? Status { get; set; }

    public string? Ip { get; set; } = null!;

    public string? Code { get; set; } = null!;

    public int? Type { get; set; }

    public string? Ticket { get; set; } = null!;

    public string? BizCode { get; set; } = null!;

    public string? InterfaceName { get; set; }
}
