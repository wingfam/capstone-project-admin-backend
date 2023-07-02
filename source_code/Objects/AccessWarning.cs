using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Models;

public partial class AccessWarning
{
    public AccessWarning()
    {
    }

    public AccessWarning(string message, string lockerId, bool status, DateTime createdDate)
    {
        this.message = message;
        this.lockerId = lockerId;
        this.status = status;
        this.createDate = createdDate;
    }

    public string id { get; set; } = null!;

    public string? message { get; set; }

    public DateTime? createDate { get; set; }

    public string? lockerId { get; set; }
    public bool status { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public virtual Box? Locker { get; set; }
}
