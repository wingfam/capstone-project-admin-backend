using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class Resident
{
    public Resident()
    {
    }

    public Resident(string residentId, string phone, string email, string password, string fullname, bool isAvaiable) {
        this.ResidentId = residentId;
        this.Phone = phone;
        this.Email = email;
        this.PasswordHash = password;
        this.Fullname = fullname;
        this.IsAvaiable = isAvaiable;
    }
    public string Id { get; set; } = null!;

    public string ResidentId { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Fullname { get; set; }

    public bool? IsAvaiable { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingOrder> BookingOrders { get; set; } = new List<BookingOrder>();
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<PackageInfo> PackageInfos { get; set; } = new List<PackageInfo>();
}
