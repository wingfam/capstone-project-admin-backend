using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Models;

public partial class Box
{
    public Box()
    {
    }

    public Box(string name, string size, bool isStore, bool isAvaiable, string cabinetId)
    {
        this.name = name;
        this.size = size;
        this.isStore = isStore;
        this.isAvaiable = isAvaiable;
        this.cabinetId = cabinetId;
    }

    public string id { get; set; } = null!;

    public string name { get; set; } = null!;
    public string? size { get; set; }
    public bool? isStore { get; set; }
    public bool? isAvaiable { get; set; }
    public string? cabinetId { get; set; }

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
