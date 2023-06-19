 using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class PackageInfo
{
    public string id { get; set; } = null!;

    public string packageInfoId { get; set; } = null!;

    public DateTime? deliveryDate { get; set; }

    public DateTime? pickupDate { get; set; }

    public string? residentId { get; set; }

    public string? lockerId { get; set; }

    [JsonIgnore]
    public virtual Locker? Locker { get; set; }
    [JsonIgnore]
    public virtual Resident? Resident { get; set; }
}
