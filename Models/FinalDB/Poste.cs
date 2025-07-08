using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("Postes", Schema = "dbo")]
    public partial class Poste
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Post_id { get; set; }

        [ConcurrencyCheck]
        public string Post_name { get; set; }

        [ConcurrencyCheck]
        public decimal? Post_Salery { get; set; }

        public ICollection<Employe> Employes { get; set; }
    }
}