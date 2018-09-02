
using System;

namespace LogotronLib
{
    public static class clsConst
    {

#if DEBUG
        public static bool bDebug = true;
        public static bool bRelease = false;
#else
        public static bool bDebug = false;
        public static bool bRelease = true;
#endif

        public const int iNbColonnes = 8; // 21/08/2018 Fréquence ajoutée
        public const int iTirageImpossible = -1;

        public const string sSelectLogotron = "L";
        public const string sSelectDictionnaire = "D";
        public const string sHasard = "H";

        //public const string sCarSautDeLigne = "↲";
        public static readonly string sCrLf = Environment.NewLine; // "\r\n"

        public const int iColSegment = 0;
        public const int iColPrefixe = 0;
        public const int iColSuffixe = 0;
        public const int iColSens = 1;
        public const int iColLogotron = 2;
        public const int iColNiveau = 3;
        public const int iColEtym = 4;
        public const int iColUnicite = 5;

        // Séparateur entre la définition du suffixe et celle(s) du(des) préfixe(s)
        public const string sSepDef = "  ";

        public const bool bInclureNeologismesAmusants = true; // 21/06/2018
    }

    public static class enumOrigine
    {
        public const string sGrec = "Grec";
        public const string sLatin = "Latin";
        public const string sAutre = "Autre"; // Danois, Italien, Anglais, ...
        public const string sNonPrecise = "Non précisé";
        public const string sGrecoLatin = "Gréco-latin";
        public const string sNeologismeAmusant = "Néologisme amusant"; // Fiscalo-
    }
    
    public static class enumNiveau
    {
        public const string N1 = "1";
        public const string N2 = "2";
        public const string N3 = "3";

        public static int iCoef(string sNiveaux)
        {
            int iCoefNiv = 0;
            switch (sNiveaux)
            {
                case "1 ": iCoefNiv = 1; break;
                case "1 2 ": iCoefNiv = 2; break;
                case "2 ": iCoefNiv = 3; break;
                case "1 2 3 ": iCoefNiv = 5; break;
                case "1 3 ": iCoefNiv = 6; break;
                case "2 3 ": iCoefNiv = 8; break;
                case "3 ": iCoefNiv = 10; break;
                default: break;
            }
            return iCoefNiv;
        }
    }

    public static class enumFrequence
    {
        public const string Frequent = "Frequent";
        public const string Moyen = "Moyen";
        public const string Rare = "Rare";
        public const string Absent = "Absent"; // Impossible, sauf si les fréquences ne sont plus à jour
    }

    public static class enumFrequenceAbrege
    {
        public const string Frequent = "Fréq.";
        public const string Moyen = "Moy.";
        public const string Rare = "Rare";
        public const string Absent = "Abs.";

        public static string sConv(string sFreqAbrege)
        {
            string sFreqComplet = "";
            switch (sFreqAbrege)
            {
                case enumFrequenceAbrege.Frequent:
                    sFreqComplet = enumFrequence.Frequent;
                    break;
                case enumFrequenceAbrege.Moyen:
                    sFreqComplet = enumFrequence.Moyen;
                    break;
                case enumFrequenceAbrege.Rare:
                    sFreqComplet = enumFrequence.Rare;
                    break;
                case enumFrequenceAbrege.Absent:
                    sFreqComplet = enumFrequence.Absent;
                    break;
            }
            return sFreqComplet;
        }

        public static int iCoef(string sFrequences)
        {
            int iCoefFreq = 0;
            switch (sFrequences)
            {
                case "Fréq. ": iCoefFreq = 1; break;
                case "Fréq. Abs. ": iCoefFreq = 1; break;
                case "Fréq. Moy. ": iCoefFreq = 2; break;
                case "Fréq. Moy. Abs. ": iCoefFreq = 2; break;
                case "Moy. ": iCoefFreq = 3; break;
                case "Moy. Abs. ": iCoefFreq = 3; break;
                case "Fréq. Moy. Rare ": iCoefFreq = 5; break;
                case "Fréq. Moy. Rare Abs. ": iCoefFreq = 5; break;
                case "Fréq. Rare ": iCoefFreq = 6; break;
                case "Fréq. Rare Abs. ": iCoefFreq = 6; break;
                case "Moy. Rare ": iCoefFreq = 8; break;
                case "Moy. Rare Abs. ": iCoefFreq = 8; break;
                case "Rare ": iCoefFreq = 10; break;
                case "Rare Abs. ": iCoefFreq = 10; break;
                case "Abs. ": iCoefFreq = 10; break;
                default: break;
            }
            return iCoefFreq;
        }
    }
}
