using System;
using System.Collections.Generic;

namespace SH.Models
{
    public partial class Appliances
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Voltages { get; set; }
        public int? RoomId { get; set; }
    }
}
