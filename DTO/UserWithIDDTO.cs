﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UserWithIDDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Password { get; set; } = null!;

        [EmailAddress(ErrorMessage = "invalid email")]
        public string? Email { get; set; }

        [NotMapped]
        public string Token { get; set; }
    }
}
