using System;
using System.Collections.Generic;

namespace DatabaseLibrary.Models;

public partial class PickupPoint
{
    public int PickupPointId { get; set; }
    public int Index { get; set; }
    public string City { get; set; } = null!;
    public string Street { get; set; } = null!;
    public byte HomeNumber { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public string FullAddress => $"{City}, {Street}, {HomeNumber} (индекс: {Index})";
}