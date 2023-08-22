using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class BookingOrder
{
    public string id { get; set; } = null!;
    public DateTime createDate { get; set; }
    public DateTime? validDate { get; set; }
    public int? status { get; set; }
    public string? businessId { get; set; }
    public string? boxId { get; set; }
    public string? customerId { get; set; }
    public string? unlockCode { get; set; }

    [JsonIgnore]
    public virtual Business? Business { get; set; }
    [JsonIgnore]
    public virtual Box? Box { get; set; }
}
