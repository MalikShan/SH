using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SH.Models
{
    public partial class Residents
    {
        public int Id { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Username { get; set; }
        
        public string Email { get; set; }
        public string Password { get; set; }       
        public string Permission { get; set; }
        public string Usertype { get; set; }
    }
}
