using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class AccessWarning
{
    public string id { get; set; } = null!;

    public string accessWarningId { get; set; } = null!;

    public string? message { get; set; }

    public DateTime? createDate { get; set; }

    public string? lockerId { get; set; }
    public string? status { get; set; }

    [JsonIgnore]
    public virtual Locker? Locker { get; set; }
}
