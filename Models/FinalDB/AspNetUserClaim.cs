using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("AspNetUserClaims", Schema = "dbo")]
    public partial class AspNetUserClaim
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
        public string UserId { get; set; }

        public AspNetUser User { get; set; }
    }
}