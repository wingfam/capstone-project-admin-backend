using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MailBoxTest.Models;

public partial class AccessWarning
{
    public string Id { get; set; } = null!;

    public string AccessWarningId { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? LockerId { get; set; }
    public string? Status { get; set; }

    [JsonIgnore]
    public virtual Locker? Locker { get; set; }
}
