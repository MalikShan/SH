using System;
using System.Collections.Generic;

namespace SH.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public DateTime? Request { get; set; }
        public string Permission { get; set; }
        public int? AppId { get; set; }
        public string Inspect { get; set; }
        public string Action { get; set; }
        public string Day { get; set; }
        public DateTime? Timespan { get; set; }
    }
}
