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

    public Box(string name, string size, bool isStore, bool isAvaiable, string cabinetId)
    {
        this.name = name;
        this.size = size;
        this.isStore = isStore;
        this.isAvaiable = isAvaiable;
        this.cabinetId = cabinetId;
    }

    public string id { get; set; } = null!;

    public string name { get; set; } = null!;
    public string? size { get; set; }
    public bool? isStore { get; set; }
    public bool? isAvaiable { get; set; }
    public string? cabinetId { get; set; }

    [JsonIgnore]
    public Cabinet? Cabinet { get; set; }
}
