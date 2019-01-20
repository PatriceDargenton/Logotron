
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DicoLogotronMdb
{
    [Util.TableDescription("Table des préfixes")]
    [Table("Prefixe")] // Eviter le pluriel
    public class Prefixe
    {
        public Prefixe() { }

        [Util.Description("Identifiant du préfixe")]
        [Key]
        public int IdPrefixe { get; set; }

        [Util.Description("Clé unique du préfixe")]
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string ClePrefixe { get; set; }

        [Util.Description("Préfixe")]
        [Required]
        [MaxLength(50)]
        [Index]
        [Column("Prefixe")]
        public string Prefixe_ { get; set; }

        // Soit on ne précise que ça, mais dans ce cas le nom du champ sera
        //  Segment_IdSegment : pas terrible
        //[Required]
        //public virtual Segment Segment { get; set; } // Segment_IdSegment
        // Soit on précise ForeignKey + un autre champ de même nom
        [ForeignKey("IdSegment")]
        public virtual Segment Segment { get; set; }
        [Util.Description("Identifiant du segment d'origine")]
        public int IdSegment { get; set; }

        [Util.Description("Booléen indiquant si le préfixe est compatible avec le Logotron (ou sinon si le préfixe sert pour l'analyse du dictionnaire)")]
        [Required]
        [Index]
        public bool bLogotron { get; set; }

        [Util.Description("Fréquence du préfixe parmi les mots 'Logotroniques'")]
        [Required]
        [MaxLength(50)]
        public string Frequence { get; set; }

        [Util.Description("Origine étymologique spécifique du préfixe, si elle est différente de celle du segment ou de la racine")]
        [Index]
        [MaxLength(50)]
        public string Origine { get; set; }

        [Util.Description("Etymologique spécifique du préfixe, si elle est différente de celle du segment ou de la racine")]
        public string Etymologie { get; set; }

        [Util.Description("Exemple(s) de mot(s) formé(s) avec ce préfixe")]
        [MaxLength(255)]
        public string Exemples { get; set; }

        [Util.Description("Liste exclusive de mots pouvant être formés avec ce préfixe")]
        public string ListeExclusiveMots { get; set; }

        [Util.Description("Unicité du préfixe (pour regrouper les préfixes relatifs à un même segment, si besoin est)")]
        [MaxLength(50)]
        public string Unicite { get; set; }

        // Cet index n'est pas unique, il doit être unique que pour le tirage
        //  pour éviter des confusions trop difficiles à départager
        [Util.Description("Unicité explicite du préfixe (pour regrouper les préfixes relatifs à un même segment)")]
        [Required]
        [Index]
        [MaxLength(50)]
        public string UniciteSynth { get; set; }

        public override string ToString()
        {
            return string.Format("{2}: {0} - {1}", IdPrefixe, Prefixe_, base.ToString());
        }

        public string ToJson()
        {
            string sFormat = "    {{\n" +
                "        \"IdPrefixe\": {0},\n" +
                "        \"Prefixe\": \"{1}\",\n" +
                "        \"IdSegment\": {2},\n" +
                "        \"bLogotron\": {3},\n" +
                "        \"Frequence\": \"{4}\"";
            
            string sVal = string.Format(sFormat, IdPrefixe, Prefixe_, Segment.IdSegment,
                (bLogotron ? "true" : "false"), Frequence);

            if (!string.IsNullOrEmpty(Origine))
                sVal += string.Format(
                ",\n        \"Origine\": \"{0}\"", Origine);
            if (!string.IsNullOrEmpty(Etymologie))
                sVal += string.Format(
                ",\n        \"Etymologie\": \"{0}\"", Etymologie);
            if (!string.IsNullOrEmpty(Exemples))
                sVal += string.Format(
                ",\n        \"Exemples\": \"{0}\"", Exemples);
            if (!string.IsNullOrEmpty(ListeExclusiveMots))
                sVal += string.Format(
                ",\n        \"ListeExclusiveMots\": \"{0}\"", ListeExclusiveMots);
            sVal += "\n" + "    }";
            return sVal;
        }
        
        public string sCle()
        {
            return ClePrefixe;
        }

        public string sCleSegment()
        {
            return Segment.sCle();
        }

        public string ToJsonTxtId()
        {
            string sFormat = "    {{\n" +
                "        \"IdPrefixe\": \"{0}\",\n" +
                "        \"Prefixe\": \"{1}\",\n" +
                "        \"IdSegment\": \"{2}\",\n" +
                "        \"bLogotron\": {3},\n" +
                "        \"Frequence\": \"{4}\"";
            string sVal = string.Format(sFormat, sCle(), Prefixe_, sCleSegment(),
                (bLogotron ? "true" : "false"), Frequence);
            if (!string.IsNullOrEmpty(Origine))
                sVal += string.Format(
                ",\n        \"Origine\": \"{0}\"", Origine);
            if (!string.IsNullOrEmpty(Etymologie))
                sVal += string.Format(
                ",\n        \"Etymologie\": \"{0}\"", Etymologie);
            if (!string.IsNullOrEmpty(Exemples))
                sVal += string.Format(
                ",\n        \"Exemples\": \"{0}\"", Exemples);
            if (!string.IsNullOrEmpty(ListeExclusiveMots))
                sVal += string.Format(
                ",\n        \"ListeExclusiveMots\": \"{0}\"", ListeExclusiveMots);
            sVal += "\n" + "    }";
            return sVal;
        }
    }
}
