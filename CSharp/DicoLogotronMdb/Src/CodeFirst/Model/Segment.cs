
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DicoLogotronMdb
{
    [Util.TableDescription("Table des segments")]
    [Table("Segment")] // Eviter le pluriel
    public class Segment
    {
        public virtual ICollection<Prefixe> lstPrefixes { get; set; }
        public virtual ICollection<Suffixe> lstSuffixes { get; set; }
        
        [NotMapped]
        public HashSet<string> hsVariantes { get; set; }

        [NotMapped]
        public HashSet<string> hsSens { get; set; }

        public Segment() {
            lstPrefixes = new List<Prefixe>();
            lstSuffixes = new List<Suffixe>();
            hsVariantes = new HashSet<string>();
            hsSens = new HashSet<string>();
        }

        [Util.Description("Identifiant du segment")]
        [Key]
        public int IdSegment { get; set; }

        [Util.Description("Clé unique du segment")]
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string CleSegment { get; set; }

        // Exemple : carcino-, et CleSegment : 1:carcino : cancer, 1:carcino : crustacé
        [Util.Description("Segment")]
        [Required]
        [MaxLength(50)]
        [Column("Segment")]
        public string Segment_ { get; set; }

        [Util.Description("Variantes du segment")]
        [Required]
        [MaxLength(clsConstMdb.iMaxCar255)]
        public string Variantes { get; set; }

        // Soit on ne précise que ça, mais dans ce cas le nom du champ sera
        //  Racine_IdRacine : pas terrible
        //[Required]
        //public virtual Racine Racine { get; set; } // Racine_IdRacine
        // Soit on précise ForeignKey + un autre champ de même nom
        [ForeignKey("IdRacine")]
        public virtual Racine Racine { get; set; }
        [Util.Description("Identifiant de la racine d'origine")]
        public int IdRacine { get; set; }

        [Util.Description("Sens principal du segment")]
        [Required]
        [MaxLength(100)]
        public string SensPrincipal { get; set; } 

        [Util.Description("Sens spécifique du segment, s'il est nuancé par rapport au sens de la racine ou du concept")]
        [MaxLength(clsConstMdb.iMaxCar255)]
        public string Sens { get; set; } // Liste des sens

        [Util.Description("Nombre de sens répertoriés")]
        [Required]
        public int NbSens { get; set; }

        [Util.Description("Booléen indiquant si le segment correspond à des préfixes (ou sinon des suffixes)")]
        [Required]
        [Index]
        public bool bPrefixe { get; set; }

        [Util.Description("Origine étymologique spécifique du segment, si elle est différente de celle de la racine")]
        [Index]
        [MaxLength(50)]
        public string Origine { get; set; }

        [Util.Description("Etymologique spécifique du segment, si elle est différente de celle de la racine")]
        public string Etymologie { get; set; }

        [Util.Description("Unicité du segment (pour distinguer les préfixes ou suffixes, si besoin est)")]
        [MaxLength(50)]
        public string Unicite { get; set; }

        // Cet index n'est pas unique, il doit être unique que pour le tirage
        //  pour éviter des confusions trop difficiles à départager
        [Util.Description("Unicité explicite du segment (pour distinguer les préfixes ou suffixes)")]
        [Required]
        [Index]
        [MaxLength(50)]
        public string UniciteSynth { get; set; }

        public override string ToString()
        {
            return string.Format("{2}: {0} - {1}", IdSegment, Segment_, base.ToString());
        }

        public string ToJson()
        {
            string sFormat = "    {{\n" +
                "        \"IdSegment\": {0},\n" +
                "        \"Segment\": \"{1}\",\n" +
                "        \"Variantes\": \"{2}\",\n" +
                "        \"IdRacine\": {3},\n" +
                "        \"bPrefixe\": {4},\n" +
                "        \"SensPrincipal\": \"{5}\"";
            string sVal = string.Format(sFormat, IdSegment, Segment_, Variantes, 
                Racine.IdRacine, (bPrefixe ? "true" : "false"), SensPrincipal); 
            if (!string.IsNullOrEmpty(Origine))
                sVal += string.Format(
                ",\n        \"Origine\": \"{0}\"", Origine);
            if (!string.IsNullOrEmpty(Etymologie))
                sVal += string.Format(
                ",\n        \"Etymologie\": \"{0}\"", Etymologie);
            if (!string.IsNullOrEmpty(Sens))
                sVal += string.Format(
                ",\n        \"Sens\": \"{0}\"", Sens);
            if (NbSens>1)
                sVal += string.Format(
                ",\n        \"NbSens\": {0}", NbSens);
            sVal += "\n" + "    }";
            return sVal;
        }
        
        public string sCle()
        {
            return CleSegment;
        }

        public string sCleRacine()
        {
            return Racine.sCle();
        }

        public string ToJsonTxtId()
        {
            string sFormat = "    {{\n" +
                "        \"IdSegment\": \"{0}\",\n" +
                "        \"Segment\": \"{1}\",\n" +
                "        \"Variantes\": \"{2}\",\n" +
                "        \"IdRacine\": \"{3}\",\n" +
                "        \"bPrefixe\": {4},\n" +
                "        \"SensPrincipal\": \"{5}\"";
            string sVal = string.Format(sFormat, sCle(), Segment_, Variantes, sCleRacine(),
                (bPrefixe ? "true" : "false"), SensPrincipal);
            if (!string.IsNullOrEmpty(Origine))
                sVal += string.Format(
                ",\n        \"Origine\": \"{0}\"", Origine);
            if (!string.IsNullOrEmpty(Etymologie))
                sVal += string.Format(
                ",\n        \"Etymologie\": \"{0}\"", Etymologie);
            if (!string.IsNullOrEmpty(Sens))
                sVal += string.Format(
                ",\n        \"Sens\": \"{0}\"", Sens);
            if (NbSens > 1)
                sVal += string.Format(
                ",\n        \"NbSens\": {0}", NbSens);
            sVal += "\n" + "    }";
            return sVal;
        }
    }
}
