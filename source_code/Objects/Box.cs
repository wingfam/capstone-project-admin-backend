using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DeliverBox_BE.Objects;

public partial class Box
{
    public Box()
    {
    }

    public Box(string name, bool isStore, bool isAvailable, string cabinetId)
    {
        this.nameBox = name;
        this.isStore = isStore;
        this.isAvailable = isAvailable;
        this.cabinetId = cabinetId;
    }

    public string id { get; set; } = null!;
    public string nameBox { get; set; }
    public bool? isStore { get; set; }
    public bool? isAvailable { get; set; }
    public string? cabinetId { get; set; }

    [JsonIgnore]
    public Cabinet? Cabinet { get; set; }
}
