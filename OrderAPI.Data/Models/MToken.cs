using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderAPI.Data.Models 
{
    public class MToken
    {
        [Key]
        public int Codigo { get; set; }

        public Guid Actor { get; set; }

        public string RefreshToken { get; set; }

    }
}
