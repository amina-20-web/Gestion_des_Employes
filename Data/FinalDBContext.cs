using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB;

namespace DESKTOPJN6V6MTSQLEXPRESS.Data
{
    public partial class FinalDBContext : DbContext
    {
        public FinalDBContext()
        {
        }

        public FinalDBContext(DbContextOptions<FinalDBContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin>().HasKey(table => new {
                table.LoginProvider, table.ProviderKey
            });

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole>().HasKey(table => new {
                table.UserId, table.RoleId
            });

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken>().HasKey(table => new {
                table.UserId, table.LoginProvider, table.Name
            });

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence>()
              .HasOne(i => i.employe)
              .WithMany(i => i.Absences)
              .HasForeignKey(i => i.employe_id)
              .HasPrincipalKey(i => i.Employe_id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe>()
              .HasOne(i => i.Departement)
              .WithMany(i => i.Employes)
              .HasForeignKey(i => i.Departement_id)
              .HasPrincipalKey(i => i.Departement_id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe>()
              .HasOne(i => i.Manager)
              .WithMany(i => i.InverseManager)
              .HasForeignKey(i => i.Manager_id)
              .HasPrincipalKey(i => i.Employe_id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe>()
              .HasOne(i => i.Post)
              .WithMany(i => i.Employes)
              .HasForeignKey(i => i.Post_id)
              .HasPrincipalKey(i => i.Post_id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim>()
              .HasOne(i => i.Role)
              .WithMany(i => i.AspNetRoleClaims)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim>()
              .HasOne(i => i.User)
              .WithMany(i => i.AspNetUserClaims)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin>()
              .HasOne(i => i.User)
              .WithMany(i => i.AspNetUserLogins)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole>()
              .HasOne(i => i.AspNetRole)
              .WithMany(i => i.AspNetUserRoles)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserRoles)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken>()
              .HasOne(i => i.User)
              .WithMany(i => i.AspNetUserTokens)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser>()
              .Property(p => p.LockoutEnd)
              .HasColumnType("datetimeoffset");
            this.OnModelBuilding(builder);
        }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Absence> Absences { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Departement> Departements { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Employe> Employes { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.Poste> Postes { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetRole> AspNetRoles { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserClaim> AspNetUserClaims { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserLogin> AspNetUserLogins { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserRole> AspNetUserRoles { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUser> AspNetUsers { get; set; }

        public DbSet<DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB.AspNetUserToken> AspNetUserTokens { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}