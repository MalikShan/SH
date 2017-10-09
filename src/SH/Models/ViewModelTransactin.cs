using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SH
{
    public class ViewModelTransactin
    {
        public int appId { get; set; }
        public string appName { get; set; }
        public string Status { get; set; }
        public string Voltages { get; set; }
        public int? RoomId { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; }


        public int roomId { get; set; }
        public string roomName { get; set; }

        public int tId { get; set; }
        public DateTime? Request { get; set; }
        public string Permission { get; set; }
        public int? AppId { get; set; }
        public string Action { get; set; }
        public int Timespan { get; set; }

    }
}
