using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class Resident
{
    public Resident()
    {
    }

    public Resident(string phone, string email, string password, string fullname, bool isAvaiable, string locationId)
    {
        this.phone = phone;
        this.email = email;
        this.password = password;
        this.fullname = fullname;
        this.isAvaiable = isAvaiable;
        this.locationId = locationId;
    }
    public string id { get; set; } = null!;

    public string phone { get; set; } = null!;

    public string? email { get; set; }

    public string? password { get; set; }

    public string? fullname { get; set; }
    public string? locationId { get; set; }
    public bool? isAvaiable { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingOrder> BookingOrders { get; set; } = new List<BookingOrder>();
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<PackageInfo> PackageInfos { get; set; } = new List<PackageInfo>();
}
