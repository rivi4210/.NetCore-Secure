using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Password { get; set; }

    public string? Salt { get; set; }

    public string? Email { get; set; }

    [NotMapped]
    public string Token { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
