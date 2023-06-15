using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class PackageInfo
{
    public string Id { get; set; } = null!;

    public string PackageInfoId { get; set; } = null!;

    public DateTime? DeliveryDate { get; set; }

    public DateTime? PickupDate { get; set; }

    public string? ResidentId { get; set; }

    public string? LockerId { get; set; }

    [JsonIgnore]
    public virtual Locker? Locker { get; set; }
    [JsonIgnore]
    public virtual Resident? Resident { get; set; }
}
