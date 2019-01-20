
using System;
using System.Collections.Generic;

namespace LogotronLib.Src
{
    public sealed class clsListePrefixes
    {

        public static void LirePrefixesCodeEn()
        {
            var prefixes = new List<string> {
                "bio", "life", "L", "1", "From Ancient Greek βίος (bíos, “bio-, life”).", "", "Gréco-latin", "Rare",
                "tele", "afar", "L", "1", "From Ancient Greek τῆλε (têle, “afar”).", "", "Gréco-latin", "Rare"
            };
            clsGestBase.m_prefixes.DefinirSegments(prefixes, clsConst.iNbColonnes);
        }

        public static void LirePrefixesCode()
        {

            // Cette liste peut être récupérée via PrefixesSuffixes2.txt

            var prefixes = new List<string> {
            "a", "sans", "D", "1", "Du grec ancien ἀ-, a- exprimant la privation.", "a", "Grec", "Frequent",
            "acanth", "l'épine", "D", "3", "Du grec ἄκανθος, akanthos (« épine »).", "acantho", "Grec", "Rare",
            "acantho", "l'épine", "L", "3", "Du grec ἄκανθος, akanthos (« épine »).", "acantho", "Grec", "Moyen",
            "acou", "l'audition", "L", "2", "Du grec ancien ἀκούω, akoúô (« entendre »).", "", "Grec", "Moyen",
            "acro", "élevé(e) / extrême", "L", "3", "Du grec ancien ἄκρος, acros (« élevé, extrême »).", "", "Grec", "Frequent",
            "actin", "le rayon", "D", "3", "Du grec ancien ἀκτίς, ἀκτίνος, actis, actinos (« rayon »).", "", "Grec", "Moyen",
            "actini", "le rayon", "D", "3", "Du grec ancien ἀκτίς, ἀκτίνος, actis, actinos (« rayon »).", "", "Grec", "Rare",
            "actino", "le rayon", "L", "3", "Du grec ancien ἀκτίς, ἀκτίνος, actis, actinos (« rayon »).", "", "Grec", "Moyen",
            "acu", "l'aiguille", "L", "2", "Du latin acus (« aiguille »).", "", "Latin", "Absent",
            "addicto", "l'addiction", "L", "1", "De l'anglais addict.", "", "Anglais", "Rare"
            };
            clsGestBase.m_prefixes.DefinirSegments(prefixes, clsConst.iNbColonnes);
        }
    }
}
