using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class BookingHistory
{
    public string id { get; set; } = null!;

    public string bookingHistoryId { get; set; } = null!;

    public string bcodeName { get; set; } = null!;

    public string lockerName { get; set; } = null!;

    public string? unlockCode { get; set; }

    public string? bookingId { get; set; }
    public string? residentId { get; set; }

    [JsonIgnore]
    public virtual BookingOrder? Booking { get; set; }
}
