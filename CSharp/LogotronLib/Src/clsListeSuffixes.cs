
using System;
using System.Collections.Generic;

namespace LogotronLib.Src
{
    public sealed class clsListeSuffixes
    {
        public static void LireSuffixesCode()
        {

            // Cette liste peut être récupérée via PrefixesSuffixes2.txt

            var suffixes = new List<string> {
            "able", "qui peut être", "D", "1", "Du latin -abilis (« capable de »), suffixe formateur de noms sur la base de verbes en -are.", "", "Latin", "Frequent",
            "acanthe", "épine", "L", "3", "Du grec ἄκανθος, akanthos (« épine »).", "acantho", "Grec", "Rare",
            "acousie", "audition", "L", "2", "Du grec ancien ἀκούω, akoúô (« entendre »).", "", "Grec", "Moyen",
            "adelphe", "frère", "L", "3", "Du grec ancien ἀδελφός, adelphós (« utérin, frère »).", "", "Grec", "Rare",
            "adelphie", "frère", "L", "3", "Du grec ancien ἀδελφός, adelphós (« utérin, frère »).", "", "Grec", "Absent",
            "agogie", "guidage", "L", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : guider", "Grec", "Moyen",
            "agogique", "guidage", "D", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : guider", "Grec", "Moyen",
            "agogue", "meneur", "L", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : guider", "Grec", "Moyen",
            "agogue", "conduction -> écoulement", "L", "2", "Du grec ancien ἀγωγή, agôgê (« action de mener ») ou ἀγωγός, agôgos (« qui conduit, qui guide ») dérivé de ἄγω, agô (« mener »).", "agogie : écouler", "Grec", "Moyen",
            "algésie", "douleur", "L", "2", "Du grec ancien ἄλγος algos (« douleur »).", "", "Grec", "Moyen"
            };
            clsGestBase.m_suffixes.DefinirSegments(suffixes, clsConst.iNbColonnes);
        }
    }
}
