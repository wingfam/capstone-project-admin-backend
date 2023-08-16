﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class Box
{
    public Box()
    {
    }

    public Box(string name, int? status, string cabinetId)
    {
        this.nameBox = name;
        this.status = status;
        this.cabinetId = cabinetId;
    }

    public string? id { get; set; } = null!;
    public string? nameBox { get; set; }
    public int? status { get; set; }
    public string? cabinetId { get; set; }

    [JsonIgnore]
    public Cabinet? Cabinet { get; set; }
}
