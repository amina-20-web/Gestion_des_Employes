using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("AspNetRoleClaims", Schema = "dbo")]
    public partial class AspNetRoleClaim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ConcurrencyCheck]
        public string ClaimType { get; set; }

        [ConcurrencyCheck]
        public string ClaimValue { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string RoleId { get; set; }

        public AspNetRole Role { get; set; }
    }
}