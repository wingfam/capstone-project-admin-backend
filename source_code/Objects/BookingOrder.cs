using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Models;

public partial class BookingOrder
{
    public string id { get; set; } = null!;

    public string bookingOrderId { get; set; } = null!;

    public DateTime? bookingDate { get; set; }

    public DateTime? bookingValidDate { get; set; }

    public bool? bookingStatus { get; set; }

    public string? residentId { get; set; }

    public string? lockerId { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingCode> BookingCodes { get; set; } = new List<BookingCode>();
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual ICollection<BookingHistory> BookingHistories { get; set; } = new List<BookingHistory>();
    [JsonIgnore]
    public virtual Box? Locker { get; set; }
    [JsonIgnore]
    public virtual Resident? Resident { get; set; }
}
