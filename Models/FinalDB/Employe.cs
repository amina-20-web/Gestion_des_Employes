using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DESKTOPJN6V6MTSQLEXPRESS.Models.FinalDB
{
    [Table("Employe", Schema = "dbo")]
    public partial class Employe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Employe_id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Employe_Prenom { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Employe_nom { get; set; }

        [ConcurrencyCheck]
        public decimal? EmployeSalery { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Sexe { get; set; }

        [ConcurrencyCheck]
        public string Email { get; set; }

        [ConcurrencyCheck]
        public string Telephone { get; set; }

        [ConcurrencyCheck]
        public DateTime? DateEmbauche { get; set; }

        [ConcurrencyCheck]
        public int? Manager_id { get; set; }

        public Employe Manager { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int Post_id { get; set; }

        public Poste Post { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int Departement_id { get; set; }

        public Departement Departement { get; set; }

        [ConcurrencyCheck]
        public DateTime? Date_naissance { get; set; }

        public ICollection<Absence> Absences { get; set; }

        public ICollection<Employe> InverseManager { get; set; }
    }
}