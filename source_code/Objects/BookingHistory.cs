using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class BookingHistory
{
    public string id { get; set; } = null!;

    public string? bookingId { get; set; }
    public string? residentId { get; set; }

    [JsonIgnore]
    public virtual BookingOrder? BookingOrder { get; set; }
    [JsonIgnore]
    public virtual Bussiness? Resident { get; set; }
}
