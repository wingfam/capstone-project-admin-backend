using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class Business
{
    public Business()
    {
    }

    public Business(string name, string address, string phone, int status)
    {
        this.businessName = name;
        this.address = address;
        this.phone = phone;
        this.status = status;
    }
    public string id { get; set; } = null!;
    public string? businessName { get; set; }
    public string? address { get; set; }
    public string? phone { get; set; }
    public int? status { get; set; }

}
