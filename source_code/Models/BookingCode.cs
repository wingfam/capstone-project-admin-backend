using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class BookingCode
{
    public string Id { get; set; } = null!;

    public string BookingCodeId { get; set; } = null!;

    public string BcodeName { get; set; } = null!;

    public DateTime? BcodeValidDate { get; set; }

    public string? BookingId { get; set; }

    [JsonIgnore]
    public virtual BookingOrder? Booking { get; set; }
}
