
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DicoLogotronMdb
{
    [Util.TableDescription("Table des racines")]
    [Table("Racine")] // Eviter le pluriel
    public class Racine
    {
        // Collections
        public virtual ICollection<Segment> lstSegments { get; set; }
        
        [NotMapped]
        public HashSet<string> hsSegments { get; set; }

        public Racine() {
            lstSegments = new List<Segment>();
            hsSegments = new HashSet<string>();
        }

        [Util.Description("Identifiant de la racine")]
        [Key]
        public int IdRacine { get; set; }

        [Util.Description("Clé unique de la racine")]
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string CleRacine { get; set; }

        [Util.Description("Racine principale (représentative)")]
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        [Column("Racine")]
        public string RacinePrincipale { get; set; }

        [Util.Description("Sens spécifique de la racine, s'il est nuancé par rapport au sens du concept")]
        [MaxLength(100)]
        [Index]
        public string Sens { get; set; }

        [Util.Description("Liste des segments associés")]
        [Required]
        [MaxLength(clsConstMdb.iMaxCar255)]
        public string Segments { get; set; }

        // Soit on ne précise que ça, mais dans ce cas le nom du champ sera
        //  Concept_IdConcept : pas terrible
        //[Required]
        //public virtual Concept Concept { get; set; } // Concept_IdConcept
        // Soit on précise ForeignKey + un autre champ de même nom
        [ForeignKey("IdConcept")]
        public virtual Concept Concept { get; set; }
        [Util.Description("Identifiant du concept d'origine")]
        public int IdConcept { get; set; }

        [Util.Description("Niveau de difficulté de la racine")]
        [Required]
        [Index]
        public short Niveau { get; set; }

        [Util.Description("Origine étymologique de la racine")]
        [MaxLength(50)]
        [Index]
        public string Origine { get; set; }

        [Util.Description("Etymologie de la racine")]
        public string Etymologie { get; set; }
        
        // Etymologie si le segment correspond à l'unicité
        [NotMapped]
        public string EtymologieUnicite { get; set; }
       
        public override string ToString()
        {
            return string.Format("{2}: {0} - {1}", IdRacine, CleRacine, base.ToString());
        }
        
        public string ToJson()
        {
            string sFormat = "    {{\n" +
                "        \"IdRacine\": {0},\n" +
                "        \"Racine\": \"{1}\",\n" +
                "        \"IdConcept\": {2},\n" +
                "        \"Segments\": \"{3}\",\n" +
                "        \"Niveau\": {4}" ;
            string sVal = string.Format(sFormat, 
                IdRacine, RacinePrincipale, Concept.IdConcept, Segments, Niveau); 
            if (!string.IsNullOrEmpty(Sens))
                sVal += string.Format(
                ",\n        \"Sens\": \"{0}\"", Sens);
            if (!string.IsNullOrEmpty(Origine))
                sVal += string.Format(
                ",\n        \"Origine\": \"{0}\"", Origine);
            if (!string.IsNullOrEmpty(Etymologie))
                sVal += string.Format(
                ",\n        \"Etymologie\": \"{0}\"", Etymologie);
            sVal += "\n" + "    }";
            return sVal;
        }

        public string sCle()
        {
            return CleRacine;
        }
        
        public string sCleConcept()
        {
            return Concept.sCle(); 
        }

        public string ToJsonTxtId()
        {
            string sFormat = "    {{\n" +
                "        \"IdRacine\": \"{0}\",\n" +
                "        \"Racine\": \"{1}\",\n" +
                "        \"IdConcept\": \"{2}\",\n" +
                "        \"Segments\": \"{3}\",\n" +
                "        \"Niveau\": {4}";
            string sVal = string.Format(sFormat, sCle(), RacinePrincipale,
                sCleConcept(), Segments, Niveau); 
            if (!string.IsNullOrEmpty(Sens))
                sVal += string.Format(
                ",\n        \"Sens\": \"{0}\"", Sens);
            if (!string.IsNullOrEmpty(Origine))
                sVal += string.Format(
                ",\n        \"Origine\": \"{0}\"", Origine);
            if (!string.IsNullOrEmpty(Etymologie))
                sVal += string.Format(
                ",\n        \"Etymologie\": \"{0}\"", Etymologie);
            sVal += "\n" + "    }";
            return sVal;
        }
    }
}
