using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using plant_ecommerce_server.Models;

namespace plant_ecommerce_server.Data;

public partial class PlantEcommerceContext : DbContext
{
    public PlantEcommerceContext()
    {
    }

    public PlantEcommerceContext(DbContextOptions<PlantEcommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Plant> Plants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Mssql");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
