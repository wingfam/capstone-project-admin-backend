using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class BookingHistory
{
    public string Id { get; set; } = null!;

    public string BookingHistoryId { get; set; } = null!;

    public string BcodeName { get; set; } = null!;

    public string LockerName { get; set; } = null!;

    public string? UnlockCode { get; set; }

    public string? BookingId { get; set; }

    [JsonIgnore]
    public virtual BookingOrder? Booking { get; set; }
}
