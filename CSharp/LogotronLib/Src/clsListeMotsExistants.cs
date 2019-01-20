
using System.Collections.Generic;

namespace LogotronLib.Src
{
    public sealed class clsListeMotsExistants
    {

        public static void ChargerMotsExistantsCodeEn(
            Dictionary<string, clsMotExistant> dicoMotsExistants)
        {
            // mot, DÉFINITION, préfixe, suffixe, niveau préfixe, niveau suffixe, unicité préfixe, unicité suffixe, fréq. préfixe, fréq. suffixe
            List<string> lstMots = new List<string>() {
                "biology", "STUDY  LIFE", "bio", "logy", "1", "1", "", "", "Rare", "Rare",
                "bioscope", "LOOK AT  LIFE", "bio", "scope", "1", "1", "", "", "Rare", "Rare",
                "telephone", "VOICE  AFAR", "tele", "phone", "1", "1", "", "", "Rare", "Rare",
                "telescope", "LOOK AT  AFAR", "tele", "scope", "1", "1", "", "", "Rare", "Rare"
            };
            clsMotExistant.InitMots(lstMots, dicoMotsExistants);
        }

        // Cette liste peut être récupérée via DicoLogotron\Doc\MotsSimplesCode_fr.txt

#if DEBUG

        public static void ChargerMotsExistantsCode(
            Dictionary<string, clsMotExistant> dicoMotsExistants)
        {
            // mot, DÉFINITION, préfixe, suffixe, niveau préfixe, niveau suffixe, unicité préfixe, unicité suffixe, fréq. préfixe, fréq. suffixe
            List<string> lstMots = new List<string>() {
                "abiotique", "VIE  SANS", "a", "biotique", "1", "1", "a", "bio", "Frequent", "Moyen",
                "acalorique", "CHALEUR  SANS", "a", "calorique", "1", "2", "a", "calori", "Frequent", "Moyen",
                "acanthocéphale", "TÊTE  DE L'ÉPINE", "acantho", "céphale", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthocyte", "CELLULE  DE L'ÉPINE", "acantho", "cyte", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthodactyle", "DOIGT  DE L'ÉPINE", "acantho", "dactyle", "3", "2", "acantho", "dactylo", "Moyen", "Frequent",
                "acanthoglosse", "LANGUE  DE L'ÉPINE", "acantho", "glosse", "3", "3", "acantho", "glosso", "Moyen", "Moyen",
                "acantholyse", "DÉCOMPOSITION  DE L'ÉPINE", "acantho", "lyse", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthomètre", "MESUREUR  DE L'ÉPINE", "acantho", "mètre", "3", "1", "acantho", "métro (métron)", "Moyen", "Frequent",
                "acanthozoïde", "ANIMAL  DE L'ÉPINE", "acantho", "zoïde", "3", "1", "acantho", "", "Moyen", "Moyen",
                "acarpe", "POIGNET  SANS", "a", "carpe", "1", "3", "a", "", "Frequent", "Moyen"
            };

            clsMotExistant.InitMots(lstMots, dicoMotsExistants);
        }

#else

        public static void ChargerMotsExistantsCode(
            Dictionary<string, clsMotExistant> dicoMotsExistants)
        {
            List<string> lstMots = new List<string>() {
                "abiotique", "VIE  SANS", "a", "biotique", "1", "1", "a", "bio", "Frequent", "Moyen",
                "acalorique", "CHALEUR  SANS", "a", "calorique", "1", "2", "a", "calori", "Frequent", "Moyen",
                "acanthocéphale", "TÊTE  DE L'ÉPINE", "acantho", "céphale", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthocyte", "CELLULE  DE L'ÉPINE", "acantho", "cyte", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthodactyle", "DOIGT  DE L'ÉPINE", "acantho", "dactyle", "3", "2", "acantho", "dactylo", "Moyen", "Frequent",
                "acanthoglosse", "LANGUE  DE L'ÉPINE", "acantho", "glosse", "3", "3", "acantho", "glosso", "Moyen", "Moyen",
                "acantholyse", "DÉCOMPOSITION  DE L'ÉPINE", "acantho", "lyse", "3", "2", "acantho", "", "Moyen", "Frequent",
                "acanthomètre", "MESUREUR  DE L'ÉPINE", "acantho", "mètre", "3", "1", "acantho", "métro (métron)", "Moyen", "Frequent",
                "acanthozoïde", "ANIMAL  DE L'ÉPINE", "acantho", "zoïde", "3", "1", "acantho", "", "Moyen", "Moyen",
                "acarpe", "POIGNET  SANS", "a", "carpe", "1", "3", "a", "", "Frequent", "Moyen",
                "acentrique", "CENTRÉ  SANS", "a", "centrique", "1", "1", "a", "centro", "Frequent", "Frequent",
                "acéphale", "TÊTE  SANS", "a", "céphale", "1", "2", "a", "", "Frequent", "Frequent",
                "acéphalie", "TÊTE  SANS", "a", "céphalie", "1", "2", "a", "", "Frequent", "Frequent",
                "acère", "CORNE  SANS", "a", "cère", "1", "3", "a", "cérato", "Frequent", "Moyen",
                "achrome", "COULEUR  SANS", "a", "chrome", "1", "1", "a", "chromo", "Frequent", "Frequent",
                "achromie", "COULEUR  SANS", "a", "chromie", "1", "1", "a", "chromo", "Frequent", "Frequent",
                "achronique", "TEMPOREL(LE)  SANS", "a", "chronique", "1", "1", "a", "chrono", "Frequent", "Moyen",
                "acide", "QUI TUE  SANS", "a", "cide", "1", "1", "a", "", "Frequent", "Frequent",
                "acinèse", "MOUVEMENT  SANS", "a", "cinèse", "1", "2", "a", "cinèse", "Frequent", "Moyen",
                "acinésie", "MOUVEMENT  SANS", "a", "cinésie", "1", "2", "a", "cinèse", "Frequent", "Moyen"
            };

            clsMotExistant.InitMots(lstMots, dicoMotsExistants);
        }

#endif

    }
}
