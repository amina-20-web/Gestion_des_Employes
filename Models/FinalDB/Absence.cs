using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("Absence", Schema = "dbo")]
    public partial class Absence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int absence_id { get; set; }

        [ConcurrencyCheck]
        public int? employe_id { get; set; }

        public Employe employe { get; set; }

        [ConcurrencyCheck]
        public DateTime? date_debut { get; set; }

        [ConcurrencyCheck]
        public DateTime? date_fin { get; set; }

        [ConcurrencyCheck]
        public string type_absence { get; set; }

        [ConcurrencyCheck]
        public string statut { get; set; }
    }
}