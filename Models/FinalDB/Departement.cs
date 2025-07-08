using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("Departement", Schema = "dbo")]
    public partial class Departement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Departement_id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Departement_Name { get; set; }

        [ConcurrencyCheck]
        public string Manager { get; set; }

        public ICollection<Employe> Employes { get; set; }
    }
}