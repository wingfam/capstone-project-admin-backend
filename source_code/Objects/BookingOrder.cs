using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class BookingOrder
{
    public string id { get; set; } = null!;

    public DateTime? createDate { get; set; }

    public DateTime? validDate { get; set; }

    public string? status { get; set; }
    //InProccess
    //Done
    //Storing

    public string? residentId { get; set; }

    public string? boxId { get; set; }

    [JsonIgnore]
    public virtual Bussiness? Resident { get; set; }
    [JsonIgnore]
    public virtual Box? Box { get; set; }
}
