using System;
using System.Collections.Generic;
using System.Text;
using backend_wonderservice.DATA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WonderService.Data.Services
{
    public partial class DataContext:IdentityDbContext<User>

    {
     public virtual DbSet<Photo> Photo { get; set; }
     public virtual DbSet<backend_wonderservice.DATA.Models.Services> Services { get; set; }
     public virtual DbSet<Customer> Customer { get; set; }
     public virtual DbSet<ServicesTypes> ServicesTypes { get; set; }
     public virtual DbSet<States> States { get; set; }
     public virtual DbSet<LocalGovernment> LocalGovernment { get; set; }
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.ServicesTypeId);

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AppointmentDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.AppointmentDateEnd).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.EntryDate)
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LocalGovernmentId).HasColumnName("local_government_Id");

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.HasOne(d => d.LocalGovernment)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.LocalGovernmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
                    

                entity.HasOne(d => d.ServicesType)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.ServicesTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.StateId);

            });

            modelBuilder.Entity<LocalGovernment>(entity =>
            {
                entity.ToTable("local_government");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.LocalGovernment)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_local_government_states");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasIndex(e => e.ServicesId);

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Services)
                    .WithMany(p => p.Photo)
                    .HasForeignKey(d => d.ServicesId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Photo)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<backend_wonderservice.DATA.Models.Services>(entity =>
            {
                entity.HasIndex(e => e.ServiceTypeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ServiceType)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.ServiceTypeId);
            });

            modelBuilder.Entity<ServicesTypes>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.ToTable("states");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
