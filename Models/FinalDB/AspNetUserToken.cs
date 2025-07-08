using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("AspNetUserTokens", Schema = "dbo")]
    public partial class AspNetUserToken
    {
        [Key]
        [Required]
        public string UserId { get; set; }

        public AspNetUser User { get; set; }

        [Key]
        [Required]
        public string LoginProvider { get; set; }

        [Key]
        [Required]
        public string Name { get; set; }

        [ConcurrencyCheck]
        public string Value { get; set; }
    }
}