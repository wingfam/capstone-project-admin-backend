using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class BookingCode
{
    public string id { get; set; } = null!;

    public string bookingCodeId { get; set; } = null!;

    public string bcodeName { get; set; } = null!;

    public DateTime? bcodeValidDate { get; set; }

    public string? bookingId { get; set; }

    [JsonIgnore]
    public virtual BookingOrder? Booking { get; set; }
}
