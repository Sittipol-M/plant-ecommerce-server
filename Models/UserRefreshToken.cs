using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace plant_ecommerce_server.Models;

[PrimaryKey("UserId", "RefreshToken")]
[Table("user_refresh_token")]
public partial class UserRefreshToken
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Key]
    [Column("refresh_token")]
    [StringLength(300)]
    [Unicode(false)]
    public string RefreshToken { get; set; } = null!;

    [Column("expired_at", TypeName = "datetime")]
    public DateTime? ExpiredAt { get; set; }
}
