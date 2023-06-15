using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class BookingOrder
{
    public string Id { get; set; } = null!;

    public string BookingOrderId { get; set; } = null!;

    public DateTime? BookingDate { get; set; }

    public DateTime? BookingValidDate { get; set; }

    public bool? BookingStatus { get; set; }

    public string? ResidentId { get; set; }

    public string? LockerId { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingCode> BookingCodes { get; set; } = new List<BookingCode>();
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();
    [JsonIgnore]
    public virtual Locker? Locker { get; set; }
    [JsonIgnore]
    public virtual Resident? Resident { get; set; }
}
