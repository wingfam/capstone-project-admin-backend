using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class BookingOrder
{
    public string id { get; set; } = null!;

    public DateTime? createDate { get; set; }

    public DateTime? ValidDate { get; set; }

    public bool? status { get; set; }

    public string? residentId { get; set; }

    public string? cabinetId { get; set; }

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
