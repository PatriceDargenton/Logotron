
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DicoLogotronMdb
{
    [Util.TableDescription("Table des concepts")]
    [Table("Concept")] // Eviter le pluriel
    public class Concept //: BaseEntity
    {
        // Collections
        public virtual ICollection<Racine> lstRacines { get; set; }

        [NotMapped]
        public HashSet<string> hsRacines { get; set; }

        public Concept() {
            lstRacines = new List<Racine>();
            hsRacines = new HashSet<string>();
        }

        [Util.Description("Identifiant du concept")]
        [Key]
        public int IdConcept { get; set; }

        [Util.Description("Sens du concept")]
        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        [Column("Concept")]
        public string Concept_ { get; set; }

        [Util.Description("Liste des racines associées")]
        [Required]
        [MaxLength(clsConstMdb.iMaxCar255)]
        public string Racines { get; set; }

        [Util.Description("Nombre de racines associées")]
        [Required]
        public int NbRacines { get; set; }

        public override string ToString()
        {
            return string.Format("{2}: {0} - {1}", IdConcept, Concept_, base.ToString());
        }
        
        public string ToJson()
        {
            string sFormat = "    {{\n" +
                "        \"IdConcept\": {0},\n" +
                "        \"Concept\": \"{1}\",\n" +
                "        \"Racines\": \"{2}\",\n" +
                "        \"NbRacines\": {3}\n" +
                "    }}";
            return string.Format(sFormat, IdConcept, Concept_, Racines, NbRacines);
        }
        
        public string sCle()
        {
            return Concept_;
        }
        
        public string ToJsonTxtId()
        {
            string sFormat = "    {{\n" +
                "        \"IdConcept\": \"{0}\",\n" +
                "        \"Concept\": \"{1}\",\n" +
                "        \"Racines\": \"{2}\",\n" +
                "        \"NbRacines\": {3}\n" +
                "    }}";
            return string.Format(sFormat, sCle(), Concept_, Racines, NbRacines);
        }
    }
}
