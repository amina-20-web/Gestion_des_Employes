using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("AspNetUserLogins", Schema = "dbo")]
    public partial class AspNetUserLogin
    {
        [Key]
        [Required]
        public string LoginProvider { get; set; }

        [Key]
        [Required]
        public string ProviderKey { get; set; }

        [ConcurrencyCheck]
        public string ProviderDisplayName { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string UserId { get; set; }

        public AspNetUser User { get; set; }
    }
}