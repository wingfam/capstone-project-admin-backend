using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class Bussiness
{
    public Bussiness()
    {
    }

    public Bussiness(string email, string password, string fullname, bool isAvaiable, string locationId)
    {
        this.email = email;
        this.password = password;
        this.fullname = fullname;
        this.isAvailable = isAvaiable;
        this.locationId = locationId;
    }
    public string id { get; set; } = null!;

    public string? bussinessName { get; set; }

    public string? address { get; set; }

    public string? phone { get; set; }
    public string? status { get; set; }

    [JsonIgnore]
    public virtual Location? Location { get; set; }
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingOrder> BookingOrders { get; set; } = new List<BookingOrder>();
}
