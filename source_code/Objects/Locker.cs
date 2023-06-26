using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class Locker
{
    public Locker()
    {
    }

    public Locker(string locker_name, bool locker_status, string unlock_code, DateTime validDate)
    {
        this.lockerName = locker_name;
        this.lockerStatus = locker_status;
        this.unlockCode = unlock_code;
        this.validDate = validDate;
    }

    public Locker(string lockerName, bool lockerStatus, object value, string[] strings)
    {
        this.lockerName = lockerName;
        this.lockerStatus = lockerStatus;
    }

    public string id { get; set; } = null!;

    public string lockerName { get; set; } = null!;

    public bool? lockerStatus { get; set; }

    public string? unlockCode { get; set; }

    public DateTime? validDate { get; set; }

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
