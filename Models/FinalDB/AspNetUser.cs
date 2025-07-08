using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("AspNetUsers", Schema = "dbo")]
    public partial class AspNetUser
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int AccessFailedCount { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; }

        [ConcurrencyCheck]
        public string Email { get; set; }

        [ConcurrencyCheck]
        public bool EmailConfirmed { get; set; }

        [ConcurrencyCheck]
        public bool LockoutEnabled { get; set; }

        [ConcurrencyCheck]
        public DateTimeOffset? LockoutEnd { get; set; }

        [ConcurrencyCheck]
        public string NormalizedEmail { get; set; }

        [ConcurrencyCheck]
        public string NormalizedUserName { get; set; }

        [ConcurrencyCheck]
        public string PasswordHash { get; set; }

        [ConcurrencyCheck]
        public string PhoneNumber { get; set; }

        [ConcurrencyCheck]
        public bool PhoneNumberConfirmed { get; set; }

        [ConcurrencyCheck]
        public string SecurityStamp { get; set; }

        [ConcurrencyCheck]
        public bool TwoFactorEnabled { get; set; }

        [ConcurrencyCheck]
        public string UserName { get; set; }

        public ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }

        public ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }

        public ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

        public ICollection<AspNetUserToken> AspNetUserTokens { get; set; }
    }
}