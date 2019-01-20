
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DicoLogotronMdb
{
    [Util.TableDescription("Table des mots")]
    [Table("Mot")] // Eviter le pluriel
    public class Mot
    {
        public Mot() { }

        [Util.Description("Identifiant du mot")]
        [Key]
        public int IdMot { get; set; }

        [Util.Description("Mot")]
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)] 
        [Column("Mot")]
        public string Mot_ { get; set; }

        // Soit on ne précise que ça, mais dans ce cas le nom du champ sera
        //  Segment_IdSegment : pas terrible
        //[Required]
        //public virtual Segment Segment { get; set; } // Segment_IdSegment
        // Soit on précise ForeignKey + un autre champ de même nom
        [ForeignKey("IdPrefixe")]
        public virtual Prefixe Prefixe { get; set; }
        [Util.Description("Identifiant du préfixe formant le mot")]
        public int IdPrefixe { get; set; }
        
        [ForeignKey("IdPrefixe2")]
        public virtual Prefixe Prefixe2 { get; set; }
        [Util.Description("Identifiant du préfixe n°2 formant le mot")]
        public int? IdPrefixe2 { get; set; }

        [ForeignKey("IdPrefixe3")]
        public virtual Prefixe Prefixe3 { get; set; }
        [Util.Description("Identifiant du préfixe n°3 formant le mot")]
        public int? IdPrefixe3 { get; set; }

        [ForeignKey("IdPrefixe4")]
        public virtual Prefixe Prefixe4 { get; set; }
        [Util.Description("Identifiant du préfixe n°4 formant le mot")]
        public int? IdPrefixe4 { get; set; }

        [ForeignKey("IdSuffixe")]
        public virtual Suffixe Suffixe { get; set; }
        [Util.Description("Identifiant du suffixe formant le mot")]
        public int IdSuffixe { get; set; }

        public override string ToString()
        {
            return string.Format("{2}: {0} - {1}", IdMot, Mot_, base.ToString());
        }

        public string ToJson()
        {
            string sFormat = "    {{\n" +
                "        \"IdMot\": {0},\n" +
                "        \"Mot\": \"{1}\",\n" +
                "        \"IdPrefixe\": {2}";
            string sVal = string.Format(sFormat, IdMot, Mot_, IdPrefixe);
            if (IdPrefixe2 != null) sVal += string.Format(
                ",\n        \"IdPrefixe2\": {0}", IdPrefixe2);
            if (IdPrefixe3 != null) sVal += string.Format(
                ",\n        \"IdPrefixe3\": {0}", IdPrefixe3);
            if (IdPrefixe4 != null) sVal += string.Format(
                ",\n        \"IdPrefixe4\": {0}", IdPrefixe4);
            sVal += string.Format(
                ",\n        \"IdSuffixe\": {0}", IdSuffixe);
            sVal += "\n" + "    }";
            return sVal;
        }

        //public string sCle()
        //{
        //    return Mot_;
        //}
        
        public string sClePrefixe() { return Prefixe.sCle(); }
        public string sClePrefixe2() { return Prefixe2.sCle(); }
        public string sClePrefixe3() { return Prefixe3.sCle(); }
        public string sClePrefixe4() { return Prefixe4.sCle(); }

        public string sCleSuffixe() { return Suffixe.sCle(); }

        public string ToJsonTxtId()
        {
            string sFormat = "    {{\n" +
                "        \"Mot\": \"{0}\",\n" +
                "        \"IdPrefixe\": \"{1}\"";
            string sVal = string.Format(sFormat, Mot_, sClePrefixe());
            if (IdPrefixe2 != null) sVal += string.Format(
                ",\n        \"IdPrefixe2\": \"{0}\"", sClePrefixe2());
            if (IdPrefixe3 != null) sVal += string.Format(
                ",\n        \"IdPrefixe3\": \"{0}\"", sClePrefixe3());
            if (IdPrefixe4 != null) sVal += string.Format(
                ",\n        \"IdPrefixe4\": \"{0}\"", sClePrefixe4());
            sVal += string.Format(
                ",\n        \"IdSuffixe\": \"{0}\"", sCleSuffixe());
            sVal += "\n" + "    }";
            return sVal;
        }
    }
}
