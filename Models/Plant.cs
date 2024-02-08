using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace plant_ecommerce_server.Models;

[Table("plant")]
public partial class Plant
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name_th")]
    [StringLength(50)]
    [Unicode(false)]
    public string NameTh { get; set; } = null!;

    [Column("name_en")]
    [StringLength(50)]
    [Unicode(false)]
    public string NameEn { get; set; } = null!;
}
