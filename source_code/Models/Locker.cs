using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class Locker
{
    public string Id { get; set; } = null!;

    public string LockerId { get; set; } = null!;

    public string LockerName { get; set; } = null!;

    public bool? LockerStatus { get; set; }

    public string? UnlockCode { get; set; }

    public DateTime? UcodeValidDate { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<AccessWarning> AccessWarnings { get; set; } = new List<AccessWarning>();
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingOrder> BookingOrders { get; set; } = new List<BookingOrder>();
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<PackageInfo> PackageInfos { get; set; } = new List<PackageInfo>();
}
