using System;
using System.Collections.Generic;

namespace SH.Models
{
    public partial class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NoOfAppliances { get; set; }
        public string UserId { get; set; }
    }
}
